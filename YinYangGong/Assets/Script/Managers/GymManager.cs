using System;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class GymManager : MonoBehaviour
{
    [SerializeField]
    private GameObject AddQuestMenu;
    [SerializeField]
    private GameObject PopUp;

    [SerializeField]
    private GymQuestInfo GymQuestInfo;
    [SerializeField]
    private GymRessourcesInfo GymRessourcesInfo;
    [SerializeField]
    private GymNewQuest NewQuest;

    private Quest ActiveQuest;
    private Level QuestList;

    private QuestManager QuestManager;

    private void OnEnable()
    {
        FindQuestManager();

        SetInformation();

        if (ActiveQuest != null)
            QuestInformation();
        if (QuestList != null)
            CreateQuestList();
        SetRessources();
    }

    private void FindQuestManager()
    {
        GameObject QuestObject;

        if (QuestManager == null)
        {
            QuestObject = GameObject.FindGameObjectWithTag("QuestManager");

            if (QuestObject != null)
                QuestManager = QuestObject.GetComponent<QuestManager>();
            else
                Debug.Log("Quest Manager not found");
        }
    }

    public void SetInformation()
    {
        SetQuestList(QuestManager.GetFilledLevel());
        SetActiveQuest(QuestManager.GetActiveQuest());
    }

    public void SetQuestList(Level questList)
    {
        QuestList = questList;
    }

    public void SetActiveQuest(Quest currentQuest)
    {
        ActiveQuest = currentQuest;
    }

    private void QuestInformation()
    {
        QuestDefinition questDefinition = ActiveQuest.questDefinition;
        Reward rewardChoice1 = questDefinition.rewardChoice1;
        Reward rewardChoice2 = questDefinition.rewardChoice2;

        GymQuestInfo.QuestName.text = questDefinition.questName;
        GymQuestInfo.QuestDesc.text = $"{questDefinition.questDescription}\n\n\t{questDefinition.questGiverName,10:C}";
        GymQuestInfo.Choice1Info.text = $"Name: {questDefinition.choice1Name}\n\nDefinition: {questDefinition.choice1Description}\n\nReveal: {questDefinition.choice1Reveal}";
        GymQuestInfo.Choice2Info.text = $"Name: {questDefinition.choice2Name}\n\nDefinition: {questDefinition.choice2Description}\n\nReveal: {questDefinition.choice2Reveal}";
        GymQuestInfo.Reward1.text = $"Money: {rewardChoice1.moneyReward}\nTemple Readiness: {rewardChoice1.templeReadiness}\nYin Yang Balance: {rewardChoice1.yinYangBalance}\nNombreDisciple: {rewardChoice1.nbDisciple}\nKi: {rewardChoice1.nbKi}";

        GymQuestInfo.Reward2.text = $"Money: {rewardChoice2.moneyReward}\nTemple Readiness: {rewardChoice2.templeReadiness}\nYin Yang Balance: {rewardChoice2.yinYangBalance}\nNombreDisciple: {rewardChoice2.nbDisciple}\nKi: {rewardChoice2.nbKi}";
    }

    private void CreateQuestList()
    {
        string listQuest = "";

        if (QuestList.questList != null)
        {
            listQuest = $"1- {QuestList.questList[0].questDefinition.questName}";

            for (int i = 1; i < QuestList.questList.Count; i++)
            {
                listQuest += $"\n{i+1}- {QuestList.questList[i].questDefinition.questName}";
            }
        }

        GymQuestInfo.QuestList.text = listQuest;
    }

    private void SetRessources()
    {
        GymRessourcesInfo.MaxYinYang.text = $"{QuestManager.maxClanBalance}";
        GymRessourcesInfo.YinYangBalance.text = $"{QuestManager.GetYinYangBalance()}";
        GymRessourcesInfo.Readiness.text = $"{QuestManager.GetReadiness()}";
        GymRessourcesInfo.Money.text = $"{QuestManager.GetMoney()}";
        GymRessourcesInfo.NbDisciple.text = $"{QuestManager.GetDisciple()}";
        GymRessourcesInfo.Ki.text = $"{QuestManager.GetKi()}";
    }

    public void CloseGymMenu()
    {
        gameObject.SetActive(false);
    }

    public void OpenAddQuestMenu()
    {
        AddQuestMenu.SetActive(true);
        SetNewQuestValues();
    }

    public void CloseAddQuestMenu()
    {
        AddQuestMenu.SetActive(false);
    }

    public void PopUpWindow(string information)
    {
        PopUp.SetActive(true);
        SetInterractible(false);
        PopUp.GetComponentInChildren<TextMeshProUGUI>().text = information;
    }

    public void ClosePopUpWindow()
    {
        SetInterractible(true);
        PopUp.SetActive(false);
    }

    public void ApplyChanges()
    {
        QuestManager.ChangeValue(int.Parse(GymRessourcesInfo.MaxYinYang.text), int.Parse(GymRessourcesInfo.YinYangBalance.text), float.Parse(GymRessourcesInfo.Readiness.text), int.Parse(GymRessourcesInfo.Money.text), int.Parse(GymRessourcesInfo.NbDisciple.text), int.Parse(GymRessourcesInfo.Ki.text));
    }

    private void SetNewQuestValues()
    {
        NewQuest.QuestPosition.text = $"{QuestManager.GetCurrentQuestPosition() + 2}";
    }

    public void AddQuest()
    {
        Quest newQuest = ValidatedQuest();
        int newQuestPosition = ValidateQuestIndex();

        if (newQuest != null)
        {
            QuestManager.AddQuest(newQuestPosition, newQuest);
            CreateQuestList();
            ResetInputFields();
            PopUpWindow("Quest added.");
        }
        else
            PopUpWindow("Quest Invalid!");
    }

    private Quest ValidatedQuest()
    {
        Quest newQuest = null;

        if (!EmptyField())
        {
            newQuest = Quest.CreateInstance<Quest>();
            newQuest.questDefinition = CreateDefinition();            
        }

        return newQuest;
    }

    private QuestDefinition CreateDefinition()
    {
        QuestDefinition newDefinition = new QuestDefinition();

        newDefinition.questName = NewQuest.Name.text;
        newDefinition.questDescription = NewQuest.Description.text;
        newDefinition.questGiverName = NewQuest.QuestGiver.text;
        newDefinition.choice1Name = NewQuest.Choice1Name.text;
        newDefinition.choice1Description = NewQuest.Choice2Desc.text;
        newDefinition.choice1Reveal = NewQuest.Choice1Reveal.text;
        newDefinition.choice2Name = NewQuest.Choice2Name.text;
        newDefinition.choice2Description = NewQuest.Choice2Desc.text;
        newDefinition.choice2Reveal = NewQuest.Choice2Reveal.text;

        newDefinition.rewardChoice1 = CreateReward1();
        newDefinition.rewardChoice2 = CreateReward2();

        return newDefinition;
    }

    private Reward CreateReward1()
    {
        Reward newReward = new Reward();

        newReward.moneyReward = int.Parse(NewQuest.Reward1Money.text);
        newReward.templeReadiness = float.Parse(NewQuest.Reward1Readiness.text);
        newReward.yinYangBalance = int.Parse(NewQuest.Reward1Balance.text);
        newReward.nbDisciple = int.Parse(NewQuest.Reward1Disciple.text);
        newReward.nbKi = int.Parse(NewQuest.Reward1Ki.text);

        return newReward;
    }    
    
    private Reward CreateReward2()
    {
        Reward newReward = new Reward();

        newReward.moneyReward = int.Parse(NewQuest.Reward2Money.text);
        newReward.templeReadiness = float.Parse(NewQuest.Reward2Readiness.text);
        newReward.yinYangBalance = int.Parse(NewQuest.Reward2Balance.text);
        newReward.nbDisciple = int.Parse(NewQuest.Reward2Disciple.text);
        newReward.nbKi = int.Parse(NewQuest.Reward2Ki.text);

        return newReward;
    }

    private bool EmptyField()
    {
        string fieldText;
        bool empty = false;

        TMP_InputField[] field = AddQuestMenu.GetComponentsInChildren<TMP_InputField>();

        for (int i = 0; !empty && i < field.Length; i++)
        {
            fieldText = field[i].text;

            if (string.IsNullOrWhiteSpace(fieldText))
            {
                empty = true;
            }
        }

        return empty;
    }

    private void ResetInputFields()
    {
        TMP_InputField[] field = AddQuestMenu.GetComponentsInChildren<TMP_InputField>();

        for (int i = 0; i < field.Length; i++)
        {
            field[i].text = "";
        }
        SetNewQuestValues();
    }

    private int ValidateQuestIndex()
    {
        int newQuestPosition = 0;
        int currentPosition = QuestManager.GetCurrentQuestPosition();
        int lastPosition = QuestManager.GetFilledLevel().questList.Count-1;

        if (!NewQuest.QuestPosition.text.Equals(""))
            newQuestPosition = int.Parse(NewQuest.QuestPosition.text) -1;

        if (newQuestPosition < currentPosition)
            newQuestPosition = currentPosition;
        else if(newQuestPosition > lastPosition)
            newQuestPosition = lastPosition;

        return newQuestPosition;
    }

    private void SetInterractible(bool value)
    {
        Button[] allButtons = gameObject.GetComponentsInChildren<Button>();
        TMP_InputField[] allInputs = gameObject.GetComponentsInChildren<TMP_InputField>();

        foreach (Button item in allButtons)
        {
            item.interactable = value;
        }

        foreach (TMP_InputField item in allInputs)
        {
            item.interactable = value;
        }

        PopUp.GetComponentInChildren<Button>().interactable = !value;
    }
}

