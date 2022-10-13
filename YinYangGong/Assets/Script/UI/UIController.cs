using System;
using System.Collections;
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
    protected GameUI UIMenu;

    [SerializeField]
    protected GameObject NPCQueue;

    [SerializeField]
    protected RectTransform ScrollBackground;


    [Header("Texte de defaite")]
    [SerializeField]
    protected TextMeshProUGUI DefeatTxt;

    [Header("Bouton selectionner par default")]
    [SerializeField]
    protected FirstMenuBtn MenuBtn;

    [SerializeField]
    protected TextRessources TextRessources;
    [SerializeField]
    protected TextQuest TextQuest;
    [SerializeField]
    protected BtnQuest BtnQuest;
    [SerializeField]
    protected QuestChoiceUi questChoiceUi;

    [Header("Texte affiche lors de la defaite")]
    [SerializeField]
    protected LossText Loss;

    protected Ouroboros PlayerInputs;

    protected Quest CurrentQuest;
    protected QuestManager questManager;
    protected SoundManager soundManager;
    protected MusicSFXManager musicSFXManager;

    public float timeTransitionAnimation = 0.1f;
    public float timeTransitionAnimationBalance = 1f;
    public float timeTransitionAnimationMoney = 0.1f;
    public float timeTransitionAnimationReadiness = 0.5f;

    protected Image[] NPCFileImage;

    private void Awake()
    {
        soundManager = gameObject.GetComponent<SoundManager>();
        musicSFXManager = GameObject.FindGameObjectWithTag("Music").GetComponent<MusicSFXManager>();

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

        if (NPCQueue != null)
            NPCFileImage = NPCQueue.GetComponentsInChildren<Image>();

        UIMenu.ShopUi.SetActive(false);
        UIMenu.InventoryUi.SetActive(false);
    }

    protected virtual void AccepterChoice(InputAction.CallbackContext obj)
    {
        questManager.NextQuest();
    }

    public virtual void SetQuest(Quest quest)
    {
        CurrentQuest = quest;

        if (CurrentQuest != null)
        {
            TextQuest.QuestInformation.SetActive(false);

            TextQuest.NomQuest.text = CurrentQuest.questDefinition.questName;

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

    float textspeed = 0.01f;
    protected IEnumerator TypeLine(TMP_Text tmpText, string text)
    {
        tmpText.text = "";

        musicSFXManager.ChangeSFX(CurrentQuest.questDefinition.questGiverAudio);
        musicSFXManager.SFXLoop(true);

        for (int i = 0; i < text.Length; i++)
        {
            //if <>
            if (text[i] == '<')
            {
                string substring = "";
                while (text[i] != '>')
                {
                    substring += text[i];
                    i++;
                }
                i++;
                substring += '>';
                tmpText.text += substring;

                //if <color>
                if (substring.Contains("color"))
                {
                    string coloredSubstring = "";
                    while (text[i] != '<')
                    {
                        coloredSubstring += text[i];
                        i++;
                    }
                    substring = "</color>";
                    i += substring.Length-1;

                    string currentText = tmpText.text;

                    for (int z = 0; z < coloredSubstring.Length; z++)
                    {
                        currentText += coloredSubstring[z];
                        tmpText.text = currentText + substring;
                        yield return new WaitForSecondsRealtime(textspeed);
                    }
                }
            }
            else
            {
                tmpText.text += text[i];
            }

            yield return new WaitForSecondsRealtime(textspeed);
        }

        musicSFXManager.SFXLoop(false);
    }

    public virtual void ShowHideShop()
    {
        UIMenu.ShopUi.SetActive(!UIMenu.ShopUi.activeSelf);
        UIMenu.InventoryUi.SetActive(!UIMenu.InventoryUi.activeSelf);
        UIMenu.ItemDescriptionUi.SetActive(UIMenu.InventoryUi.activeSelf);
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
    public  void SetStartingRessources(int argent, int yinYangBalance, float templeReadiness, int disciple, int ki)
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

    protected void MakeAllButtonInvisible(bool status)
    {
        BtnQuest.ButtonObject.SetActive(status);
    }

    public virtual void SetArgent(int argent)
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

    protected int lastBalance = 0;
    protected virtual async Task ChangeBalance(int newCurrentBalance)
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

    protected void BalanceClan(Clan nom, int balance)
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
    protected async Task ChangeDisciple(int newDisciple)
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
    protected async Task ChangeKi(int newKi)
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
        TextRessources.TempleReadiness.text = $"{Mathf.Round(readiness)} / 100";
    }

    float lastReadiness = 0;
    protected async Task ChangeReadiness(float newReadiness)
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

    public virtual void SetNPCFile(Level niveau, int questIndex)
    {
        Sprite questGiverSprite;
        int NPCLeft = NPCFileImage.Length;

        for (int i = 0; NPCLeft >= 0 && i < NPCFileImage.Length; i++)
        {
            NPCLeft--;

            if (questIndex + i < niveau.questList.Count)
            {
                questGiverSprite = niveau.questList[questIndex + i].questDefinition.questGiverSprite;

                if (questGiverSprite != null)
                {
                    HideSprite(NPCFileImage[NPCLeft], false);
                    NPCFileImage[NPCLeft].sprite = questGiverSprite;
                }
                else
                    HideSprite(NPCFileImage[NPCLeft], true);

            }
            else
                HideSprite(NPCFileImage[NPCLeft], true);
        }
    }

    protected void HideSprite(Image image, bool status)
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

    protected void RevealChoice(bool isChoice1, string txtChoice)
    {
        questChoiceUi.ShowChoice(isChoice1, txtChoice);
    }

    protected void ActivateBtn()
    {
        FirstChoiceBtnActivate(true);
        SecondChoiceBtnActivate(true);
        NextPersonBtnActivate(false);

        EventSystem.current.SetSelectedGameObject(MenuBtn.GameFirstChoice);
        EventSystem.current.firstSelectedGameObject = MenuBtn.GameFirstChoice;
    }

    protected void ActivateNextBtn()
    {
        ActivateNextPerson();
        ShowBtn(false);
    }

    protected void ActivateNextPerson()
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

    protected void MakeButtonInvisibleExceptNextPerson(bool status)
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

    protected void HideBtn()
    {
        BtnQuest.FirstChoice.gameObject.SetActive(false);
        BtnQuest.SecondChoice.gameObject.SetActive(false);
        BtnQuest.NextPerson.gameObject.SetActive(false);
    }

    protected async void OpeningScroll()
    {
        Vector2 normalSize = ScrollBackground.sizeDelta;
        int sizeIncrease = 60;
        int openingScrollDelay = 60;

        ScrollBackground.sizeDelta = new Vector2(25, 25);

        for (int i = 0; i < normalSize.x && i < normalSize.y; i += sizeIncrease)
        {
            ScrollBackground.sizeDelta = new Vector2(i * 2, i);
            await Task.Delay(openingScrollDelay);
        }

        ScrollBackground.sizeDelta = normalSize;

        TextQuest.QuestInformation.SetActive(true);
        ShowBtn(true);

        StopAllCoroutines();
        StartCoroutine(TypeLine(TextQuest.DescriptionQuest, $"{CurrentQuest.questDefinition.questDescription}"));
    }

    protected void ShowQuest(bool status)
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

    protected void InGameMenuPressed(InputAction.CallbackContext obj)
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

    protected void OnDestroy()
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
    public GameObject ItemDescriptionUi;
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