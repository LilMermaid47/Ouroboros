using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Quest/MerchantQuest", fileName = "MerchantQuest")]
public class MerchantQuest : Quest
{
    public MerchantItem itemChoice1;
    public MerchantItem itemChoice2;

    public override TypeOfQuest QuestType()
    {
        return TypeOfQuest.MerchantQuest;
    }
    //public object gpi to buy
}

[Serializable]
public class MerchantItem
{
    public int itemPrice;
    public Item item;
}
