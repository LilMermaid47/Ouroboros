using System;
using UnityEngine;
using UnityEngine.InputSystem.XR;
using UnityEngine.UI;

[RequireComponent(typeof(SoundManager))]
public class TutorialUIController : UIController
{

    [SerializeField]
    private GameObject HuangseiFlag;
    [SerializeField]
    private GameObject SusodaFlag;
    [SerializeField]
    private GameObject OldMonk;

    private void Awake()
    {
        soundManager = gameObject.GetComponent<SoundManager>();
        musicSFXManager = GameObject.FindGameObjectWithTag("Music").GetComponent<MusicSFXManager>();

        UIMenu.QuestUI.SetActive(false);
        UIMenu.VictoryUI.SetActive(false);
        UIMenu.DefeatUI.SetActive(false);
        UIMenu.InGameMenuUI.SetActive(false);

        HideShopBtn(true);

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
        HideRessources(true);
        HideOldMonk(true);
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

    public void HideOldMonk(bool status)
    {
        OldMonk.SetActive(!status);
    }

    public void HideRessources(bool status)
    {
        HideFlags(status);
        foreach (Ressources item in Enum.GetValues(typeof(Ressources)))
        {
            HideRessources(item, status);
        }
    }

    public void HideShopBtn(bool status)
    {
        BtnQuest.ShopButton.gameObject.SetActive(!status);
    }

    public void HideFlags(bool status)
    {
        ShowHuangseiFlag(!status);
        ShowSusodaFlag(!status);
    }

    public void ShowHuangseiFlag(bool status)
    {
        HuangseiFlag.SetActive(status);
    }
    public void ShowSusodaFlag(bool status)
    {
        SusodaFlag.SetActive(status);
    }

    public void HideRessources(Ressources ressources, bool status)
    {
        switch (ressources)
        {
            case Ressources.Audience:
                TextRessources.nbQuestLeft.gameObject.SetActive(!status);
                break;
            case Ressources.Balance:
                TextRessources.BalanceClanSusoda.HideBar(!status);
                break;
            case Ressources.Disciple:
                TextRessources.Disciple.transform.parent.gameObject.SetActive(!status);
                break;
            case Ressources.Readiness:
                TextRessources.TempleReadiness.transform.parent.gameObject.SetActive(!status);
                break;
            case Ressources.Yuan:
                TextRessources.Yuan.transform.parent.gameObject.SetActive(!status);
                break;
        }
    }
}

public enum Ressources
{
    Audience,
    Balance,
    Disciple,
    Readiness,
    Yuan
}