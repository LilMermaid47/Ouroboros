using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class HoverChoice : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
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
    }
    private void ShowMessage()
    {
        if(isLeftGong)
            HoverTooltipManager.OnMouseHover(questManager.currentQuest.questDefinition.choice1Description, Input.mousePosition);
        else
            HoverTooltipManager.OnMouseHover(questManager.currentQuest.questDefinition.choice2Description, Input.mousePosition);
    }

    private IEnumerator StartTimer()
    {
        yield return new WaitForSeconds(timeToWait);

        ShowMessage();
    }
}
