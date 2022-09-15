using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Level/Random Quest List", fileName = "RandomQuestList")]
public class RandomQuestList : ScriptableObject
{
    public List<Quest> randomQuestList;
}
