using System;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    [Header("GameObject des differents UI.")]
    [SerializeField]
    private GameObject QuestUI;
    [SerializeField]
    private GameObject VictoryUI;
    [SerializeField]
    private GameObject DefeatUI;
    [SerializeField]
    private GameObject InGameMenuUI;
    [SerializeField]
    private GameObject GymMenu;

    [Header("Texte de defaite")]
    [SerializeField]
    private TMPro.TextMeshProUGUI DefeatTxt;

    [Header("Bouton selectionner par default")]
    [SerializeField]
    private FirstMenuBtn MenuBtn;

    [SerializeField]
    private TextRessources TextRessources;
    [SerializeField]
    private TextQuest TextQuest;
    [SerializeField]
    private BtnQuest BtnQuest;

    [Header("Texte affiche lors de la defaite")]
    [SerializeField]
    private LossText Loss;

    private Ouroboros PlayerInputs;

    private Quest CurrentQuest;

    public float timeTransitionAnimationBalance = 1f;
    public float timeTransitionAnimationMoneyAndReadiness = 1f;

    private void Awake()
    {
        QuestUI.SetActive(false);
        VictoryUI.SetActive(false);
        DefeatUI.SetActive(false);
        InGameMenuUI.SetActive(false);

        if(GymMenu != null)
            GymMenu.SetActive(false);
    }

    private void Start()
    {
        PlayerInputs = new Ouroboros();

        PlayerInputs.InGameMenu.Pause.performed += InGameMenuPressed;

        PauseMenuActive(true);
    }

    public void SetQuest(Quest quest)
    {
        CurrentQuest = quest;

        if (CurrentQuest != null)
        {
            TextQuest.NomQuest.text = CurrentQuest.questDefinition.questName;
            TextQuest.DescriptionQuest.text = $"{CurrentQuest.questDefinition.questDescription}\n\n\t{CurrentQuest.questDefinition.questGiverName}";

            BtnQuest.FirstChoiceTxt.text = CurrentQuest.questDefinition.choice1Name;
            BtnQuest.SecondChoiceTxt.text = CurrentQuest.questDefinition.choice2Name;
            ShowQuest(true);
            ActivateBtn();
        }
        else
            Debug.LogWarning("No quest receved.");
    }

    public void SetRessources(int argent, int yinYangBalance, float templeReadiness)
    {
        StartCoroutine(ChangeArgent(argent));
        StartCoroutine(ChangeBalance(yinYangBalance));
        StartCoroutine(ChangeReadiness(templeReadiness));
    }
    public void SetRessources(Clan premierClan, int premierClanDisciple, Clan secondClan, int secondClanDisciple)
    {
        ChangeDisciple(premierClan, premierClanDisciple);
        ChangeDisciple(secondClan, secondClanDisciple);
    }

    public void SetRessources(int argent, int yinYangBalance, float templeReadiness, Clan nom, int disciple)
    {
        SetRessources(argent, yinYangBalance, templeReadiness);
        ChangeDisciple(nom, disciple);
    }

    public void SetRessources(int argent, int yinYangBalance, float templeReadiness, Clan premierClan, int premierClanDisciple, Clan secondClan, int secondClanDisciple)
    {
        SetRessources(argent, yinYangBalance, templeReadiness);
        SetRessources(premierClan, premierClanDisciple, secondClan, secondClanDisciple);
    }

    public void SetArgent(int argent)
    {
        TextRessources.Yuan.text = $"{argent}¥";
    }

    int lastArgent = 0;
    private IEnumerator ChangeArgent(int argent)
    {
        float timerArgent = 0;

        float diff = (argent - lastArgent);

        while (timerArgent < timeTransitionAnimationMoneyAndReadiness)
        {
            yield return 0;
            timerArgent += Time.deltaTime;
            SetArgent((int)(lastArgent + (diff * timerArgent) / timeTransitionAnimationMoneyAndReadiness));
        }

        lastArgent = argent;
    }

    public void SetCurrentBalance(int currentBalance)
    {
        if (currentBalance > 0)
        {
            BalanceClan(Clan.Huangsei, 0);
            BalanceClan(Clan.Susoda, currentBalance);
        }
        else if (currentBalance < 0)
        {
            BalanceClan(Clan.Huangsei, -currentBalance);
            BalanceClan(Clan.Susoda, 0);
        }
        else
        {
            BalanceClan(Clan.Huangsei, 0);
            BalanceClan(Clan.Susoda, 0);
        }
    }

    int lastBalance = 0;
    private IEnumerator ChangeBalance(int newCurrentBalance)
    {
        float timerBalance = 0;

        float diff = (newCurrentBalance - lastBalance);

        while (timerBalance < timeTransitionAnimationMoneyAndReadiness)
        {
            yield return 0;
            timerBalance += Time.deltaTime;
            SetCurrentBalance((int)(lastBalance + (diff * timerBalance) / timeTransitionAnimationMoneyAndReadiness));
        }

        lastBalance = newCurrentBalance;
    }

    private void BalanceClan(Clan nom, int balance)
    {
        switch (nom)
        {
            case Clan.Huangsei:
                TextRessources.BalanceClanHuangsei.UpdateValue(balance);
                break;
            case Clan.Susoda:
                TextRessources.BalanceClanSusoda.UpdateValue(balance);
                break;
        }
    }

    public void IncreaseMaxBalance(int balanceIncrease)
    {
        TextRessources.BalanceClanHuangsei.UpdateMaxValue(balanceIncrease);
        TextRessources.BalanceClanSusoda.UpdateMaxValue(balanceIncrease);
    }
    public void SetMaxBalance(int balanceIncrease)
    {
        TextRessources.BalanceClanHuangsei.setValue(balanceIncrease);
        TextRessources.BalanceClanSusoda.setValue(balanceIncrease);
    }

    public void ChangeDisciple(Clan nom, int disciple)
    {
        switch (nom)
        {
            case Clan.Huangsei:
                TextRessources.DiscipleHuangsei.text = $"{disciple}";
                break;
            case Clan.Susoda:
                TextRessources.DiscipleSusoda.text = $"{disciple}";
                break;
        }
    }

    public void SetReadiness(float readiness)
    {
        TextRessources.TempleReadiness.text = $"{readiness}%";
    }

    float lastReadiness = 0;
    private IEnumerator ChangeReadiness(float newReadiness)
    {
        float timerReadiness = 0;

        float diff = (newReadiness - lastReadiness);

        while (timerReadiness < timeTransitionAnimationBalance)
        {
            yield return 0;
            timerReadiness += Time.deltaTime;
            SetReadiness((int)(lastReadiness + (diff * timerReadiness) / timeTransitionAnimationBalance));
        }

        lastReadiness = newReadiness;
    }

    public void SetNbQuestLeft(int nbQuest)
    {
        TextRessources.nbQuestLeft.text = $"Nombre d'audience restante: {nbQuest}";
    }

    public void RevealChoice1()
    {
        RevealChoice(CurrentQuest.questDefinition.choice1Reveal);
    }
    public void RevealChoice2()
    {
        RevealChoice(CurrentQuest.questDefinition.choice2Reveal);
    }

    private void RevealChoice(string txtChoice)
    {
        TextQuest.DescriptionQuest.text = txtChoice;
        ActivateNextPerson();
        ShowBtn(false);
    }

    private void ActivateBtn()
    {
        FirstChoiceBtnActivate(true);
        SecondChoiceBtnActivate(true);
        NextPersonBtnActivate(false);

        EventSystem.current.SetSelectedGameObject(MenuBtn.GameFirstChoice);
    }

    private void ActivateNextPerson()
    {
        FirstChoiceBtnActivate(false);
        SecondChoiceBtnActivate(false);
        NextPersonBtnActivate(true);

        EventSystem.current.SetSelectedGameObject(BtnQuest.NextPerson.gameObject);
    }

    public void FirstChoiceBtnActivate(bool status)
    {
        BtnQuest.FirstChoice.interactable = status;
    }

    public void SecondChoiceBtnActivate(bool status)
    {
        BtnQuest.SecondChoice.interactable = status;
    }

    private void NextPersonBtnActivate(bool status)
    {
        BtnQuest.NextPerson.interactable = status;
    }

    private void ShowBtn(bool status)
    {
        BtnQuest.FirstChoice.gameObject.SetActive(status);
        BtnQuest.SecondChoice.gameObject.SetActive(status);
        BtnQuest.NextPerson.gameObject.SetActive(!status);
    }

    private void ShowQuest(bool status)
    {
        QuestUI.SetActive(status);
        ShowBtn(status);
    }

    public void Victory()
    {
        QuestUI.SetActive(false);
        VictoryUI.SetActive(true);

        PauseMenuActive(false);

        EventSystem.current.SetSelectedGameObject(MenuBtn.VictoryFirstChoice);
    }

    public void Defeat()
    {

        QuestUI.SetActive(false);
        DefeatUI.SetActive(true);

        PauseMenuActive(false);

        EventSystem.current.SetSelectedGameObject(MenuBtn.DefeatFirstChoice);
    }

    public void Defeat(DefeatType defeatType)
    {
        string txt = "";

        switch (defeatType)
        {
            case DefeatType.HuangseiLoss:
                txt = Loss.HuangseiLoss;
                break;
            case DefeatType.SusodaLoss:
                txt = Loss.SusodaLoss;
                break;
            case DefeatType.ReadinessLoss:
                txt = Loss.ReadinessLoss;
                break;
            case DefeatType.MoneyLoss:
                txt = Loss.MoneyLoss;
                break;
        }

        DefeatTxt.text = txt;

        Defeat();
    }

    public void PauseMenuActive(bool status)
    {
        if (status && !PlayerInputs.InGameMenu.Pause.enabled)
            PlayerInputs.InGameMenu.Pause.Enable();
        else if (PlayerInputs.InGameMenu.Pause.enabled)
            PlayerInputs.InGameMenu.Pause.Disable();

    }

    private void InGameMenuPressed(InputAction.CallbackContext obj)
    {
        OpenInGameMenu();
    }

    public void OpenInGameMenu()
    {
        bool menuOpened = InGameMenuUI.activeSelf;

        InGameMenuUI.SetActive(!menuOpened);

        ShowBtn(menuOpened);

        if (!menuOpened)
        {
            Time.timeScale = 0f;

            EventSystem.current.SetSelectedGameObject(MenuBtn.InGameMenuFirstChoice);
        }
        else
        {
            Time.timeScale = 1f;

            EventSystem.current.SetSelectedGameObject(MenuBtn.GameFirstChoice);
        }
    }

    public void OpenGymMenu()
    {
        GymMenu.SetActive(true);
    }

    private void OnDestroy()
    {
        PlayerInputs.Dispose();
    }
}

