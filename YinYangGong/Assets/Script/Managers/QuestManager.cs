using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestManager : MonoBehaviour
{
    int yinYangBalance = 0;


    [Header("Valeur de départ du niveau")]
    public int maxClanBalance = 100;
    public int maxKi = 100;
    [SerializeField]
    int disciple = 0;
    [SerializeField]
    int ki = 0;
    [SerializeField]
    int argent = 0;
    [SerializeField]
    float templeReadinessToAchieve = 100;

    float templeReadiness = 0;

    private float argentBonus = 1f;
    private float discipleBonus = 1f;
    private float kiBonus = 1f;

    public Level level;
    public RandomQuestList randomQuestList;

    private Level filledLevel;
    private RandomQuestList tempQuestList;

    UIController uIController;

    int currentQuestIndex = 0;
    public Quest currentQuest;

    private void Start()
    {
        uIController = GameObject.FindGameObjectWithTag("Canvas").GetComponent<UIController>();

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
        uIController.SetNPCFile(filledLevel);

    }

    //Come fill all the null element in the level list by random quest in the RandomQuestList
    private void SideQuestFiller()
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

    public void NextQuest()
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
            currentQuest = filledLevel.questList[currentQuestIndex];

            uIController.SetQuest(currentQuest);
            uIController.SetNbQuestLeft(filledLevel.questList.Count - currentQuestIndex);
            uIController.UpdateNPCList(filledLevel, currentQuestIndex);

            if (currentQuest.QuestType() == TypeOfQuest.RequirementQuest)
            {
                RequirementQuest requirementQuest = (RequirementQuest)currentQuest;
                CheckRequirement(requirementQuest);
            }
        }
        else
        {
            CheckIfPlayerWon();
        }
        CheckIfStillWinning();
    }

    private void CheckRequirement(RequirementQuest requirementQuest)
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

    private string DisableReason(Requirement requirement)
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

    private void CheckIfPlayerWon()
    {
        if (templeReadiness >= templeReadinessToAchieve)
            uIController.Victory();
        else
            uIController.Defeat(DefeatType.ReadinessLoss);
    }

    public InventoryItemManager InventoryItemManager;


    private bool choice1WasMade = false;
    public void UpdateStatChoix1()
    {
        QuestReward(currentQuest.questDefinition.rewardChoice1);
        choice1WasMade = true;
    }

    public void UpdateStatChoix2()
    {
        QuestReward(currentQuest.questDefinition.rewardChoice2);
        choice1WasMade = false;
    }

    private void MerchantQuestReward(Item item)
    {
        InventoryItemManager.AddItem(item);
        if (item.ItemType() == TypeOfItem.UpgradeItem)
            InventoryItemManager.UseItem(item);
    }

    private void QuestReward(Reward reward)
    {
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
    }

    private void CheckIfStillWinning()
    {
        if (yinYangBalance >= maxClanBalance)
            uIController.Defeat(DefeatType.HuangseiLoss);
        else if (yinYangBalance <= -maxClanBalance)
            uIController.Defeat(DefeatType.SusodaLoss);
        else if (disciple <= 0)
            uIController.Defeat(DefeatType.DiscipleLoss);
        else if (ki <= 0)
            uIController.Defeat(DefeatType.KiLoss);
        else if (argent < 0)
            uIController.Defeat(DefeatType.MoneyLoss);
    }

    public void AddQuest(int index, Quest questToAdd)
    {
        if (index > filledLevel.questList.Count)
            index = filledLevel.questList.Count;

        filledLevel.questList.Insert(index, questToAdd);
    }

    public Level GetFilledLevel()
    {
        return filledLevel;
    }

    public Quest GetActiveQuest()
    {
        return currentQuest;
    }

    public float GetReadiness()
    {
        return templeReadiness;
    }
    public void AddReadiness(float newReadiness)
    {
        templeReadiness += newReadiness;
        uIController.SetReadiness(templeReadiness);
    }

    public int GetYinYangBalance()
    {
        return yinYangBalance;
    }

    public void SetYinYangBalance(int newYinYangBalance)
    {
        yinYangBalance = newYinYangBalance;
        uIController.SetCurrentBalance(yinYangBalance);
    }

    public int GetMoney()
    {
        return argent;
    }

    public void RemoveMoney(int money)
    {
        argent -= money;
        uIController.SetArgent(argent);
    }

    public void AddMoney(int money)
    {
        argent += money;
        uIController.SetArgent(argent);
    }


    public void SetArgentBonus(float bonus)
    {
        argentBonus += bonus;
    }

    public int GetDisciple()
    {
        return disciple;
    }
    public void AddDisciple(int newDisciple)
    {
        disciple += newDisciple;
        uIController.SetDisciple(disciple);
    }

    public void SetDiscipleBonus(float bonus)
    {
        discipleBonus += bonus;
    }

    public float GetKi()
    {
        return ki;
    }

    public void AddKi(int addedKi)
    {
        ki += addedKi;
    }

    public void SetKiBonus(float bonus)
    {
        kiBonus += bonus;
    }

    public int GetCurrentQuestPosition()
    {
        return currentQuestIndex;
    }

    public void ChangeValue(int maxYinYang, int balance, float readiness, int money, int nbDisciple, int nbKi)
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

    public bool HasEnoughMoney(int Cost)
    {
        return Cost <= argent;
    }
}
