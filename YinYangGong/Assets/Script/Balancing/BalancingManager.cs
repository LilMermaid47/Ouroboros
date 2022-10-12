using System;
using System.IO;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class BalancingManager : MonoBehaviour
{
    [Header("Valeur de départ du niveau")]
    public int MaxClanBalance = 100;
    public int MaxKi = 10;
    [SerializeField]
    private int StartingDisciple = 0;
    [SerializeField]
    private int StartingKi = 0;
    [SerializeField]
    private int StartingArgent = 0;
    [SerializeField]
    float templeReadinessToAchieve = 100;

    public Level level;
    public RandomQuestList randomQuestList;

    private Level filledLevel;
    private RandomQuestList tempQuestList;

    [SerializeField]
    private TextMeshProUGUI QuestListTxt;
    [SerializeField]
    private TextMeshProUGUI LogTxt;    
    [SerializeField]
    private Button BotBtn;

    private float NbWin = 0;
    private float NbLoss = 0;

    private List<string> LogList = new List<string>();
    private List<Task> TaskList = new List<Task>();
    private bool TaskDone = false;

    private string TxtDocumentName = $"{Application.streamingAssetsPath}/BalancingLogs/Log{DateTime.Now.ToString("dd-MM-yy")}.txt";

    private void Start()
    {
        if (!Directory.Exists($"{Application.streamingAssetsPath}/BalancingLogs/"))
        {
            Directory.CreateDirectory($"{Application.streamingAssetsPath}/BalancingLogs/");
        }

        EnableBtn(false);

        StartBotBtn();
    }

    public void StartBotBtn()
    {
        LogTxt.text = "";
        QuestListTxt.text = "";

        NbWin = 0;
        NbLoss = 0;
        
        if (!File.Exists(TxtDocumentName))
            File.WriteAllText(TxtDocumentName, $"Start Balancing Log: {DateTime.Now.ToString("dd/MM/yy  HH:mm")}\n\n");
        else
        {
            File.Delete(TxtDocumentName);
            File.WriteAllText(TxtDocumentName, $"Start Balancing Log: {DateTime.Now.ToString("dd/MM/yy  HH:mm")}\n");
        }


        if (level != null && randomQuestList != null)
        {
            filledLevel = Instantiate(level);
            tempQuestList = Instantiate(randomQuestList);
            SideQuestFiller();

            CreateQuestList();

            StartBot();
        }
        else
            Debug.LogError($"List filled level is {filledLevel} and random quest list is {randomQuestList}.");
    }

    private void SideQuestFiller()
    {
        for (int i = 0; i < level.questList.Count; i++)
        {
            if (filledLevel.questList[i] == null)
            {
                int randomNumber = UnityEngine.Random.Range(0, tempQuestList.randomQuestList.Count);
                var quest = tempQuestList.randomQuestList[randomNumber];
                tempQuestList.randomQuestList.Remove(quest);
                filledLevel.questList[i] = quest;
            }
        }
    }

    private void CreateQuestList()
    {
        string listQuest = "";

        if (filledLevel.questList != null)
        {
            listQuest = $"1- {filledLevel.questList[0].questDefinition.questName}";

            for (int i = 1; i < filledLevel.questList.Count; i++)
            {
                listQuest += $"\n{i + 1}- {filledLevel.questList[i].questDefinition.questName}";
            }
        }

        QuestListTxt.text = listQuest;
    }

    IEnumerator ShowEndLog()
    {
        do
        {
            LogTxt.text = $"Nombre de fin: {NbWin + NbLoss}\nNombre de Victoire: {NbWin}\nNombre de Défaites: {NbLoss}\nWinrate:{NbWin / (NbWin + NbLoss) * 100:0}%";
            yield return new WaitForSeconds(2);
        }
        while (!TaskDone);
    }

    IEnumerator CreateLog()
    {
        File.AppendAllText(TxtDocumentName, $"Nombre de fin: {NbWin + NbLoss}\nNombre de Victoire: {NbWin}\nNombre de Defaites: {NbLoss}\nWinrate:{NbWin / (NbWin + NbLoss) * 100:0}%\n\n");
        File.AppendAllText(TxtDocumentName, string.Join("--------------------------------------------------------------------------------------------------------------------------------------------", LogList));

        yield return 0;

        LogTxt.text += "\n\nLog saved!";
        EnableBtn(true);
    }

    private async void StartBot()
    {

        TaskList.Add(Bot(BotSideChoice.Left));
        TaskList.Add(Bot(BotSideChoice.Right));
        StartCoroutine(ShowEndLog());

        await Task.WhenAll(TaskList);

        TaskDone = true;        
        StartCoroutine(CreateLog());
    }

    private async Task Bot(BotSideChoice side)
    {

            PlayerStats stats = new PlayerStats(StartingArgent, 0, StartingDisciple, StartingKi, 0, MaxKi);
            AIStack stack = new AIStack(0, null, side);

            bool leftSide;
            bool rightSide;

            Level currentLevel = Instantiate(filledLevel);
            Quest currentQuest;
            Reward reward = null;

        await Task.Run(() =>
        {
            do
            {
                currentQuest = NextQuest(currentLevel, stack.QuestIndex);

                leftSide = CheckRequirement(currentQuest, stats, BotSideChoice.Left);
                rightSide = CheckRequirement(currentQuest, stats, BotSideChoice.Right);

                if (side == BotSideChoice.Left)
                {
                    if (stack.BotSideChoice == BotSideChoice.Left && leftSide)
                    {
                        reward = currentQuest.questDefinition.rewardChoice1;
                        stack.BotSideChoice = BotSideChoice.Right;
                    }
                    else if (rightSide)
                    {
                        reward = currentQuest.questDefinition.rewardChoice2;
                        stack.BotSideChoice = BotSideChoice.Both;
                    }
                    else
                    {
                        reward = null;
                        stack.BotSideChoice = BotSideChoice.Both;
                    }
                }
                else if ((side == BotSideChoice.Right))
                {
                    if (stack.BotSideChoice == BotSideChoice.Right && rightSide)
                    {
                        reward = currentQuest.questDefinition.rewardChoice2;
                        stack.BotSideChoice = BotSideChoice.Left;
                    }
                    else if (leftSide)
                    {
                        reward = currentQuest.questDefinition.rewardChoice1;
                        stack.BotSideChoice = BotSideChoice.Both;
                    }
                    else
                    {
                        reward = null;
                        stack.BotSideChoice = BotSideChoice.Both;
                    }
                }
                else
                    Debug.LogError("Bot path is both.");

                if (reward != null)
                {
                    SetRewards(stats, stack, reward);
                    AddQuest(reward, currentLevel, stack.QuestIndex);
                }

                if (reward == null || CheckLoss(stats) || stack.QuestIndex >= currentLevel.questList.Count - 1)
                {
                    if (CheckWin(stats) && stack.QuestIndex == currentLevel.questList.Count - 1)
                    {
                        NbWin++;
                        CreateLog(stack, currentLevel, side, true);
                    }
                    else if (CheckLoss(stats) || (!CheckWin(stats) && stack.QuestIndex == currentLevel.questList.Count - 1))
                    {
                        NbLoss++;
                        CreateLog(stack, currentLevel, side, false);
                    }

                    RemoveRewards(stats, stack);
                    RemoveQuest(stack, currentLevel, side);

                    while (stack != null && stack.BotSideChoice == BotSideChoice.Both)
                    {
                        stack = stack.PreviousAIStack;

                        if (stack != null)
                        {
                            RemoveRewards(stats, stack);
                            RemoveQuest(stack, currentLevel, side);
                        }
                    }
                }
                else
                {
                    stack = new AIStack(stack.QuestIndex + 1, stack, side);
                }
            }
            while (stack != null && stack.QuestIndex != 0);
        });
    }


    private Quest NextQuest(Level filledLevel, int currentQuestIndex)
    {
        Quest nextQuest = null;

        if (currentQuestIndex < filledLevel.questList.Count)
        {
            nextQuest = filledLevel.questList[currentQuestIndex];
        }

        return nextQuest;
    }

    private void AddQuest(Reward reward, Level filledLevel, int currentIndex)
    {
        int index = 0;
        Quest questToAdd = null;

        if (reward.unlockQuestChoice.unlockedQuest != null)
        {
            questToAdd = reward.unlockQuestChoice.unlockedQuest;
            index = reward.unlockQuestChoice.nbQuestLater + currentIndex;

            if (index > filledLevel.questList.Count)
                index = filledLevel.questList.Count;

            if (!filledLevel.questList.Contains(questToAdd))
                filledLevel.questList.Insert(index, questToAdd);
        }
    }

    private void RemoveQuest(AIStack stack, Level filledLevel, BotSideChoice side)
    {
        Quest questToRemove = null;
        Reward rewardToRemove = RewardToRemove(stack, filledLevel, side);

        if (rewardToRemove != null)
        {
            questToRemove = rewardToRemove.unlockQuestChoice.unlockedQuest;

            if (questToRemove != null && filledLevel.questList.Contains(questToRemove))
                filledLevel.questList.Remove(questToRemove);
        }
    }

    private Reward RewardToRemove(AIStack stack, Level level, BotSideChoice side)
    {
        Reward rewardToRemove = null;
        Quest currentQuest = null;

        if (stack.QuestIndex >= 0 && stack.QuestIndex < level.questList.Count - 1)
        {
            currentQuest = level.questList[stack.QuestIndex];

            if (side == BotSideChoice.Left)
            {
                if (stack.BotSideChoice == BotSideChoice.Right)
                {
                    rewardToRemove = currentQuest.questDefinition.rewardChoice1;
                }
                else
                {
                    rewardToRemove = currentQuest.questDefinition.rewardChoice2;
                }
            }
            else if ((side == BotSideChoice.Right))
            {
                if (stack.BotSideChoice == BotSideChoice.Left)
                {
                    rewardToRemove = currentQuest.questDefinition.rewardChoice2;
                }
                else
                {
                    rewardToRemove = currentQuest.questDefinition.rewardChoice1;
                }
            }
        }

        return rewardToRemove;
    }

    private bool CheckRequirement(Quest quest, PlayerStats stats, BotSideChoice sideChoice)
    {
        bool passed = true;
        RequirementQuest requirementQuest = null;
        Requirement requirement = null;

        if (quest.QuestType() == TypeOfQuest.RequirementQuest)
        {
            requirementQuest = (RequirementQuest)quest;

            if (sideChoice == BotSideChoice.Left)
                requirement = requirementQuest.requirementChoice1;
            else
                requirement = requirementQuest.requirementChoice2;

            if (stats.money < requirement.moneyCost || stats.templeReadiness < requirement.templeReadiness || stats.nbKi < requirement.kiCost || stats.nbDisciple < requirement.disciples)
                passed = false;
        }
        return passed;
    }

    private void SetRewards(PlayerStats stats, AIStack stack, Reward reward)
    {
        stats.money += reward.moneyReward;
        stats.templeReadiness += reward.templeReadiness;
        stats.nbDisciple += reward.nbDisciple;
        stats.nbKi += reward.nbKi;
        stats.yinYangBalance += reward.yinYangBalance;

        stack.oldStats.SetStats(stats);
    }

    private void RemoveRewards(PlayerStats stats, AIStack stack)
    {
        stats.SetStats(stack.oldStats);
    }

    private bool CheckWin(PlayerStats stats)
    {
        return stats.templeReadiness >= templeReadinessToAchieve;
    }

    private bool CheckLoss(PlayerStats stats)
    {
        bool lost = false;

        if (stats.money < 0 || stats.nbDisciple <= 0 || stats.yinYangBalance <= -MaxClanBalance || stats.yinYangBalance >= MaxClanBalance || stats.nbKi < 0)
            lost = true;

        return lost;
    }

    private void CreateLog(AIStack stack, Level currentLevel, BotSideChoice bot, bool victory)
    {
        int index;
        string log = $"\n\nBot:{bot}";
        string choice;

        QuestDefinition questDefinition;
        AIStack tempStack = stack;
        List<AIStack> botPath = new List<AIStack>();

        if (victory)
            log += "\tVictory!\n\n";
        else
            log += LossReason(stack.oldStats);

        do
        {
            botPath.Insert(0, tempStack);
            tempStack = tempStack.PreviousAIStack;
        }
        while (tempStack != null);

        foreach (AIStack item in botPath)
        {
            index = item.QuestIndex;
            questDefinition = currentLevel.questList[index].questDefinition;

            if (bot == BotSideChoice.Right && item.BotSideChoice == BotSideChoice.Both || bot == BotSideChoice.Left && item.BotSideChoice == BotSideChoice.Right)
            {
                choice = questDefinition.choice1Name;
            }
            else
            {
                choice = questDefinition.choice2Name;
            }

            log += $"Quest:{index}-{questDefinition.questName}\nChoice:{choice}\n{item.oldStats.ToString()}\n\n";
        }

        LogList.Add(log);
    }

    private string LossReason(PlayerStats stats)
    {
        string loss = "\t";


        if (stats.yinYangBalance <= -MaxClanBalance || stats.yinYangBalance >= MaxClanBalance)
            loss += "Yin Yang Balance loss";
        else if (stats.money < 0)
            loss += "Money loss";
        else if (stats.nbDisciple <= 0)
            loss += "Disciple loss";
        else if (stats.nbKi < 0)
            loss += "Ki loss";
        else
            loss += "Readiness loss";

        loss += "\n\n";

        return loss;
    }

    public void EnableBtn(bool status)
    {
        BotBtn.interactable = status; 
    }

    public enum BotSideChoice
    {
        Right,
        Left,
        Both
    }

    [Serializable]
    public class PlayerStats
    {
        public int money = 0;
        public float templeReadiness = 0;
        public int nbDisciple = 0;
        public int nbKi = 0;
        public int yinYangBalance = 0;
        public int maxKi = 10;

        public PlayerStats()
        {
            money = 0;
            templeReadiness = 0;
            nbDisciple = 0;
            nbKi = 0;
            yinYangBalance = 0;
            maxKi = 10;
    }

        public PlayerStats(int playerMoney, float readiness, int disciple, int ki, int balance)
        {
            money = playerMoney;
            templeReadiness = readiness;
            nbDisciple = disciple;
            nbKi = ki;
            yinYangBalance = balance;
        }
        public PlayerStats(int playerMoney, float readiness, int disciple, int ki, int balance, int nbKi)
        {
            money = playerMoney;
            templeReadiness = readiness;
            nbDisciple = disciple;
            nbKi = ki;
            yinYangBalance = balance;
            maxKi = nbKi;
        }

    public void SetStats(PlayerStats playerStats)
        {
            money = playerStats.money;
            templeReadiness = playerStats.templeReadiness;
            nbDisciple = playerStats.nbDisciple;
            yinYangBalance = playerStats.yinYangBalance;
            nbKi = playerStats.nbKi;

            if(nbKi > maxKi)
                nbKi = maxKi;
        }

        public override string ToString()
        {
            return $"Ki:{nbKi} \t||\t Money:{money} \t||\t Yin Yang Balance:{yinYangBalance} \t||\t Disciple:{nbDisciple} \t||\t TempleReadiness:{templeReadiness}";
        }
    }

    [Serializable]
    private class AIStack
    {
        public int QuestIndex = 0;
        public AIStack PreviousAIStack = null;
        public PlayerStats oldStats = new PlayerStats();

        public BotSideChoice BotSideChoice;

        public AIStack(int questIndex, AIStack previousAIStack, BotSideChoice botSideChoice)
        {
            QuestIndex = questIndex;
            PreviousAIStack = previousAIStack;
            BotSideChoice = botSideChoice;
        }
    }
}
