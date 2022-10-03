using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

[RequireComponent(typeof(SoundManager))]
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

    private SoundManager soundManager;

    public float timeTransitionAnimation = 0.1f;
    public float timeTransitionAnimationBalance = 1f;
    public float timeTransitionAnimationMoney = 0.1f;
    public float timeTransitionAnimationReadiness = 0.5f;

    private void Awake()
    {
        soundManager = gameObject.GetComponent<SoundManager>();

        QuestUI.SetActive(false);
        VictoryUI.SetActive(false);
        DefeatUI.SetActive(false);
        InGameMenuUI.SetActive(false);

        if (GymMenu != null)
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
            TextQuest.DescriptionQuest.text = $"{CurrentQuest.questDefinition.questDescription}";
            TextQuest.QuestGiver.text = $"{CurrentQuest.questDefinition.questGiverName}";

            BtnQuest.FirstChoiceTxt.text = CurrentQuest.questDefinition.choice1Name;
            BtnQuest.SecondChoiceTxt.text = CurrentQuest.questDefinition.choice2Name;
            ShowQuest(true);
            ActivateBtn();
        }
        else
            Debug.LogWarning("No quest receved.");
    }

    public void SetStartingRessources(int argent, int yinYangBalance, float templeReadiness)
    {
        SetArgent(argent);
        SetCurrentBalance(yinYangBalance);
        SetReadiness(templeReadiness);

    }
    public void SetStartingRessources(int argent, int yinYangBalance, float templeReadiness, int disciple, int ki)
    {
        SetStartingRessources(argent, yinYangBalance, templeReadiness);
        SetDisciple(disciple);
        SetKi(ki);
    }

    public async void SetRewardsRessources(int argent, int yinYangBalance, float templeReadiness, int disciple, int ki)
    {
        List<Task> taskList = new List<Task>();

        HideBtn();

        taskList.Add(ChangeArgent(argent));
        taskList.Add(ChangeBalance(yinYangBalance));
        taskList.Add(ChangeReadiness(templeReadiness));
        taskList.Add(ChangeDisciple(disciple));
        taskList.Add(ChangeKi(ki));

        await Task.WhenAll(taskList);
        ActivateNextBtn();
    }

    public void SetArgent(int argent)
    {
        TextRessources.Yuan.text = $"{argent}¥";
    }

    int lastArgent = 0;
    private async Task ChangeArgent(int argent)
    {
        float timerArgent = 0;

        float diff = (argent - lastArgent);

        float timeForTransition = Mathf.Abs(diff * timeTransitionAnimationMoney);

        if (diff == 0)
        {
            SetArgent(argent);
        }
        else
        {
            if (diff > 0)
            {
                soundManager.MoneyIsGoingUpFor(timeForTransition);
            }

            while (timerArgent < timeForTransition+1)
            {
                await Task.Yield();
                timerArgent += Time.deltaTime;
                SetArgent((int)(lastArgent + (diff * timerArgent) / timeForTransition));
            }
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
    private async Task ChangeBalance(int newCurrentBalance)
    {
        float timerBalance = 0;

        float diff = (newCurrentBalance - lastBalance);
        float timeForTransition = Mathf.Abs(diff * timeTransitionAnimationBalance);

        if (diff == 0)
            SetCurrentBalance(newCurrentBalance);
        else
        {
            while (timerBalance < timeForTransition)
            {
                await Task.Yield();
                timerBalance += Time.deltaTime;
                SetCurrentBalance((int)(lastBalance + (diff * timerBalance) / timeForTransition));
            }
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

    public void SetDisciple(int disciple)
    {
        TextRessources.Disciple.text = $"{disciple} disciple";
    }

    int lastDisciple = 0;
    private async Task ChangeDisciple(int newDisciple)
    {
        float timerDisciple = 0;

        float diff = (newDisciple - lastDisciple);

        float timeForTransition = Mathf.Abs(diff * timeTransitionAnimation);

        if (diff != 0)
            while (timerDisciple < timeForTransition)
            {
                await Task.Yield();
                timerDisciple += Time.deltaTime;
                SetDisciple((int)(lastDisciple + (diff * timerDisciple) / timeForTransition));
            }

        SetDisciple(newDisciple);
        lastDisciple = newDisciple;
    }

    public void SetKi(int ki)
    {
        TextRessources.Ki.text = $"{ki} ki";
    }

    int lastKi = 0;
    private async Task ChangeKi(int newKi)
    {
        float timerKi = 0;

        float diff = (newKi - lastKi);

        float timeForTransition = Mathf.Abs(diff * timeTransitionAnimation);

        if (diff == 0)
            SetKi(newKi);
        while (timerKi < timeForTransition)
        {
            await Task.Yield();
            timerKi += Time.deltaTime;
            SetKi((int)(lastKi + (diff * timerKi) / timeForTransition));
        }

        lastKi = newKi;
    }

    public void SetReadiness(float readiness)
    {
        TextRessources.TempleReadiness.text = $"{readiness} / 100";
    }

    float lastReadiness = 0;
    private async Task ChangeReadiness(float newReadiness)
    {
        float timerReadiness = 0;

        float diff = (newReadiness - lastReadiness);

        float timeForTransition = Mathf.Abs(diff * timeTransitionAnimationReadiness);

        if (diff == 0)
            SetReadiness(newReadiness);
        while (timerReadiness < timeForTransition)
        {
            await Task.Yield();
            timerReadiness += Time.deltaTime;
            SetReadiness((int)(lastReadiness + (diff * timerReadiness) / timeForTransition));
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
        TextQuest.QuestGiver.text = "";
    }

    private void ActivateBtn()
    {
        FirstChoiceBtnActivate(true);
        SecondChoiceBtnActivate(true);
        NextPersonBtnActivate(false);

        EventSystem.current.SetSelectedGameObject(MenuBtn.GameFirstChoice);
    }

    private void ActivateNextBtn()
    {
        ActivateNextPerson();
        ShowBtn(false);
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

    private void HideBtn()
    {
        BtnQuest.FirstChoice.gameObject.SetActive(false);
        BtnQuest.SecondChoice.gameObject.SetActive(false);
        BtnQuest.NextPerson.gameObject.SetActive(false);
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
            EventSystem.current.SetSelectedGameObject(MenuBtn.InGameMenuFirstChoice);
        }
        else
        {
            EventSystem.current.SetSelectedGameObject(MenuBtn.GameFirstChoice);
        }
    }

    public void OpenGymMenu()
    {
        if (GymMenu != null)
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
    public TMPro.TextMeshProUGUI Disciple;
    public TMPro.TextMeshProUGUI Ki;
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
    public TMPro.TextMeshProUGUI QuestGiver;
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
    MoneyLoss,
    KiLoss
}