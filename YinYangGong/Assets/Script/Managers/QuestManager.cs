using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestManager : MonoBehaviour
{
    [Header("Valeur de DEBUG")]
    public int yinYangBalance = 0;


    [Header("Valeur de d�part du niveau")]
    public int maxClanBalance = 100;
    [SerializeField]
    int disciple = 0;
    [SerializeField]
    int ki = 0;
    [SerializeField]
    int argent = 0;
    [SerializeField]
    float templeReadinessToAchieve = 100;

    float templeReadiness = 0;


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

        uIController.SetQuest(currentQuest);
        uIController.SetStartingRessources(argent, yinYangBalance, (templeReadiness / templeReadinessToAchieve) * 100, disciple, ki);
        uIController.SetNbQuestLeft(filledLevel.questList.Count - currentQuestIndex);
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
        currentQuestIndex++;
        if (currentQuestIndex < filledLevel.questList.Count)
        {
            currentQuest = filledLevel.questList[currentQuestIndex];

            if (currentQuest.QuestType() == TypeOfQuest.RequirementQuest)
            {
                RequirementQuest requirementQuest = (RequirementQuest)currentQuest;
                CheckRequirement(requirementQuest);
            }

            uIController.SetQuest(currentQuest);
            uIController.SetNbQuestLeft(filledLevel.questList.Count - currentQuestIndex);
        }
        else
        {
            CheckIfPlayerWon();
        }
        CheckIfStillWinning();
    }

    private void CheckRequirement(RequirementQuest requirementQuest)
    {
        if (requirementQuest.requirementChoice1.templeReadiness < templeReadiness ||
            requirementQuest.requirementChoice1.moneyCost < argent ||
            requirementQuest.requirementChoice1.kiCost < ki ||
            requirementQuest.requirementChoice1.disciples < disciple)
        {
            uIController.FirstChoiceBtnActivate(false);
        }
        else
        {
            uIController.FirstChoiceBtnActivate(true);
        }
        if (requirementQuest.requirementChoice2.templeReadiness < templeReadiness ||
            requirementQuest.requirementChoice2.moneyCost < argent ||
            requirementQuest.requirementChoice2.kiCost < ki ||
            requirementQuest.requirementChoice2.disciples < disciple)
        {
            uIController.SecondChoiceBtnActivate(false);
        }
        else
        {
            uIController.SecondChoiceBtnActivate(true);
        }
    }

    private void CheckIfPlayerWon()
    {
        if (templeReadiness >= templeReadinessToAchieve)
            uIController.Victory();
        else
            uIController.Defeat(DefeatType.ReadinessLoss);
    }

    public void UpdateStatChoix1()
    {
        QuestReward(currentQuest.questDefinition.rewardChoice1);
    }

    public void UpdateStatChoix2()
    {
        QuestReward(currentQuest.questDefinition.rewardChoice2);
    }

    private void QuestReward(Reward reward)
    {
        argent += reward.moneyReward;
        templeReadiness += reward.templeReadiness;
        yinYangBalance += reward.yinYangBalance;
        disciple += reward.nbDisciple;
        ki += reward.nbKi;
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
        else if (argent < 0)
            uIController.Defeat(DefeatType.MoneyLoss);
        else if (ki < 0)
            uIController.Defeat(DefeatType.KiLoss);
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

    public int GetMoney()
    {
        return argent;
    }

    public int GetDisciple()
    {
        return disciple;
    }

    public float GetKi()
    {
        return ki;
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
}
