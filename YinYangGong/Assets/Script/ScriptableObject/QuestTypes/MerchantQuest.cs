using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Quest/MerchantQuest", fileName = "MerchantQuest")]
public class MerchantQuest : Quest
{
    public int itemPrice;
    public Item item;

    public override TypeOfQuest QuestType()
    {
        return TypeOfQuest.MerchantQuest;
    }
    //public object gpi to buy
}
