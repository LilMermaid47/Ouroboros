using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewItemClass", menuName = "Item/Upgradeable")]
public class UpgradeableItem : Item
{
    public override TypeOfItem ItemType()
    {
        return TypeOfItem.UpgradeItem;
    }

    public override void UseItem()
    {
        throw new System.NotImplementedException();
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
