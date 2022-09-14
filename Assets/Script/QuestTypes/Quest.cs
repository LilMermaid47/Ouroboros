using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Quest/Quest",fileName = "Quest")]
public class Quest : ScriptableObject
{
    public QuestDefinition questDefinition;
}

[Serializable]
public class QuestDefinition
{
    [Header("Basic quest info")]
    public string questName;
    public string questGiverName;

    [TextArea(minLines:2, maxLines:5)]
    public string questDescription;

    [Header("Choice descrption & Reveal Description")]
    public string choice1Name;
    [TextArea(minLines: 2, maxLines: 5)]
    public string choice1Description;
    [TextArea(minLines: 2, maxLines: 5)]
    public string choice1Reveal;

    public string choice2Name;
    [TextArea(minLines: 2, maxLines: 5)]
    public string choice2Description;
    [TextArea(minLines: 2, maxLines: 5)]
    public string choice2Reveal;

    [Header("Quest Rewards")]
    public Reward rewardChoice1;
    public Reward rewardChoice2;
}

[Serializable]
public class Reward
{
    public int moneyReward;
    public float templeReadiness;

    public ClanDefinition recompenseClanSusoda;
    public ClanDefinition recompenseClanHuangsei;
}

[Serializable]
public class ClanDefinition
{
    public float honor;
    public int discple;
    public float devotion;
}
