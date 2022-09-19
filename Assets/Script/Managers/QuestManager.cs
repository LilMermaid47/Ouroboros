using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestManager : MonoBehaviour
{
    [Header("Valeur de d�part du niveau")]
    public float clanHonorDevotionModifier = 0.2f;

    [SerializeField]
    int argent = 0;
    public int yinYangBalance = 0;
    float templeReadiness = 0;
    [SerializeField]
    ClanDefinition clanSusoda = new ClanDefinition(0, 0, 0);
    [SerializeField]
    ClanDefinition clanHuangsei = new ClanDefinition(0, 0, 0);

    public Level level;
    public RandomQuestList randomQuestList;

    private Level filledLevel;
    private RandomQuestList tempQuestList;

    UIController uIController;

    int currentQuestIndex = 0;
    Quest currentQuest;

    private void Start()
    {
        uIController = GameObject.FindGameObjectWithTag("Canvas").GetComponent<UIController>();

        //creates a copy to not lose original data (important)
        filledLevel = Instantiate(level);
        tempQuestList = Instantiate(randomQuestList);
        SideQuestFiller();
        currentQuest = filledLevel.questList[currentQuestIndex];

        uIController.SetMaxBalance(100);

        uIController.SetQuest(currentQuest);
        uIController.SetRessources(argent, yinYangBalance, templeReadiness, Clan.Susoda, clanSusoda.discple, Clan.Huangsei, clanHuangsei.discple);
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
            uIController.SetQuest(currentQuest);

        }
        else
        {
            Debug.Log("LvlCompleted");
        }
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
        templeReadiness += templeReadiness;
        clanHuangsei.ClanReward(reward.recompenseClanHuangsei);
        clanSusoda.ClanReward(reward.recompenseClanSusoda);

        yinYangBalance = YinYangCalculator();

        uIController.SetRessources(argent, yinYangBalance, templeReadiness, Clan.Susoda, clanSusoda.discple, Clan.Huangsei, clanHuangsei.discple);

        if(reward.unlockQuestChoice.unlockedQuest != null)
        {
            if (reward.AdditiveQuest)
                AddQuest(reward.unlockQuestChoice.nbQuestLater, reward.unlockQuestChoice.unlockedQuest);
            else
                InsertQuest(reward.unlockQuestChoice.nbQuestLater, reward.unlockQuestChoice.unlockedQuest);
        }
    }

    public void AddQuest(int index, Quest questToAdd)
    {
        filledLevel.questList.Insert(index, questToAdd);
    }

    public void InsertQuest(int index, Quest questToAdd)
    {
        filledLevel.questList[index] = questToAdd;
    }

    private int YinYangCalculator()
    {
        float susodaPower = (1 - clanHonorDevotionModifier) * clanSusoda.discple * clanSusoda.honor + (1 + clanHonorDevotionModifier) * clanSusoda.discple * clanSusoda.devotion;
        float huangseiPower = (1 + clanHonorDevotionModifier) * clanHuangsei.discple * clanHuangsei.honor + (1 - clanHonorDevotionModifier) * clanHuangsei.discple * clanHuangsei.devotion;
        return (int) (susodaPower - huangseiPower);
    }
}
