﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelSountrackManager : MonoBehaviour
{
    [SerializeField]
    AudioClip[] normalForestSoundtracks;
    [SerializeField]
    AudioClip[] winterForestSoundtracks;

    private AudioSource audioSource;
    private float timer;
    private float currentPlayingAudioClipLenght;

    void Start()
    {
        audioSource = this.gameObject.AddComponent<AudioSource>();
        audioSource.volume = .3f;
        audioSource.clip = normalForestSoundtracks[0];
        timer = 0.0f;

        StartCoroutine(PlaySoundtracks());
    }

    private IEnumerator PlaySoundtracks()
    {
        currentPlayingAudioClipLenght = audioSource.clip.length;
        // Startet das abspielen
        audioSource.Play();
        // pausiert den code bis der track durch ist
        while(timer < currentPlayingAudioClipLenght)
        {
            timer += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        
        StartCoroutine(ChangeSoundtrack());
    }

    private IEnumerator ChangeSoundtrack()
    {
        // beendet das abspielen und setzt alle einstellungen zurück
        audioSource.Stop();
        audioSource.clip = null;
        timer = 0.0f;
        // erstellt eine pseudo zufällige zeit, in der kein soundtrack gespielt wird
        float randomRepeatTimer = Random.Range(0, 31);
        yield return new WaitForSeconds(randomRepeatTimer);
        // setzt einen pseudo zufälligen der als normalForestSoundtracks festgelegten soundtrack als neuen track zum spielen
        audioSource.clip = normalForestSoundtracks[Random.Range(0, normalForestSoundtracks.Length)];
        
        StartCoroutine(PlaySoundtracks());
    }
}
