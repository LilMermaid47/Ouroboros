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

    public void onChoiceWasMade()
    {
        if (choiceSlider.value <= -97)
        {
            uiController.RevealChoice1();
            questManager.UpdateStatChoix1();
            choiceSlider.value = 0;
            choiceSlider.gameObject.SetActive(false);
        }
        else if (choiceSlider.value >= 97)
        {
            uiController.RevealChoice2();
            questManager.UpdateStatChoix2();
            choiceSlider.value = 0;
            choiceSlider.gameObject.SetActive(false);
        }
        else if(choiceSlider.value <= -10 && showTooltip)
        {
            hoverChoiceGauche.ShowMessage();
            hoverChoiceGauche.ShowImpactOfChoice();
        }
        else if(choiceSlider.value >= 10 && showTooltip)
        {
            hoverChoiceDroit.ShowMessage();
            hoverChoiceDroit.ShowImpactOfChoice();
        }
        else
        {
            hoverChoiceGauche.OnPointerExit();
            hoverChoiceGauche.TurnLampOff();
        }
    }

    public void SliderToZero()
    {
        choiceSlider.value = 0;
    }

    public void EnableSlider()
    {
        choiceSlider.gameObject.SetActive(true);
    }
}
