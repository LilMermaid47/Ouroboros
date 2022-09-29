using UnityEngine;

[CreateAssetMenu(menuName = "Blindness/BlindnessType", fileName = "BlindnessType")]
public class BlindnessTypes : ScriptableObject
{
    public string Name;

    [Header("Couleurs Huangsei")]
    public Color HuangseiPrimary = new Color(255f, 192f, 0f, 255f);
    public Color HuangseiSecondary = new Color(142f, 69f, 23f, 255f);

    [Header("Couleurs Susoda")]
    public Color SusodaPrimary = new Color(149f, 0f, 179f, 255f);
    public Color SusodaSecondary = new Color(242f, 199f, 245f, 255f);
}
