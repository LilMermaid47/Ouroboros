using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using static UnityEngine.InputSystem.LowLevel.InputStateHistory;

public class BalancingManager: MonoBehaviour
{
    [Header("Valeur de départ du niveau")]
    public float ClanHonorDevotionModifier = 0.2f;
    public int MaxClanBalance = 100;
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

    private int NbEnd = 0;
    private int NbWin = 0;
    private int NbLoss = 0;

    private void Start()
    {
        //creates a copy to not lose original data (important)
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

    //Come fill all the null element in the level list by random quest in the RandomQuestList
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

    private void ShowEndLog()
    {
        LogTxt.text = $"Nombre de fin: {NbEnd}\nNombre de Victoire: {NbWin}\nNombre de Defaites: {NbLoss}";
    }

    private async void StartBot()
    {
        List<Task> taskList = new List<Task>();

        taskList.Add(Bot(BotSideChoice.Left));
        taskList.Add(Bot(BotSideChoice.Right));

        await Task.WhenAll(taskList);

        ShowEndLog();
    }

    private async Task Bot(BotSideChoice side)
    {
        PlayerStats stats = new PlayerStats(StartingArgent, 0, StartingDisciple, StartingKi, 0);
        AIStack stack = new AIStack(0, null, side);

        bool leftSide;
        bool rightSide;

        Level currentLevel = Instantiate(filledLevel);
        Quest currentQuest;
        Reward reward = null;

        await Task.Yield();

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
                reward = currentQuest.questDefinition.rewardChoice1;

                SetRewards(stats, reward);
                AddQuest(reward, currentLevel, stack.QuestIndex);
            }

            if (CheckLoss(stats) || stack.QuestIndex >= currentLevel.questList.Count - 1)
            {
                if (CheckWin(stats))
                {
                    NbWin++;
                }
                else
                    NbLoss++;

                NbEnd++;

                RemoveQuest(stack, currentLevel, side);

                if (stack.QuestIndex != 0 && stack.BotSideChoice == BotSideChoice.Both)
                {
                    stack = stack.PreviousAIStack;
                    RemoveQuest(stack, currentLevel, side);

                    while (stack != null && stack.BotSideChoice == BotSideChoice.Both)
                    {
                        stack = stack.PreviousAIStack;
                        RemoveQuest(stack, currentLevel, side);
                    }
                }
            }
            else
            {
                stack = new AIStack(stack.QuestIndex + 1, stack, side);
            }
            Debug.Log(currentLevel.questList.Count);
        }
        while (stack != null && stack.QuestIndex != 0);
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

        if (rewardToRemove.unlockQuestChoice.unlockedQuest != null)
        {
            questToRemove = rewardToRemove.unlockQuestChoice.unlockedQuest;

            if (filledLevel.questList.Contains(questToRemove))
                filledLevel.questList.Remove(questToRemove);
        }
    }

    private Reward RewardToRemove(AIStack stack, Level level, BotSideChoice side)
    {
        Reward rewardToRemove = null;
        Quest currentQuest = level.questList[stack.QuestIndex];

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

            if (stats.money < requirement.moneyCost || stats.templeReadiness < requirement.templeReadiness || stats.nbKi < requirement.kiCost)
                passed = false;
        }

        return passed;
    }

    private void SetRewards(PlayerStats stats, Reward reward)
    {
        stats.money += reward.moneyReward;
        stats.templeReadiness += reward.templeReadiness;
        stats.nbDisciple += reward.nbDisciple;
        stats.nbKi += reward.nbKi;
        stats.yinYangBalance += reward.yinYangBalance;
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

    public enum BotSideChoice
    {
        Right,
        Left,
        Both
    }

    [Serializable]
    public class PlayerStats
    {
        public int money { get; set; } = 0;
        public float templeReadiness { get; set; } = 0;
        public int nbDisciple { get; set; } = 0;
        public int nbKi { get; set; } = 0;
        public int yinYangBalance { get; set; } = 0;

        public PlayerStats(int playerMoney, float readiness, int disciple, int ki, int balance)
        {
            money = playerMoney;
            templeReadiness = readiness;
            nbDisciple = disciple;
            nbKi = ki;
            yinYangBalance = balance;
        }
    }

    [Serializable]
    private class AIStack
    {
        public int QuestIndex { get; set; } = 0;
        public AIStack PreviousAIStack { get; set; } = null;

        public BotSideChoice BotSideChoice { get; set; }

        public AIStack(int questIndex, AIStack previousAIStack, BotSideChoice botSideChoice)
        {
            QuestIndex = questIndex;
            PreviousAIStack = previousAIStack;
            BotSideChoice = botSideChoice;
        }
    }
}
