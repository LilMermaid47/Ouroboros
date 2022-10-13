using UnityEngine;

public class TutorialQuestManager : QuestManager
{
    [SerializeField]
    private QuestChoiceUi ChoiceUi;

    private TutorialUIController TutorialUI; 

    private void Start()
    {
        TutorialUI = GameObject.FindGameObjectWithTag("Canvas").GetComponent<TutorialUIController>();
        uIController = TutorialUI;

        ChoiceUi = GameObject.FindGameObjectWithTag("Canvas").GetComponentInChildren<QuestChoiceUi>();

        musicSFXManager = GameObject.FindGameObjectWithTag("Music").GetComponent<MusicSFXManager>();

        //creates a copy to not lose original data (important)
        filledLevel = Instantiate(level);
        tempQuestList = Instantiate(randomQuestList);
        SideQuestFiller();
        currentQuest = filledLevel.questList[currentQuestIndex];

        uIController.SetMaxBalance(maxClanBalance);
        uIController.SetMaxKi(maxKi);

        uIController.SetQuest(currentQuest);
        uIController.SetStartingRessources(argent, yinYangBalance, (templeReadiness / templeReadinessToAchieve) * 100, disciple, ki);
        uIController.SetNbQuestLeft(filledLevel.questList.Count - currentQuestIndex);
        uIController.SetNPCFile(filledLevel, currentQuestIndex);
    }
    public override void NextQuest()
    {
        if (currentQuest.QuestType() == TypeOfQuest.MerchantQuest)
        {
            MerchantQuest merchantQuest = (MerchantQuest)currentQuest;

            if (choice1WasMade)
                MerchantQuestReward(merchantQuest.itemChoice1.item);
            else
                MerchantQuestReward(merchantQuest.itemChoice2.item);
        }
        currentQuestIndex++;
        Debug.Log(currentQuestIndex);

        switch (currentQuestIndex)
        {
            case 2:
                TutorialUI.HideRessources(Ressources.Balance, false);
                TutorialUI.HideOldMonk(false);
                break;
            case 4:
                TutorialUI.HideOldMonk(true);
                break;
            case 5:
                TutorialUI.HideRessources(Ressources.Readiness, false);
                TutorialUI.HideRessources(Ressources.Audience, false);
                TutorialUI.HideOldMonk(false);
                break;
            case 6:
                TutorialUI.HideOldMonk(true);
                break;
            case 7:
                TutorialUI.HideRessources(Ressources.Yuan, false);
                break;
            case 9:
                ChoiceUi.HideAcceptBtn(false);
                TutorialUI.HideOldMonk(false);
                break;
            case 10:
                ChoiceUi.HideAcceptBtn(true);
                break;
        }

        if (currentQuestIndex < filledLevel.questList.Count)
        {
            UpdateCurrentQuest();

            if (currentQuest.questDefinition.questName == "Leader of Huangsei")
                TutorialUI.ShowHuangseiFlag(true);
            else if (currentQuest.questDefinition.questName == "Leader of Susoda")
                TutorialUI.ShowSusodaFlag(true);
        }
        else
        {
            CheckIfPlayerWon();
        }

        CheckIfStillWinning();
    }
}
