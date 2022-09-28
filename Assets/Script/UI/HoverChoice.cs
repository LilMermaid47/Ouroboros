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
    public void OnPointerExit()
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

    public void ShowMessage()
    {
        if (isLeftGong)
            HoverTooltipManager.OnMouseHover(questManager.currentQuest.questDefinition.choice1Description, Input.mousePosition);
        else
            HoverTooltipManager.OnMouseHover(questManager.currentQuest.questDefinition.choice2Description, Input.mousePosition);
    }

    public void ShowImpactOfChoice()
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
        if (reward.yinYangBalance != 0)
            balanceLamp.sprite = lampOn;
    }

    public void TurnLampOff()
    {
        readinessLamp.sprite = lampOff;
        yuanLamp.sprite = lampOff;
        balanceLamp.sprite = lampOff;
    }

    private IEnumerator StartTimer()
    {
        yield return new WaitForSeconds(timeToWait / 2);
        ShowImpactOfChoice();
        yield return new WaitForSeconds(timeToWait / 2);
        ShowMessage();
    }
}
