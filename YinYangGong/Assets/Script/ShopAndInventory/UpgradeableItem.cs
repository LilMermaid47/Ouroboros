using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using UnityEngine;

[CreateAssetMenu(fileName = "NewItemClass", menuName = "Item/Upgradeable")]
public class UpgradeableItem : Item
{
    public UpgradeItemList upgradeItemList;
    public float bonus = 0.1f;

    private QuestManager questManager;
    private UIController uIController;

    public override TypeOfItem ItemType()
    {
        return TypeOfItem.UpgradeItem;
    }

    public override void UseItem()
    {
        questManager = GameObject.FindGameObjectWithTag("QuestManager").GetComponent<QuestManager>();
        uIController = GameObject.FindGameObjectWithTag("Canvas").GetComponent<UIController>();

        switch (upgradeItemList) 
        {
            case UpgradeItemList.Shop:
                uIController.EnableShop(true);
                break;
            case UpgradeItemList.GoldenBuddhaStatue:
                questManager.SetArgentBonus(bonus);
                break;
            case UpgradeItemList.JadeBuddhaStatue:
                uIController.IncreaseMaxBalance((int)bonus);
                break;
            case UpgradeItemList.DisciplesSanctuary:
                questManager.SetDiscipleBonus(bonus);
                break;
            case UpgradeItemList.BlueBuddhaStatue:
                questManager.SetKiBonus(bonus);
                break;
            case UpgradeItemList.SusodasGarden:
                break;
            case UpgradeItemList.HuangseisWatchTower:
                break;
            case UpgradeItemList.Palisade:
                questManager.AddReadiness(bonus);
                break;
            case UpgradeItemList.WoodenWall:
                questManager.AddReadiness(bonus);
                break;
            case UpgradeItemList.StoneWall:
                questManager.AddReadiness(bonus);
                break;
            case UpgradeItemList.SusodasStatue:
                break;
            case UpgradeItemList.HuangseisStatue:
                break;
            case UpgradeItemList.MothersTree:
                break;
        }
    }
}
public enum UpgradeItemList
{
    Shop,
    GoldenBuddhaStatue,
    JadeBuddhaStatue,
    DisciplesSanctuary,
    BlueBuddhaStatue,
    SusodasGarden,
    HuangseisWatchTower,
    Palisade,
    WoodenWall,
    StoneWall,
    SusodasStatue,
    HuangseisStatue,
    MothersTree
}
