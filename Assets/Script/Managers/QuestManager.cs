using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestManager : MonoBehaviour
{
    [Header("Valeur de départ du niveau")]
    [SerializeField]
    int argent = 0;
    int yinYangBalance = 0;
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

        uIController.SetRessources(argent, yinYangBalance, templeReadiness, Clan.Susoda, clanSusoda.discple, Clan.Huangsei, clanHuangsei.discple);
    }

    public void InsertQuest(int index, Quest questToAdd)
    {
        filledLevel.questList.Insert(index, questToAdd);
    }
}
