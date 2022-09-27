using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class HoverChoice : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [Header("Image to put lamps in")]
    public Image yuanLamp;
    public Image balanceLamp;
    public Image readinessLamp;

    public Image huangseiDiscipleLamp;
    public Image susodaDiscipleLamp;

    [Header("Sprite of lamps")]
    public Sprite lampOff;
    public Sprite lampOn;

    [Header("Other values")]
    public QuestManager questManager;
    public bool isLeftGong;
    private float timeToWait = 0.3f;

    public void OnPointerEnter(PointerEventData eventData)
    {
        StopAllCoroutines();
        StartCoroutine(StartTimer());
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        StopAllCoroutines();
        HoverTooltipManager.OnMouseNotHover();
        TurnLampOff();
    }

    public void ChoiceWasMade()
    {
        StopAllCoroutines();
        HoverTooltipManager.OnMouseNotHover();
        TurnLampOff();
    }

    private void ShowMessage()
    {
        if (isLeftGong)
            HoverTooltipManager.OnMouseHover(questManager.currentQuest.questDefinition.choice1Description, Input.mousePosition);
        else
            HoverTooltipManager.OnMouseHover(questManager.currentQuest.questDefinition.choice2Description, Input.mousePosition);
    }

    private void ShowImpactOfChoice()
    {
        Reward reward;
        if (isLeftGong)
            reward = questManager.currentQuest.questDefinition.rewardChoice1;
        else
            reward = questManager.currentQuest.questDefinition.rewardChoice2;

        if (reward.templeReadiness != 0)
            readinessLamp.sprite = lampOn;
        if (reward.moneyReward != 0)
            yuanLamp.sprite = lampOn;
        if (reward.recompenseClanSusoda.discple != 0)
            susodaDiscipleLamp.sprite = lampOn;
        if (reward.recompenseClanHuangsei.discple != 0)
            huangseiDiscipleLamp.sprite = lampOn;

        if (reward.recompenseClanSusoda.devotion != 0 || reward.recompenseClanSusoda.honor != 0
            || reward.recompenseClanHuangsei.devotion != 0 || reward.recompenseClanHuangsei.honor != 0)
            balanceLamp.sprite = lampOn;
    }

    private void TurnLampOff()
    {
        readinessLamp.sprite = lampOff;
        yuanLamp.sprite = lampOff;
        balanceLamp.sprite = lampOff;

        susodaDiscipleLamp.sprite = lampOff;
        huangseiDiscipleLamp.sprite = lampOff;
    }

    private IEnumerator StartTimer()
    {
        yield return new WaitForSeconds(timeToWait / 2);
        ShowImpactOfChoice();
        yield return new WaitForSeconds(timeToWait / 2);
        ShowMessage();
    }
}
