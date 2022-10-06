using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

[RequireComponent(typeof(SoundManager))]
public class UIController : MonoBehaviour
{
    [Header("GameObject des differents UI.")]
    [SerializeField]
    private GameUI UIMenu;

    [SerializeField]
    private GameObject NPCQueue;

    [SerializeField]
    private RectTransform ScrollBackground;


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
    [SerializeField]
    private QuestChoiceUi questChoiceUi;

    [Header("Texte affiche lors de la defaite")]
    [SerializeField]
    private LossText Loss;

    private Ouroboros PlayerInputs;

    private Quest CurrentQuest;
    private QuestManager questManager;
    private SoundManager soundManager;

    public float timeTransitionAnimation = 0.1f;
    public float timeTransitionAnimationBalance = 1f;
    public float timeTransitionAnimationMoney = 0.1f;
    public float timeTransitionAnimationReadiness = 0.5f;

    private Image[] NPCFileImage;

    private void Awake()
    {
        soundManager = gameObject.GetComponent<SoundManager>();

        UIMenu.QuestUI.SetActive(false);
        UIMenu.VictoryUI.SetActive(false);
        UIMenu.DefeatUI.SetActive(false);
        UIMenu.InGameMenuUI.SetActive(false);

        if (UIMenu.GymMenu != null)
            UIMenu.GymMenu.SetActive(false);
    }

    private void Start()
    {
        PlayerInputs = new Ouroboros();

        PlayerInputs.InGameMenu.Pause.performed += InGameMenuPressed;
        PlayerInputs.QuestChoice.Accepter.performed += AccepterChoice;
        questManager = GameObject.FindGameObjectWithTag("QuestManager").GetComponent<QuestManager>();
        PauseMenuActive(true);
        HideBtn();

        if(NPCQueue != null)
            NPCFileImage = NPCQueue.GetComponentsInChildren<Image>();
    }

    private void AccepterChoice(InputAction.CallbackContext obj)
    {
        questManager.NextQuest();
    }

    public void SetQuest(Quest quest)
    {
        CurrentQuest = quest;

        if (CurrentQuest != null)
        {
            TextQuest.QuestInformation.SetActive(false);

            TextQuest.NomQuest.text = CurrentQuest.questDefinition.questName;
            TextQuest.DescriptionQuest.text = $"{CurrentQuest.questDefinition.questDescription}";
            TextQuest.QuestGiver.text = $"{CurrentQuest.questDefinition.questGiverName}";

            BtnQuest.FirstChoiceTxt.text = CurrentQuest.questDefinition.choice1Name;
            BtnQuest.SecondChoiceTxt.text = CurrentQuest.questDefinition.choice2Name;

            HideBtn();

            OpeningScroll();

            ShowQuest(true);
            ActivateBtn();
        }
        else
            Debug.LogWarning("No quest receved.");
    }

    bool nextPersonWasVisible = false;