[Serializable]
public class GymQuestInfo
{
    public TextMeshProUGUI QuestName;
    public TextMeshProUGUI QuestDesc;
    public TextMeshProUGUI Choice1Info;
    public TextMeshProUGUI Choice2Info;
    public TextMeshProUGUI Reward1;
    public TextMeshProUGUI Reward2;
    public TextMeshProUGUI QuestList;
}

[Serializable]
public class GymRessourcesInfo
{
    public TMP_InputField Money;
    public TMP_InputField Readiness;
    public TMP_InputField MaxYinYang;
    public TMP_InputField YinYangBalance;
    public TMP_InputField NbDisciple;
    public TMP_InputField Ki;
}

[Serializable]
public class GymNewQuest
{
    public TMP_InputField Name;
    public TMP_InputField Description;
    public TMP_InputField QuestGiver;
    public TMP_InputField QuestPosition;
    public TMP_InputField Choice1Name;
    public TMP_InputField Choice1Desc;
    public TMP_InputField Choice1Reveal;
    public TMP_InputField Reward1Money;
    public TMP_InputField Reward1Readiness;
    public TMP_InputField Reward1Balance;
    public TMP_InputField Reward1Disciple;
    public TMP_InputField Reward1Ki;
    public TMP_InputField Choice2Name;
    public TMP_InputField Choice2Desc;
    public TMP_InputField Choice2Reveal;
    public TMP_InputField Reward2Money;
    public TMP_InputField Reward2Readiness;
    public TMP_InputField Reward2Balance;
    public TMP_InputField Reward2Disciple;
    public TMP_InputField Reward2Ki;
}
