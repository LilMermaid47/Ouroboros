using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestManager : MonoBehaviour
{
    public Level level;
    public RandomQuestList randomQuestList;

    private Level filledLevel;
    private RandomQuestList tempQuestList;

    int currentQuestIndex = 0;
    Quest currentQuest;

    private void Start()
    {
        //creates a copy to not lose original data (important)
        filledLevel = Instantiate(level);
        tempQuestList = Instantiate(randomQuestList);
        SideQuestFiller();
        currentQuest = filledLevel.questList[currentQuestIndex];
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
        currentQuest = filledLevel.questList[currentQuestIndex];
    }
    
    public void InsertQuest(int index, Quest questToAdd)
    {
        filledLevel.questList.Insert(index, questToAdd);
    }
}
