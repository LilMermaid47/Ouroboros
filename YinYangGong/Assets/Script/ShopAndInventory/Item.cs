using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Item : ScriptableObject
{
    [Header("Item Info")]
    public string Name;
    public string Description;
    public Sprite Icon;
    public int Cost = 0;
    public bool bIsStackable = false;
}