[Serializable]
public class TextRessources
{
    public TMPro.TextMeshProUGUI DiscipleSusoda;
    public TMPro.TextMeshProUGUI DiscipleHuangsei;
    public TMPro.TextMeshProUGUI Yuan;
    public TMPro.TextMeshProUGUI TempleReadiness;
    public TMPro.TextMeshProUGUI nbQuestLeft;
    public BalanceBar BalanceClanSusoda;
    public BalanceBar BalanceClanHuangsei;
}

[Serializable]
public class TextQuest
{
    public TMPro.TextMeshProUGUI NomQuest;
    public TMPro.TextMeshProUGUI DescriptionQuest;
}

[Serializable]
public class BtnQuest
{
    public Button FirstChoice;
    public Button SecondChoice;
    public Button NextPerson;

    public TMPro.TextMeshProUGUI FirstChoiceTxt;
    public TMPro.TextMeshProUGUI SecondChoiceTxt;
}

[Serializable]
public class FirstMenuBtn
{
    public GameObject GameFirstChoice;
    public GameObject InGameMenuFirstChoice;
    public GameObject VictoryFirstChoice;
    public GameObject DefeatFirstChoice;
}

[Serializable]
public class LossText
{
    public string HuangseiLoss;
    public string SusodaLoss;
    public string ReadinessLoss;
    public string MoneyLoss;
}

public enum Clan
{
    Huangsei,
    Susoda
}

public enum DefeatType
{
    HuangseiLoss,
    SusodaLoss,
    ReadinessLoss,
    MoneyLoss
}