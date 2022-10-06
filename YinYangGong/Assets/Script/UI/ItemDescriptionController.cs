using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemDescriptionController : MonoBehaviour
{
    public Image itemImage;
    public TMP_Text itemName;
    public TMP_Text itemPrice;
    public TMP_Text itemDescription;

    public void SetItemDescription(Sprite spriteItem, string nameOfItem, string priceOfItem, string descriptionOfItem)
    {
        itemImage.sprite = spriteItem;
        itemName.text = nameOfItem;
        itemPrice.text = priceOfItem;
        itemDescription.text = descriptionOfItem;
    }
}
