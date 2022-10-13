using System;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class MusicSFXManager : MonoBehaviour
{
    [SerializeField]
    private AudioSource MusicSource;
    [SerializeField]
    private AudioSource SFXSource;
    [SerializeField]
    private Music MusicClip;
    [SerializeField]
    private float FadeInOut = 0.5f;

    public int MaxVolume = 1;

    public void ChangeMusic(ChooseMusic musicChoice)
    {
        AudioClip clip = null;

        switch (musicChoice)
        {
            case ChooseMusic.NormalMusic:
                clip = MusicClip.normalMusic;
                break;
            case ChooseMusic.HuangseiMusic:
                clip = MusicClip.HuangseiMusic;
                break;
            case ChooseMusic.SusodaMusic:
                clip = MusicClip.SusodaMusic;
                break;
            case ChooseMusic.VictoryMusic:
                clip = MusicClip.VictoryMusic;
                break;
            case ChooseMusic.DefeatMusic:
                clip = MusicClip.DefeatMusic;
                break;
        }

        if (clip != null && MusicSource.clip != clip)
            StartCoroutine(MusicFadeInOut(clip));
    }

    private IEnumerator MusicFadeInOut(AudioClip audio)
    {

        for (float i = MusicSource.volume; i > 0; i -= 0.1f)
        {
            yield return new WaitForSeconds(FadeInOut);
            MusicSource.volume = i;
        }

        MusicSource.clip = audio;
        MusicSource.Play();

        for (float i = 0; i < MaxVolume; i += 0.1f)
        {
            yield return new WaitForSeconds(FadeInOut);
            MusicSource.volume = i;
        }
    }

    public void ChangeSFX(AudioClip sfx)
    {
        if (sfx != null)
        {
            SFXSource.clip = sfx;
            SFXSource.Play();
        }
    }

    public void SFXLoop(bool loop)
    {
        SFXSource.loop = loop;
    }
}

[Serializable]
public class Music
{
    public AudioClip normalMusic;
    public AudioClip HuangseiMusic;
    public AudioClip SusodaMusic;
    public AudioClip VictoryMusic;
    public AudioClip DefeatMusic;
}

public enum ChooseMusic
{
    NormalMusic,
    HuangseiMusic,
    SusodaMusic,
    VictoryMusic,
    DefeatMusic
}
