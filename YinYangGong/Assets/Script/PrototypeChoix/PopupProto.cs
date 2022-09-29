using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PopupProto : MonoBehaviour
{
    public HoverChoice hoverChoice;

    Button button;

    private void Start()
    {
        button = GetComponent<Button>();
    }

    private bool isMessageShown = false;
    public void ButtonWasPressed()
    {
        if (isMessageShown)
        {
            hoverChoice.ChoiceWasMade();
            hoverChoice.TurnLampOff();
            button.interactable = false;
            button.interactable = true;
        }
        else
        {
            hoverChoice.ShowImpactOfChoice();
            hoverChoice.ShowMessage();
        }
        isMessageShown = !isMessageShown;
    }
}
