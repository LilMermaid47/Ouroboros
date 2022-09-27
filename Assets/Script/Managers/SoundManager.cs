using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class SoundManager : MonoBehaviour
{
    [Header("Money sounds effects")]
    public AudioClip moneySoundStart;
    public AudioClip[] moneySoundsRandom;


    private AudioSource m_AudioSource;
    // Start is called before the first frame update
    void Start()
    {
        m_AudioSource = GetComponent<AudioSource>();
        m_AudioSource.playOnAwake = false;
        m_AudioSource.loop = false;
    }

    public void MoneyIsGoingUpFor(float timer)
    {
        StartCoroutine(MoneyGoingUpSfx(timer));
    }

    private IEnumerator MoneyGoingUpSfx(float timer)
    {
        AudioClip currentClip = moneySoundStart;

        int random = UnityEngine.Random.Range(0, moneySoundsRandom.Length);
        int lastRandom = -1;
        while (timer > 0)
        {
            m_AudioSource.clip = currentClip;
            m_AudioSource.Play();
            timer -= currentClip.length;
            yield return new WaitForSeconds(currentClip.length);

            while(random == lastRandom)
            {
                random = UnityEngine.Random.Range(0, moneySoundsRandom.Length);
            }
            lastRandom = random;

            currentClip = moneySoundsRandom[random];
        }
    }
}
