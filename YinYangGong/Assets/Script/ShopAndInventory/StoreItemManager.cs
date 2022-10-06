using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoreItemManager : ItemManager
{
    private InventoryItemManager PlayerInventoryManager = null;

    [SerializeField]
    private ItemDescriptionController itemDescriptionController;

    private void Awake()
    {
        GameObject[] FoundObjects = GameObject.FindGameObjectsWithTag("InventoryManager");

        if (FoundObjects.Length > 0)
        {            
            PlayerInventoryManager = FoundObjects[0].GetComponent<InventoryItemManager>();
        }
        questManager = GameObject.FindGameObjectWithTag("QuestManager").GetComponent<QuestManager>();
    }

    private QuestManager questManager;

    public void AttemptPurchaseSelectedItem()
    {
        if (PlayerInventoryManager != null && 
            CurrentlySelectedItemSlotController != null && 
            CurrentlySelectedItemSlotController.GetItem() != null && 
            questManager.HasEnoughMoney(CurrentlySelectedItemSlotController.GetItem().Cost))
        {
            questManager.RemoveMoney(CurrentlySelectedItemSlotController.GetItem().Cost);
            PlayerInventoryManager.AddItem(CurrentlySelectedItemSlotController.GetItem());
            RemoveItem(CurrentlySelectedItemSlotController);
        }
    }

    public override void HandleItemSlotControllerSelected(ItemSlotController NewSelectedItemController)
    {
        base.HandleItemSlotControllerSelected(NewSelectedItemController);
        itemDescriptionController.SetItemDescription(NewSelectedItemController.GetItem());
    }
}
