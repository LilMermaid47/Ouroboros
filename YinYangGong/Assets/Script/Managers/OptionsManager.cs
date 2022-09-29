using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class OptionsManager : MonoBehaviour
{
    [Header("Valeur de départ")]
    [Range(0, 1)] public float Volume = 1f;

    [SerializeField]
    private GameObject FirstMainMenuChoice;    
    [SerializeField]
    private GameObject FirstOptionMenuChoice;

    [SerializeField]
    private TextMeshProUGUI VolumeText;
    [SerializeField]
    private ClanColors CouleurClan;

    [SerializeField]
    private List<BlindnessTypes> ListBlindness;

    private Slider VolumeSlider;
    private TMP_Dropdown BlindessDropdown;

    private AudioSource[] AllAudioSources;

    private PersistantData SavedData;
    private float CurrentVolume;
    private BlindnessTypes SelectedBlindness;

    private void Awake()
    {
        VolumeSlider = GetComponentInChildren<Slider>();
        BlindessDropdown = GetComponentInChildren<TMP_Dropdown>();
        
        AllAudioSources = FindObjectsOfType(typeof(AudioSource)) as AudioSource[];

        SetBlindnessOptions();

        CurrentVolume = Volume;

        SelectedBlindness = ListBlindness[0];
    }

    private void Start()
    {
        SavedData = PersistantData.DataPerserver;

        SetSavedDataValues();

        OpenOptionMenu(false);
    }

    private void OnEnable()
    {
        EventSystem.current.SetSelectedGameObject(FirstOptionMenuChoice);

        VolumeSlider.onValueChanged.AddListener((valueVolume) =>
        {
            SetVolume(valueVolume);
        });

        BlindessDropdown.onValueChanged.AddListener((valueBlindness) =>
        {
            SetBlindnessColor(valueBlindness);
        });
    }

    private void OnDisable()
    {
        EventSystem.current.SetSelectedGameObject(FirstMainMenuChoice);
        VolumeSlider.onValueChanged.RemoveAllListeners();
        BlindessDropdown.onValueChanged.RemoveAllListeners();
    }

    private void OnDestroy()
    {
        VolumeSlider.onValueChanged.RemoveAllListeners();
        BlindessDropdown.onValueChanged.RemoveAllListeners();
    }

    public void OpenOptionMenu(bool status)
    {
        if(!status)
        {
            SavedData.SetCurrentVolume(CurrentVolume);
            SavedData.SetBlindnessSelected(SelectedBlindness);
        }

        gameObject.SetActive(status);
    }

    public void ResetDefaultValue()
    {
        SetVolume(Volume);
        VolumeSlider.value = Volume;
        CurrentVolume = Volume;
        
        SelectedBlindness = ListBlindness[0];
        BlindessDropdown.value = 0;
        SetBlindnessColor(0);
    }

    private void SetSavedDataValues()
    {
        int position = 0;
        float volume = SavedData.GetCurrentVolume();
        BlindnessTypes savedBlindnessSelected = SavedData.GetBlindnessSelected();

        if (savedBlindnessSelected != null)
        {
            SelectedBlindness = SavedData.GetBlindnessSelected();
            position = FindBlindnessValue(SelectedBlindness);
        }

        BlindessDropdown.value = position;
        SetBlindnessColor(position);


        SetVolume(volume);
        VolumeSlider.value = volume;
        CurrentVolume = volume;
    }

    private void SetVolume(float value)
    {
        CurrentVolume = value;
        VolumeText.text = $"{value * 100:0}%";

        foreach (AudioSource audio in AllAudioSources)
        {
            audio.volume = value;
        }
    }

    private void SetBlindnessColor(int value)
    {
        SelectedBlindness = ListBlindness[value];

        CouleurClan.HuangseiPrimaryColor.color = ListBlindness[value].HuangseiPrimary;
        CouleurClan.HuangseiSecondaryColor.color = ListBlindness[value].HuangseiSecondary;

        CouleurClan.SusodaPrimaryColor.color = ListBlindness[value].SusodaPrimary;
        CouleurClan.SusodaSecondaryColor.color = ListBlindness[value].SusodaSecondary ;
    }

    private void SetBlindnessOptions()
    {
        List<TMP_Dropdown.OptionData> options = CreateBlindnessOptionsList();

        BlindessDropdown.AddOptions(options);
    }

    private List<TMP_Dropdown.OptionData> CreateBlindnessOptionsList()
    {
        List <TMP_Dropdown.OptionData> options = new List<TMP_Dropdown.OptionData>();
        TMP_Dropdown.OptionData optionData;

        foreach (BlindnessTypes item in ListBlindness)
        {
            optionData = new TMP_Dropdown.OptionData(item.Name);
            options.Add(optionData);
        }

        return options;
    }

    private int FindBlindnessValue(BlindnessTypes blindness)
    {
        return ListBlindness.IndexOf(blindness);
    }
}

[Serializable]
public class ClanColors
{
    public Image HuangseiPrimaryColor;
    public Image HuangseiSecondaryColor;
    public Image SusodaPrimaryColor;
    public Image SusodaSecondaryColor;
}