    public void ShowHideShop()
    {
        UIMenu.ShopUi.SetActive(!UIMenu.ShopUi.activeSelf);
        UIMenu.InventoryUi.SetActive(!UIMenu.InventoryUi.activeSelf);
        MakeButtonInvisibleExceptNextPerson(!UIMenu.ShopUi.activeSelf);
        UIMenu.QuestUI.SetActive(!UIMenu.QuestUI.activeSelf);
    }
    public void SetStartingRessources(int argent, int yinYangBalance, float templeReadiness)
    {
        SetArgent(argent);
        SetCurrentBalance(yinYangBalance);
        SetReadiness(templeReadiness);
        lastArgent = argent;
        lastBalance = yinYangBalance;
        lastReadiness = templeReadiness;
    }
    public void SetStartingRessources(int argent, int yinYangBalance, float templeReadiness, int disciple, int ki)
    {
        SetStartingRessources(argent, yinYangBalance, templeReadiness);
        SetDisciple(disciple);
        SetKi(ki);
        lastDisciple = disciple;
        lastKi = ki;
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

    private void MakeAllButtonInvisible(bool status)
    {
        BtnQuest.ButtonObject.SetActive(status);
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

        if (diff != 0)
        {
            if (diff > 0)
            {
                soundManager.MoneyIsGoingUpFor(timeForTransition);
            }

            while (timerArgent < timeForTransition)
            {
                await Task.Yield();
                timerArgent += Time.deltaTime;
                SetArgent((int)(lastArgent + (diff * timerArgent) / timeForTransition));
            }
        }
        SetArgent(argent);

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
        TextRessources.Disciple.text = $"{disciple}";
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
        TextRessources.KiBar.UpdateValue(ki);
    }

    public void SetMaxKi(int maxKi)
    {
        if (maxKi == 0)
            TextRessources.KiBar.HideBar();

        TextRessources.KiBar.setValue(maxKi);
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
        TextRessources.nbQuestLeft.text = $"Audience left: {nbQuest}";
    }

    public void SetNPCFile(Level niveau)
    {
        int NPCLeft = NPCFileImage.Length;

        for (int i = 0; NPCLeft >= 0 && i < NPCFileImage.Length; i++)
        {
            NPCLeft--;
            NPCFileImage[NPCLeft].sprite = niveau.questList[i].questDefinition.questGiverSprite;
        }
    }

    public void UpdateNPCList(Level niveau, int questIndex)
    {
        Sprite questGiverSprite;

        for (int i = NPCFileImage.Length - 1; i > 0; i--)
        {
            if (NPCFileImage[i - 1].sprite != null)
            {
                HideSprite(NPCFileImage[i], false);
                NPCFileImage[i].sprite = NPCFileImage[i - 1].sprite;
            }
            else
                HideSprite(NPCFileImage[i], true);
        }

        if (questIndex + 2 < niveau.questList.Count)
        {
            questGiverSprite = niveau.questList[questIndex + 2].questDefinition.questGiverSprite;

            if (questGiverSprite != null)
            {
                HideSprite(NPCFileImage[0], false);
                NPCFileImage[0].sprite = questGiverSprite;
            }
            else
                HideSprite(NPCFileImage[0], true);

        }
        else
            HideSprite(NPCFileImage[0], true);
    }

    private void HideSprite(Image image, bool status)
    {
        if (status)
        {
            image.enabled = false;
            image.sprite = null;
        }
        else
            image.enabled = true;
    }

    public void RevealChoice1()
    {
        RevealChoice(true, CurrentQuest.questDefinition.choice1Description);
    }
    public void RevealChoice2()
    {
        RevealChoice(false, CurrentQuest.questDefinition.choice2Description);
    }

    private void RevealChoice(bool isChoice1, string txtChoice)
    {
        questChoiceUi.ShowChoice(isChoice1, txtChoice);
    }

    private void ActivateBtn()
    {
        FirstChoiceBtnActivate(true);
        SecondChoiceBtnActivate(true);
        NextPersonBtnActivate(false);

        EventSystem.current.SetSelectedGameObject(MenuBtn.GameFirstChoice);
        EventSystem.current.firstSelectedGameObject = MenuBtn.GameFirstChoice;
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
        EventSystem.current.firstSelectedGameObject = BtnQuest.NextPerson.gameObject;
    }

    public void FirstChoiceBtnActivate(bool status)
    {
        BtnQuest.FirstChoice.interactable = status;
    }

    public void DisableFirstChoiceBtn(string reason)
    {
        FirstChoiceBtnActivate(false);
        BtnQuest.FirstChoiceTxt.text += $"\n{reason}";
    }

    public void SecondChoiceBtnActivate(bool status)
    {
        BtnQuest.SecondChoice.interactable = status;
    }

    public void DisableSecondChoiceBtn(string reason)
    {
        SecondChoiceBtnActivate(false);
        BtnQuest.SecondChoiceTxt.text += $"\n{reason}";
    }

    private void NextPersonBtnActivate(bool status)
    {
        BtnQuest.NextPerson.interactable = status;
    }

    public void MakeButtonInvisible(bool status)
    {
        BtnQuest.FirstChoice.gameObject.SetActive(status);
        BtnQuest.SecondChoice.gameObject.SetActive(status);
        BtnQuest.NextPerson.gameObject.SetActive(status);
    }

    private void MakeButtonInvisibleExceptNextPerson(bool status)
    {
        BtnQuest.FirstChoice.gameObject.SetActive(status);
        BtnQuest.SecondChoice.gameObject.SetActive(status);
    }

    public void ShowBtn(bool status)
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

    private async void OpeningScroll()
    {
        Vector2 normalSize = ScrollBackground.sizeDelta;
        int sizeIncrease = 50;
        int openingScrollDelay = 60;

        ScrollBackground.sizeDelta = new Vector2(0,normalSize.y);

        for (int i = 0; i < normalSize.x; i += sizeIncrease)
        {
            ScrollBackground.sizeDelta = new Vector2(i, normalSize.y);
            await Task.Delay(openingScrollDelay);
        }

        ScrollBackground.sizeDelta = normalSize;

        TextQuest.QuestInformation.SetActive(true);
        ShowBtn(true);
    }

    private void ShowQuest(bool status)
    {
        UIMenu.QuestUI.SetActive(status);
        //ShowBtn(status);
    }

    public void Victory()
    {
        UIMenu.QuestUI.SetActive(false);
        UIMenu.VictoryUI.SetActive(true);

        PauseMenuActive(false);
        MakeAllButtonInvisible(false);

        EventSystem.current.SetSelectedGameObject(MenuBtn.VictoryFirstChoice);
    }

    public void Defeat()
    {

        UIMenu.QuestUI.SetActive(false);
        UIMenu.DefeatUI.SetActive(true);
        MakeAllButtonInvisible(false);
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
            case DefeatType.DiscipleLoss:
                txt = Loss.DiscipleLoss;
                break;
        }

        DefeatTxt.text = txt;

        Defeat();
    }

