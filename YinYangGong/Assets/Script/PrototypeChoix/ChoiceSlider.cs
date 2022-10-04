using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChoiceSlider : MonoBehaviour
{
    public bool showTooltip = true;

    public QuestManager questManager;
    public UIController uiController;
    public Slider choiceSlider;
    public HoverChoice hoverChoiceGauche;
    public HoverChoice hoverChoiceDroit;

    public void SliderToZero()
    {
        choiceSlider.value = 0;
    }

    public void EnableSlider()
    {
        choiceSlider.gameObject.SetActive(true);
    }
}
