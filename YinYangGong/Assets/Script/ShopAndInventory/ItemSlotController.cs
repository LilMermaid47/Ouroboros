using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemSlotController : MonoBehaviour
{
    [SerializeField]
    private Item Item = null;
    [SerializeField]
    private int Quantity = 0;

    [SerializeField]
    public Button SelectionButton = null;
    [SerializeField]
    public Image SelectionButtonImage = null;
    [SerializeField]
    Image Icon = null;
    [SerializeField]
    TMPro.TextMeshProUGUI QuantityText = null;

    private void Awake()
    {
        if (Item == null)
        {
            ClearItemDetails();
        }
        else
        {
            SetItemDetails(Item, Quantity);
        }
    }

    public Item GetItem() { return Item; }

    public int GetQuantity() { return Quantity; }

    public void AddQuantity(int QuantityToAdd)
    {
        Quantity += QuantityToAdd;
        QuantityText.text = Quantity.ToString();
    }

    public void RemoveQuantity(int QuantityToRemove)
    {
        if (Quantity > 0)
        {
            Quantity -= QuantityToRemove;
            QuantityText.text = Quantity.ToString();
        }
    }

    public void SetItemDetails(Item NewItem, int NewQuantity)
    {
        Item = NewItem;
        Quantity = NewQuantity;

        Icon.sprite = Item.Icon;
        Icon.enabled = true;
        QuantityText.text = Quantity.ToString();
    }

    public void ClearItemDetails()
    {
        Item = null;
        Quantity = 0;

        Icon.sprite = null;
        Icon.enabled = false;
        QuantityText.text = "";
    }
}
