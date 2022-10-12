using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialQuestManager : QuestManager
{
    private void Start()
    {
        uIController = GameObject.FindGameObjectWithTag("Canvas").GetComponent<TutorialUIController>();

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
    protected override void UpdateCurrentQuest()
    {
        currentQuest = filledLevel.questList[currentQuestIndex];

        uIController.SetQuest(currentQuest);
        uIController.SetNbQuestLeft(filledLevel.questList.Count - currentQuestIndex);
        uIController.SetNPCFile(filledLevel, currentQuestIndex);

        if (currentQuest.QuestType() == TypeOfQuest.RequirementQuest)
        {
            RequirementQuest requirementQuest = (RequirementQuest)currentQuest;
            CheckRequirement(requirementQuest);
        }
    }

    protected override void CheckRequirement(RequirementQuest requirementQuest)
    {
        Requirement requirement = requirementQuest.requirementChoice1;

        if ((requirement.templeReadiness <= templeReadiness || requirement.templeReadiness == 0) &&
            requirement.moneyCost <= argent &&
            requirement.kiCost <= ki &&
            requirement.disciples <= disciple)
        {
            uIController.FirstChoiceBtnActivate(true);
        }
        else
        {
            uIController.DisableFirstChoiceBtn(DisableReason(requirement));
        }

        requirement = requirementQuest.requirementChoice2;

        if ((requirement.templeReadiness <= templeReadiness || requirement.templeReadiness == 0) &&
            requirement.moneyCost <= argent &&
            requirement.kiCost <= ki &&
            requirement.disciples <= disciple)
        {
            uIController.SecondChoiceBtnActivate(true);
        }
        else
        {
            uIController.DisableSecondChoiceBtn(DisableReason(requirement));
        }
    }
}
