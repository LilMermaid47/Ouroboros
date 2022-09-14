using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Level/Level", fileName = "Level")]
public class Level : ScriptableObject
{
    public List<Quest> questList;
}
