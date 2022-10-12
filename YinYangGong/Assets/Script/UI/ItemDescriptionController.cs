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

    public void SetItemDescription(Item item)
    {
        if(item != null)
        {
            itemImage.sprite = item.Icon;
            itemName.text = item.Name;
            itemPrice.text = $"{item.Cost}¥"; 
            itemDescription.text = item.Description;
        }
        else
        {
            itemName.text = "";
            itemPrice.text = "";
            itemDescription.text = "";
        }
    }
}
