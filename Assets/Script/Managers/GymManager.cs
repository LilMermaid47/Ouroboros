using System;
using UnityEngine;
using TMPro;

public class GymManager : MonoBehaviour
{
    [SerializeField]
    private GymQuestInfo GymQuestInfo;
    [SerializeField]
    private GymRessourcesInfo GymRessourcesInfo;

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
        GymQuestInfo.Reward1.text = $"Money: {rewardChoice1.moneyReward}\nTemple Readiness: {rewardChoice1.templeReadiness}\n\nHuangsei:\nHonor: {rewardChoice1.recompenseClanHuangsei.honor}\nDisciple: {rewardChoice1.recompenseClanHuangsei.discple}\n" +
            $"Devotion: {rewardChoice1.recompenseClanHuangsei.devotion}\n\nSusoda:\nHonor: {rewardChoice1.recompenseClanSusoda.honor}\nDisciple {rewardChoice1.recompenseClanSusoda.discple}\nDevotion: {rewardChoice1.recompenseClanSusoda.devotion}";
        GymQuestInfo.Reward2.text = $"Money: {rewardChoice2.moneyReward}\nTemple Readiness: {rewardChoice2.templeReadiness}\n\nHuangsei:\nHonor: {rewardChoice2.recompenseClanHuangsei.honor}\nDisciple: {rewardChoice2.recompenseClanHuangsei.discple}\n" +
            $"Devotion: {rewardChoice2.recompenseClanHuangsei.devotion}\n\nSusoda:\nHonor: {rewardChoice2.recompenseClanSusoda.honor}\nDisciple {rewardChoice2.recompenseClanSusoda.discple}\nDevotion: {rewardChoice2.recompenseClanSusoda.devotion}";
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
        ClanDefinition huangsei = QuestManager.GetHuangseiClan();
        ClanDefinition susoda = QuestManager.GetSusodaClan();

        GymRessourcesInfo.HuangseiHonor.text = $"{huangsei.honor}";
        GymRessourcesInfo.HuangseiDisciple.text = $"{huangsei.discple}";
        GymRessourcesInfo.HuangseiDevotion.text = $"{huangsei.devotion}";

        GymRessourcesInfo.SusodaHonor.text = $"{susoda.honor}";
        GymRessourcesInfo.SusodaDisciple.text = $"{susoda.discple}";
        GymRessourcesInfo.SusodaDevotion.text = $"{susoda.devotion}";

        GymRessourcesInfo.YinYang.text = $"{QuestManager.maxClanBalance}";
        GymRessourcesInfo.Readiness.text = $"{QuestManager.GetReadiness()}";
        GymRessourcesInfo.Money.text = $"{QuestManager.GetMoney()}";
    }

    public void CloseGymMenu()
    {
        gameObject.SetActive(false);
    }

    public void ApplyChanges()
    {
        ClanDefinition huangsei = new ClanDefinition(float.Parse(GymRessourcesInfo.HuangseiHonor.text), int.Parse(GymRessourcesInfo.HuangseiDisciple.text), float.Parse(GymRessourcesInfo.HuangseiDevotion.text));
        ClanDefinition susoda = new ClanDefinition(float.Parse(GymRessourcesInfo.SusodaHonor.text), int.Parse(GymRessourcesInfo.SusodaDisciple.text), float.Parse(GymRessourcesInfo.SusodaDevotion.text));     
        
        QuestManager.ChangeValue(int.Parse(GymRessourcesInfo.YinYang.text),float.Parse(GymRessourcesInfo.Readiness.text), int.Parse(GymRessourcesInfo.Money.text), huangsei, susoda);
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
    public TMP_InputField HuangseiHonor;
    public TMP_InputField HuangseiDisciple;
    public TMP_InputField HuangseiDevotion;
    public TMP_InputField SusodaHonor;
    public TMP_InputField SusodaDisciple;
    public TMP_InputField SusodaDevotion;
    public TMP_InputField YinYang;
    public TMP_InputField Readiness;
    public TMP_InputField Money;
}
