using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using System;

public class QuestChoiceUi : MonoBehaviour
{
    private Ouroboros PlayerInputs;


    public QuestManager questManager;
    public UIController uiController;

    public GameObject questChoiceUiParent;
    public GameObject questUi;
    public TMP_Text description;
    public Button acceptChoice;
    public Button refuseChoice;

    private bool leftGongChoice;

    public void ShowChoice(bool choice1, string descriptionString)
    {
        ActivateListener(true);
        leftGongChoice = choice1;
        uiController.MakeButtonInvisible(false);
        questUi.SetActive(false);
        questChoiceUiParent.SetActive(true);
        description.text = descriptionString;
    }

    public void AcceptChoice()
    {
        if (leftGongChoice)
            questManager.UpdateStatChoix1();
        else
            questManager.UpdateStatChoix2();

        questChoiceUiParent.SetActive(false);
        ActivateListener(false);
    }

    public void CancelChoice()
    {
        uiController.ShowBtn(true);
        questUi.SetActive(true);
        questChoiceUiParent.SetActive(false);
        ActivateListener(false);
    }

    private void ActivateListener(bool isActive)
    {
        if (isActive)
        {
            PlayerInputs.QuestChoice.Accepter.Enable();
            PlayerInputs.QuestChoice.GoBack.Enable();

        }
        else
        {
            PlayerInputs.QuestChoice.Accepter.Disable();
            PlayerInputs.QuestChoice.GoBack.Disable();
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        PlayerInputs = new Ouroboros();
        PlayerInputs.QuestChoice.Accepter.performed += AccepterChoice;
        PlayerInputs.QuestChoice.GoBack.performed += RefuserChoice;
    }

    private void RefuserChoice(InputAction.CallbackContext obj)
    {
        CancelChoice();
    }

    private void AccepterChoice(InputAction.CallbackContext obj)
    {
        AcceptChoice();
    }
    private void OnDestroy()
    {
        PlayerInputs.Dispose();
    }
}
