using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestManager : MonoBehaviour
{
    protected int yinYangBalance = 0;


    [Header("Valeur de départ du niveau")]
    public int maxClanBalance = 100;
    public int maxKi = 100;
    [SerializeField]
    protected int disciple = 0;
    [SerializeField]
    protected int ki = 0;
    [SerializeField]
    protected int argent = 0;
    [SerializeField]
    protected float templeReadinessToAchieve = 100;
    [SerializeField]
    protected float clanMusicStartAt = 0.5f;

    protected float templeReadiness = 0;

    protected float argentBonus = 1f;
    protected float discipleBonus = 1f;
    protected float kiBonus = 1f;

    public Level level;
    public RandomQuestList randomQuestList;

    protected Level filledLevel;
    protected RandomQuestList tempQuestList;

    protected UIController uIController;
    protected MusicSFXManager musicSFXManager;

    protected int currentQuestIndex = 0;
    public Quest currentQuest;

    private void Start()
    {
        uIController = GameObject.FindGameObjectWithTag("Canvas").GetComponent<UIController>();
        musicSFXManager = GameObject.FindGameObjectWithTag("Music").GetComponent<MusicSFXManager>();

        //creates a copy to not lose original data (important)
        filledLevel = Instantiate(level);
        tempQuestList = Instantiate(randomQuestList);
        SideQuestFiller();
        currentQuest = filledLevel.questList[currentQuestIndex];

        uIController.SetMaxBalance(maxClanBalance);
        uIController.SetMaxKi(maxKi);

        uIController.SetQuest(currentQuest);
        uIController.SetStartingRessources(argent, yinYangBalance, (templeReadiness / templeReadinessToAchieve) * 100, disciple, ki);
        uIController.SetNbQuestLeft(filledLevel.questList.Count - currentQuestIndex);
        uIController.SetNPCFile(filledLevel, currentQuestIndex);

    }

    //Come fill all the null element in the level list by random quest in the RandomQuestList
    protected virtual void SideQuestFiller()
    {
        for (int i = 0; i < level.questList.Count; i++)
        {
            if (filledLevel.questList[i] == null)
            {
                int randomNumber = Random.Range(0, tempQuestList.randomQuestList.Count);
                var quest = tempQuestList.randomQuestList[randomNumber];
                tempQuestList.randomQuestList.Remove(quest);
                filledLevel.questList[i] = quest;
            }
        }
    }

    public virtual void NextQuest()
    {
        if (currentQuest.QuestType() == TypeOfQuest.MerchantQuest)
        {
            MerchantQuest merchantQuest = (MerchantQuest)currentQuest;

            if (choice1WasMade)
                MerchantQuestReward(merchantQuest.itemChoice1.item);
            else
                MerchantQuestReward(merchantQuest.itemChoice2.item);
        }

        currentQuestIndex++;

        if (currentQuestIndex < filledLevel.questList.Count)
        {
            UpdateCurrentQuest();
        }
        else
        {
            CheckIfPlayerWon();
        }

        CheckIfStillWinning();
    }

    public virtual void LastQuest()
    {
        currentQuestIndex--;
        if(currentQuestIndex < 0)
            currentQuestIndex = 0;

        if(lastReward != null)
        {
            RemoveQuestReward(lastReward);
        }

        UpdateCurrentQuest();
    }


    protected virtual void UpdateCurrentQuest()
    {
        currentQuest = filledLevel.questList[currentQuestIndex];

        uIController.SetQuest(currentQuest);
        uIController.SetNbQuestLeft(filledLevel.questList.Count - currentQuestIndex);
        uIController.SetNPCFile(filledLevel, currentQuestIndex);

        if (currentQuest.QuestType() == TypeOfQuest.RequirementQuest)
        {
            RequirementQuest requirementQuest = (RequirementQuest)currentQuest;
            CheckRequirement(requirementQuest);
        }
    }

    protected virtual void CheckRequirement(RequirementQuest requirementQuest)
    {
        Requirement requirement = requirementQuest.requirementChoice1;

        if ((requirement.templeReadiness <= templeReadiness || requirement.templeReadiness == 0) &&
            requirement.moneyCost <= argent &&
            requirement.kiCost <= ki &&
            requirement.disciples <= disciple)
        {
            uIController.FirstChoiceBtnActivate(true);
        }
        else
        {
            uIController.DisableFirstChoiceBtn(DisableReason(requirement));
        }

        requirement = requirementQuest.requirementChoice2;

        if ((requirement.templeReadiness <= templeReadiness || requirement.templeReadiness == 0) &&
            requirement.moneyCost <= argent &&
            requirement.kiCost <= ki &&
            requirement.disciples <= disciple)
        {
            uIController.SecondChoiceBtnActivate(true);
        }
        else
        {
            uIController.DisableSecondChoiceBtn(DisableReason(requirement));
        }
    }

    protected virtual string DisableReason(Requirement requirement)
    {
        string reason = "";

        if (requirement.templeReadiness >= templeReadiness)
            reason = "<b><u>Temple readiness\ntoo low!</b></u>";
        else if (requirement.moneyCost >= argent)
            reason = "<b><u>Not enough Yuan!</b></u>";
        else if (requirement.kiCost >= ki)
            reason = "<b><u>Not enough Ki!</b></u>";
        else if (requirement.disciples >= disciple)
            reason = "<b><u>You need more disciples!</b></u>";

        return reason;
    }

    protected virtual void CheckIfPlayerWon()
    {
        if (templeReadiness >= templeReadinessToAchieve)
        {
            uIController.Victory();
            musicSFXManager.ChangeMusic(ChooseMusic.VictoryMusic);
        }
        else
        {
            uIController.Defeat(DefeatType.ReadinessLoss);
            musicSFXManager.ChangeMusic(ChooseMusic.DefeatMusic);
        }
    }

    public InventoryItemManager InventoryItemManager;


    protected bool choice1WasMade = false;
    public virtual void UpdateStatChoix1()
    {
        QuestReward(currentQuest.questDefinition.rewardChoice1);
        choice1WasMade = true;
    }

    public virtual void UpdateStatChoix2()
    {
        QuestReward(currentQuest.questDefinition.rewardChoice2);
        choice1WasMade = false;
    }

    protected virtual void MerchantQuestReward(Item item)
    {
        InventoryItemManager.AddItem(item);
        if (item.ItemType() == TypeOfItem.UpgradeItem)
            InventoryItemManager.UseItem(item);
    }


    Reward lastReward;
    protected virtual void QuestReward(Reward reward)
    {
        lastReward = reward;

        argent += Mathf.FloorToInt(reward.moneyReward * argentBonus);
        templeReadiness += reward.templeReadiness;
        yinYangBalance += reward.yinYangBalance;
        disciple += Mathf.FloorToInt(reward.nbDisciple * discipleBonus);
        ki += Mathf.FloorToInt(reward.nbKi * kiBonus);
        uIController.SetRewardsRessources(argent, yinYangBalance, (templeReadiness / templeReadinessToAchieve) * 100, disciple, ki);

        if (reward.unlockQuestChoice.unlockedQuest != null)
        {
            AddQuest(currentQuestIndex + reward.unlockQuestChoice.nbQuestLater, reward.unlockQuestChoice.unlockedQuest);
        }

        Debug.Log(yinYangBalance / (float)maxClanBalance);

        if (yinYangBalance / (float)maxClanBalance >= clanMusicStartAt)
        {
            musicSFXManager.ChangeMusic(ChooseMusic.SusodaMusic);
        }
        else if (yinYangBalance / (float)maxClanBalance <= -clanMusicStartAt)
        {
            musicSFXManager.ChangeMusic(ChooseMusic.HuangseiMusic);
        }
        else
        {
            musicSFXManager.ChangeMusic(ChooseMusic.NormalMusic);
        }
    }

    protected virtual void RemoveQuestReward(Reward reward)
    {
        argent -= Mathf.FloorToInt(reward.moneyReward * argentBonus);
        templeReadiness -= reward.templeReadiness;
        yinYangBalance -= reward.yinYangBalance;
        disciple -= Mathf.FloorToInt(reward.nbDisciple * discipleBonus);
        ki -= Mathf.FloorToInt(reward.nbKi * kiBonus);
        uIController.SetRewardsRessources(argent, yinYangBalance, (templeReadiness / templeReadinessToAchieve) * 100, disciple, ki);

        if (yinYangBalance / maxClanBalance >= clanMusicStartAt)
        {
            musicSFXManager.ChangeMusic(ChooseMusic.SusodaMusic);
        }
        else if (yinYangBalance / maxClanBalance <= -clanMusicStartAt)
        {
            musicSFXManager.ChangeMusic(ChooseMusic.HuangseiMusic);
        }
        else
        {
            musicSFXManager.ChangeMusic(ChooseMusic.NormalMusic);
        }
    }

    protected virtual void CheckIfStillWinning()
    {
        if (yinYangBalance >= maxClanBalance)
        {
            musicSFXManager.ChangeMusic(ChooseMusic.DefeatMusic);
            uIController.Defeat(DefeatType.HuangseiLoss);
        }
        else if (yinYangBalance <= -maxClanBalance)
        {
            musicSFXManager.ChangeMusic(ChooseMusic.DefeatMusic);
            uIController.Defeat(DefeatType.SusodaLoss);
        }
        else if (disciple <= 0)
        {
            musicSFXManager.ChangeMusic(ChooseMusic.DefeatMusic);
            uIController.Defeat(DefeatType.DiscipleLoss);
        }
        else if (ki <= 0)
        {
            musicSFXManager.ChangeMusic(ChooseMusic.DefeatMusic);
            uIController.Defeat(DefeatType.KiLoss);
        }
        else if (argent < 0)
        {
            musicSFXManager.ChangeMusic(ChooseMusic.DefeatMusic);
            uIController.Defeat(DefeatType.MoneyLoss);
        }
    }

    public virtual void AddQuest(int index, Quest questToAdd)
    {
        if (index > filledLevel.questList.Count)
            index = filledLevel.questList.Count;

        filledLevel.questList.Insert(index, questToAdd);
    }

    public virtual Level GetFilledLevel()
    {
        return filledLevel;
    }

    public virtual Quest GetActiveQuest()
    {
        return currentQuest;
    }

    public virtual float GetReadiness()
    {
        return templeReadiness;
    }
    public virtual void AddReadiness(float newReadiness)
    {
        templeReadiness += newReadiness;
        uIController.SetReadiness(templeReadiness);
    }

    public virtual int GetYinYangBalance()
    {
        return yinYangBalance;
    }

    public virtual void SetYinYangBalance(int newYinYangBalance)
    {
        yinYangBalance = newYinYangBalance;
        uIController.SetCurrentBalance(yinYangBalance);
    }

    public virtual int GetMoney()
    {
        return argent;
    }

    public virtual void RemoveMoney(int money)
    {
        argent -= money;
        uIController.SetArgent(argent);
    }

    public virtual void AddMoney(int money)
    {
        argent += money;
        uIController.SetArgent(argent);
    }


    public virtual void SetArgentBonus(float bonus)
    {
        argentBonus += bonus;
    }

    public virtual int GetDisciple()
    {
        return disciple;
    }
    public virtual void AddDisciple(int newDisciple)
    {
        disciple += newDisciple;
        uIController.SetDisciple(disciple);
    }

    public virtual void SetDiscipleBonus(float bonus)
    {
        discipleBonus += bonus;
    }

    public virtual float GetKi()
    {
        return ki;
    }

    public virtual void AddKi(int addedKi)
    {
        ki += addedKi;
    }

    public virtual void SetKiBonus(float bonus)
    {
        kiBonus += bonus;
    }

    public virtual int GetCurrentQuestPosition()
    {
        return currentQuestIndex;
    }

    public virtual void ChangeValue(int maxYinYang, int balance, float readiness, int money, int nbDisciple, int nbKi)
    {
        argent = money;
        maxClanBalance = maxYinYang;
        yinYangBalance = balance;
        templeReadiness = readiness;
        ki = nbKi;
        disciple = nbDisciple;


        uIController.SetMaxBalance(maxClanBalance);

        uIController.SetStartingRessources(argent, yinYangBalance, (templeReadiness / templeReadinessToAchieve) * 100, disciple, ki);
    }

    public virtual bool HasEnoughMoney(int Cost)
    {
        return Cost <= argent;
    }
}
