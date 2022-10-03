using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoreItemManager : ItemManager
{
    private InventoryItemManager PlayerInventoryManager = null;

    private void Awake()
    {
        GameObject[] FoundObjects = GameObject.FindGameObjectsWithTag("InventoryManager");

        if (FoundObjects.Length > 0)
        {            
            PlayerInventoryManager = FoundObjects[0].GetComponent<InventoryItemManager>();
        }
    }

    public void AttemptPurchaseSelectedItem()
    {
        if (PlayerInventoryManager != null && 
            CurrentlySelectedItemSlotController != null && 
            CurrentlySelectedItemSlotController.GetItem() != null && 
            PlayerInventoryManager.HasEnoughMoney(CurrentlySelectedItemSlotController.GetItem().Cost))
        {
            PlayerInventoryManager.RemoveMoney(CurrentlySelectedItemSlotController.GetItem().Cost);
            PlayerInventoryManager.AddItem(CurrentlySelectedItemSlotController.GetItem());
            RemoveItem(CurrentlySelectedItemSlotController);
        }
    }
}
