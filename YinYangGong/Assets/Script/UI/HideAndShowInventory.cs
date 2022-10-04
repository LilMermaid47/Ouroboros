using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HideAndShowInventory : MonoBehaviour
{
    Canvas inventoryCanvas;
    private void Start()
    {
        inventoryCanvas = GetComponent<Canvas>();
    }

    public void ShowInventory()
    {
        inventoryCanvas.enabled = true;
    }

    public void HideInventory()
    {
        inventoryCanvas.enabled = false;
    }
}
