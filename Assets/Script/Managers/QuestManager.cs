using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestManager : MonoBehaviour
{
    public Level level;
    public RandomQuestList randomQuestList;

    private Level filledLevel;
    private RandomQuestList tempQuestList;

    private void Start()
    {
        //creates a copy to not lose original data (important)
        filledLevel = Instantiate(level);
        tempQuestList = Instantiate(randomQuestList);
        SideQuestFiller();
    }

    //Come fill all the null element in the level list by random quest in the RandomQuestList
    private void SideQuestFiller()
    {
        for (int i = 0; i < level.questList.Count; i++)
        {
            if(filledLevel.questList[i] == null)
            {
                int randomNumber = Random.Range(0, tempQuestList.randomQuestList.Count);
                var quest = tempQuestList.randomQuestList[randomNumber];
                tempQuestList.randomQuestList.Remove(quest);
                filledLevel.questList.Insert(i, quest);
                filledLevel.questList.RemoveAt(i + 1);
            }
        }

        foreach (var item in filledLevel.questList)
        {
            Debug.Log(item.questDefinition.questName);
        }
    }
}
