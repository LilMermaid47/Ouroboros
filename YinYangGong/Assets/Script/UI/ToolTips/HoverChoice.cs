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
    public Image disciplesLamp;
    public Image kiLamp;

    [Header("Sprite of lamps")]
    public Sprite lampOff;
    public Sprite lampOn;

    [Header("Other values")]
    public QuestManager questManager;
    public bool isLeftGong;
    private float timeToWait = 0.1f;

    public bool disableOnHover = false;
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!disableOnHover)
        {
            StopAllCoroutines();
            StartCoroutine(StartTimer());
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (!disableOnHover)
        {
            StopAllCoroutines();
            HoverTooltipManager.OnMouseNotHover();
            TurnLampOff();
        }
    }
    public void OnPointerExit()
    {
        if (!disableOnHover)
        {
            StopAllCoroutines();
            HoverTooltipManager.OnMouseNotHover();
            TurnLampOff();
        }
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
        if (reward.nbDisciple !=0)
            disciplesLamp.sprite = lampOn;
        if (reward.nbKi != 0)
            kiLamp.sprite = lampOn;
    }

    public void TurnLampOff()
    {
        readinessLamp.sprite = lampOff;
        yuanLamp.sprite = lampOff;
        balanceLamp.sprite = lampOff;
        disciplesLamp.sprite = lampOff;
        kiLamp.sprite = lampOff;
    }

    private IEnumerator StartTimer()
    {
        yield return new WaitForSeconds(timeToWait);
        ShowImpactOfChoice();
    }
}
