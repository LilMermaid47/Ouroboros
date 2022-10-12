using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.GraphicsBuffer;

public class ItemDescriptionController : MonoBehaviour
{
    public Image itemImage;
    public TMP_Text itemName;
    public TMP_Text itemPrice;
    public TMP_Text itemDescription;

    private Item currentItem;
    private QuestManager questManager;
    private InventoryItemManager PlayerInventoryManager;

    private void Start()
    {
        questManager = GameObject.FindGameObjectWithTag("QuestManager").GetComponent<QuestManager>();
        PlayerInventoryManager = GameObject.FindGameObjectWithTag("InventoryManager").GetComponent<InventoryItemManager>();

    }

    public void SetItemDescription(Item item)
    {
        if(item != null)
        {
            currentItem = item;
            itemImage.sprite = item.Icon;
            itemName.text = item.Name;
            itemPrice.text = $"{item.Cost}¥"; 
            itemDescription.text = item.Description;
        }
        else
        {
            currentItem = null;
            itemName.text = "";
            itemPrice.text = "";
            itemDescription.text = "";
        }
    }

    public void AttemptPurchaseSelectedItem()
    {
        questManager.RemoveMoney(currentItem.Cost);
        PlayerInventoryManager.AddItem(currentItem);
    }
}
