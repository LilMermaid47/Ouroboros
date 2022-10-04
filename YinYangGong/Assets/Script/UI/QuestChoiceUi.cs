using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class QuestChoiceUi : MonoBehaviour
{
    public QuestManager questManager;
    public UIController uiController;

    public GameObject questChoiceUiParent;
    public GameObject questUi;
    public TMP_Text description;
    public Button acceptChoice;
    public Button refuseChoice;

    private bool leftGongChoice;

    public void ShowChoice(bool choice1,string descriptionString)
    {
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
    }

    public void CancelChoice()
    {
        uiController.ShowBtn(true);
        questUi.SetActive(true);
        questChoiceUiParent.SetActive(false);
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
