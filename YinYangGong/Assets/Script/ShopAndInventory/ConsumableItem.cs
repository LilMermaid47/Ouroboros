using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewItemClass", menuName = "Item/Consumable")]
public class ConsumableItem : Item
{
    public ConsumableItemList consumableItemList;
    private QuestManager questManager;

    public override void UseItem()
    {
        questManager = GameObject.FindGameObjectWithTag("QuestManager").GetComponent<QuestManager>();
        switch (consumableItemList)
        {
            case ConsumableItemList.SeerPotion:
                break;
            case ConsumableItemList.PerfectGoodbyeGift:
                questManager.NextQuest();
                break;
            case ConsumableItemList.ReversePotion:
                break;
            case ConsumableItemList.EquilibriumPotion:
                break;
            case ConsumableItemList.BribedWithFood:
                break;
            case ConsumableItemList.KiEnergyPotion:
                break;
            case ConsumableItemList.GetReady:
                break;
            case ConsumableItemList.WealthPotion:
                break;
        }
    }
    public override TypeOfItem ItemType()
    {
        return TypeOfItem.ConsumableItem;
    }
}

public enum ConsumableItemList
{
    SeerPotion,
    PerfectGoodbyeGift,
    ReversePotion,
    EquilibriumPotion,
    BribedWithFood,
    KiEnergyPotion,
    GetReady,
    WealthPotion
}
