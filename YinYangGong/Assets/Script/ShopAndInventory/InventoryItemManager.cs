using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryItemManager : ItemManager
{
    [SerializeField]
    private ItemSlotController CurrencySlot = null;

    public bool HasEnoughMoney(int Cost)
    {
        return Cost <= CurrencySlot.GetQuantity();
    }

    public void AddMoney(int QuantityToAdd)
    {
        CurrencySlot.AddQuantity(QuantityToAdd);
    }

    public void RemoveMoney(int QuantityToRemove)
    {
        CurrencySlot.RemoveQuantity(QuantityToRemove);
    }
}
