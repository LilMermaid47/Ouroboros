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
                EquilibriumPotion();
                break;
            case ConsumableItemList.BribedWithFood:
                BribedWithFood();
                break;
            case ConsumableItemList.KiEnergyPotion:
                break;
            case ConsumableItemList.GetReady:
                GetReady();
                break;
            case ConsumableItemList.WealthPotion:
                break;
        }
    }
    public override TypeOfItem ItemType()
    {
        return TypeOfItem.ConsumableItem;
    }

    int readinessMin = 4;
    int readinessMax = 8;
    private void GetReady()
    {
        questManager.AddReadiness(Random.Range(readinessMin, readinessMax));
    }

    int bribedWithFoodMin = 4;
    int bribedWithFoodMax = 8;
    private void BribedWithFood()
    {
        questManager.AddReadiness(Random.Range(bribedWithFoodMin, bribedWithFoodMax));
    }

    int equlibriumYinYangDiff = 10;
    private void EquilibriumPotion()
    {
        int currentYinYang = questManager.GetYinYangBalance();
        int newYinYang;

        if(currentYinYang > 0)
        {
            newYinYang = Mathf.Clamp(currentYinYang - equlibriumYinYangDiff, 0, 50);
        }
        else
        {
            newYinYang = Mathf.Clamp(currentYinYang + equlibriumYinYangDiff, -50, 0);
        }

        questManager.SetYinYangBalance(newYinYang);
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