    public void EnableShop(bool status)
    {
        BtnQuest.ShopButton.gameObject.SetActive(status);
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
        bool menuOpened = UIMenu.InGameMenuUI.activeSelf;

        UIMenu.InGameMenuUI.SetActive(!menuOpened);

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
        if (UIMenu.GymMenu != null)
            UIMenu.GymMenu.SetActive(true);
    }

    private void OnDestroy()
    {
        PlayerInputs.Dispose();
    }
}

[Serializable]
public class GameUI
{
    public GameObject QuestUI;
    public GameObject VictoryUI;
    public GameObject DefeatUI;
    public GameObject InGameMenuUI;
    public GameObject GymMenu;
    public GameObject ShopUi;
    public GameObject InventoryUi;
}


[Serializable]
public class TextRessources
{
    public TextMeshProUGUI Disciple;
    public TextMeshProUGUI Yuan;
    public TextMeshProUGUI TempleReadiness;
    public TextMeshProUGUI nbQuestLeft;
    public BalanceBar BalanceClanSusoda;
    public BalanceBar BalanceClanHuangsei;
    public BalanceBar KiBar;
}

[Serializable]
public class TextQuest
{
    public TextMeshProUGUI NomQuest;
    public TextMeshProUGUI DescriptionQuest;
    public TextMeshProUGUI QuestGiver;

    public GameObject QuestInformation;
}

[Serializable]
public class BtnQuest
{
    public GameObject ButtonObject;
    public Button FirstChoice;
    public Button SecondChoice;
    public Button NextPerson;
    public Button ShopButton;

    public TextMeshProUGUI FirstChoiceTxt;
    public TextMeshProUGUI SecondChoiceTxt;
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
    public string DiscipleLoss;
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
    DiscipleLoss,
    KiLoss
}