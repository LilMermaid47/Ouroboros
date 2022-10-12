using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

[RequireComponent(typeof(SoundManager))]
public class TutorialUIController : UIController
{

    private void Awake()
    {
        soundManager = gameObject.GetComponent<SoundManager>();

        UIMenu.QuestUI.SetActive(false);
        UIMenu.VictoryUI.SetActive(false);
        UIMenu.DefeatUI.SetActive(false);
        UIMenu.InGameMenuUI.SetActive(false);

        if (UIMenu.GymMenu != null)
            UIMenu.GymMenu.SetActive(false);

        if (NPCQueue != null)
            NPCFileImage = NPCQueue.GetComponentsInChildren<Image>();
    }

    private void Start()
    {
        PlayerInputs = new Ouroboros();

        PlayerInputs.InGameMenu.Pause.performed += InGameMenuPressed;
        PlayerInputs.QuestChoice.Accepter.performed += AccepterChoice;
        questManager = GameObject.FindGameObjectWithTag("QuestManager").GetComponent<TutorialQuestManager>();
        PauseMenuActive(true);
        HideBtn();
    }

    public override void SetNPCFile(Level niveau, int questIndex)
    {
        Sprite questGiverSprite;
        int NPCLeft = NPCFileImage.Length;

        for (int i = 0; NPCLeft >= 0 && i < NPCFileImage.Length; i++)
        {
            NPCLeft--;

            if (questIndex + i < niveau.questList.Count && (questIndex > 4 || i == 0))
            {
                questGiverSprite = niveau.questList[questIndex + i].questDefinition.questGiverSprite;

                if (questGiverSprite != null)
                {
                    HideSprite(NPCFileImage[NPCLeft], false);
                    NPCFileImage[NPCLeft].sprite = questGiverSprite;
                }
                else
                    HideSprite(NPCFileImage[NPCLeft], true);

            }
            else
                HideSprite(NPCFileImage[NPCLeft], true);
        }
    }

}