using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Quest/Requirement Quest", fileName = "ReqQuest")]
public class RequirementQuest : Quest
{
    public Requirement requirementChoice1;
    public Requirement requirementChoice2;

    public override TypeOfQuest QuestType()
    {
        return TypeOfQuest.RequirementQuest;
    }
}


[Serializable]
public class Requirement
{
    public int moneyCost;
    public float templeReadiness;
    public int kiCost;
}
