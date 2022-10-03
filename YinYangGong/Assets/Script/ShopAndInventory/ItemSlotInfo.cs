using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ItemSlotInfo
{
    [SerializeField]
    public Item Item = null;
    [SerializeField]
    public int Quantity = 0;

    public ItemSlotInfo(Item InItem, int InQuantity)
    {
        Item = InItem;
        Quantity = InQuantity;
    }
}
