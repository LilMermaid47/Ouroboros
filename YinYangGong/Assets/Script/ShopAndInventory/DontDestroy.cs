using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DontDestroy : MonoBehaviour
{
    public String TagToCheck;
    private void Awake()
    {
        GameObject[] FoundObjects = GameObject.FindGameObjectsWithTag(TagToCheck);

        if (FoundObjects.Length > 1)
        {
            Destroy(this.gameObject);
        }

        DontDestroyOnLoad(this.gameObject);
    }
}
