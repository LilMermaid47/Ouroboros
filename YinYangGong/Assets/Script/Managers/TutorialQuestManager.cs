using UnityEngine;

public class TutorialQuestManager : QuestManager
{
    [SerializeField]
    private QuestChoiceUi ChoiceUi;

    private TutorialUIController TutorialUI;
    private SceneController sceneController;

    private void Start()
    {
        TutorialUI = GameObject.FindGameObjectWithTag("Canvas").GetComponent<TutorialUIController>();
        uIController = TutorialUI;
        sceneController = TutorialUI.gameObject.GetComponent<SceneController>();

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

        if (currentQuestIndex < filledLevel.questList.Count)
        {
            UpdateCurrentQuest();

            switch (currentQuest.questDefinition.questName)
            {
                case "Leader of Huangsei":
                    TutorialUI.HideRessources(Ressources.Balance, false);
                    TutorialUI.HideOldMonk(false);
                    TutorialUI.ShowHuangseiFlag(true);
                    break;
                case "Leader of Susoda":
                    TutorialUI.ShowSusodaFlag(true);
                    break;
                case "Assume One's Wrongs":
                    TutorialUI.HideRessources(Ressources.Readiness, false);
                    TutorialUI.HideRessources(Ressources.Audience, false);
                    TutorialUI.HideOldMonk(false);
                    break;
                case "First day":
                    TutorialUI.HideOldMonk(true);
                    break;                
                case "The Clans":
                    TutorialUI.HideOldMonk(true);
                    break;
                case "General Store":
                    TutorialUI.HideRessources(Ressources.Yuan, false);
                    break;
                case "Buying Peace":
                    ChoiceUi.HideAcceptBtn(false);
                    TutorialUI.HideOldMonk(false);
                    break;
            }
        }
        else
        {
            CheckIfPlayerWon();
        }

        CheckIfStillWinning();
    }

    protected override void CheckIfPlayerWon()
    {
        if (templeReadiness >= templeReadinessToAchieve)
        {
            sceneController.LoadScene(SceneList.Level01);
            musicSFXManager.ChangeMusic(ChooseMusic.VictoryMusic);
        }
        else
        {
            uIController.Defeat(DefeatType.ReadinessLoss);
            musicSFXManager.ChangeMusic(ChooseMusic.DefeatMusic);
        }
    }
}
