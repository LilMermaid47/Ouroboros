using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemManager : MonoBehaviour
{
    [SerializeField]
    private GameObject ItemSlotContainer = null;
    [SerializeField]
    private GameObject ItemSlotPrefab = null;
    [SerializeField]
    private Color SelectedColor = Color.clear;
    [SerializeField]
    private Color UnselectedColor = Color.clear;

    private UIController uiController;


    [SerializeField]
    protected int NumberOfAvailableItemSlots = 10;

    [SerializeField]
    protected List<ItemSlotInfo> StartingItems = new List<ItemSlotInfo>();

    protected ItemSlotController[] ItemSlotControllers;

    protected ItemSlotController CurrentlySelectedItemSlotController = null;

    private void Start()
    {
        uiController = GameObject.FindGameObjectWithTag("Canvas").GetComponent<UIController>();
        ItemSlotControllers = new ItemSlotController[NumberOfAvailableItemSlots];

        for (int i = 0; i < NumberOfAvailableItemSlots; ++i)
        {
            GameObject NewItemSlot = Instantiate(ItemSlotPrefab) as GameObject;
            NewItemSlot.transform.SetParent(ItemSlotContainer.transform);

            ItemSlotController NewItemSlotController = NewItemSlot.transform.GetComponent<ItemSlotController>();
            NewItemSlotController.SelectionButtonImage.color = UnselectedColor;
            NewItemSlotController.SelectionButton.onClick.AddListener(delegate { HandleItemSlotControllerSelected(NewItemSlotController); });

            ItemSlotControllers[i] = NewItemSlotController;
        }

        for (int i = 0; i < StartingItems.Count; ++i)
        {
            if (i < NumberOfAvailableItemSlots)
            {
                ItemSlotControllers[i].SetItemDetails(StartingItems[i].Item, StartingItems[i].Quantity);
            }
        }

        HandleItemSlotControllerSelected(ItemSlotControllers[0]);
    }

    public bool HasItem(Item ItemToFind)
    {
        return FindSlotContainingItem(ItemToFind) != null;
    }

    public void AddItem(Item Item)
    {
        ItemSlotController ExistingSlotControllerWithItem = FindSlotContainingItem(Item);

        if (ExistingSlotControllerWithItem != null && ExistingSlotControllerWithItem.GetItem().bIsStackable)
        {
            ExistingSlotControllerWithItem.AddQuantity(1);
        }
        else
        {
            foreach (ItemSlotController ItemSlotController in ItemSlotControllers)
            {
                if (ItemSlotController.GetItem() == null)
                {
                    ItemSlotController.SetItemDetails(Item, 1);
                    break;
                }
            }            
        }
    }

    public void RemoveItem(Item Item)
    {
        ItemSlotController ExistingSlotControllerWithItem = FindSlotContainingItem(Item);

        if (ExistingSlotControllerWithItem != null)
        {
            if (ExistingSlotControllerWithItem.GetQuantity() > 1)
            {
                ExistingSlotControllerWithItem.RemoveQuantity(1);
            }
            else
            {
                ExistingSlotControllerWithItem.ClearItemDetails();
            }
        }
    }
    
    public void UseItem()
    {
        Item currentItem = CurrentlySelectedItemSlotController.GetItem();

        if (currentItem != null)
        {
            uiController.ShowHideShop();
            currentItem.UseItem();
            RemoveItem(currentItem);
        }
    }

    public void UseItem(Item item)
    {
        item.UseItem();
        RemoveItem(item);
    }

    protected void RemoveItem(ItemSlotController SlotControllerContainingItem)
    {
        if (SlotControllerContainingItem.GetQuantity() > 1)
        {
            SlotControllerContainingItem.RemoveQuantity(1);
        }
        else
        {
            SlotControllerContainingItem.ClearItemDetails();
        }
    }

    protected ItemSlotController FindSlotContainingItem(Item ItemToSearchFor)
    {
        foreach(ItemSlotController ItemSlotController in ItemSlotControllers)
        {
            if (ItemSlotController.GetItem() == ItemToSearchFor)
            {
                return ItemSlotController;
            }
        }

        return null;
    }

    public virtual void HandleItemSlotControllerSelected(ItemSlotController NewSelectedItemController)
    {
        if (CurrentlySelectedItemSlotController != null)
        {
            CurrentlySelectedItemSlotController.SelectionButtonImage.color = UnselectedColor;
        }        
        CurrentlySelectedItemSlotController = NewSelectedItemController;
        CurrentlySelectedItemSlotController.SelectionButtonImage.color = SelectedColor;


    }
}
