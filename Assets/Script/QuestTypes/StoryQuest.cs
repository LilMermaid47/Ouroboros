using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Quest/Story Quest", fileName = "StoryQuest")]
public class StoryQuest : Quest
{
    public UnlockedQuest unlockQuestChoice1;
    public UnlockedQuest unlockQuestChoice2;

}

[Serializable]
public class UnlockedQuest
{
    public int nbQuestLater;
    public Quest unlockedQuest;
}

