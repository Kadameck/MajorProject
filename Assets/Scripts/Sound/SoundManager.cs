using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class SoundManager
{
    // Samlung der namen aller SoundEffekte die es geben soll
    public enum Sound
    {
        Walk
    }

    // Bibliothek die jedem sound einen audioclip zuordnet
    private static Dictionary<Sound, AudioClip> audioClips = new Dictionary<Sound, AudioClip>();
    // Bibliothek die jedem sound einen floatwert zugeordnet hat, welcher im folgenden als timer dient
    private static Dictionary<Sound, float> soundTimers = new Dictionary<Sound, float>();

    // Nimmt einen array aus kombinationen aus effektnahmen und clips entgegen und convertiert dieses in die audioClips bibliothek
    public static void InitAudio(audioCollector[] soundEffects)
    {
        foreach (audioCollector ac in soundEffects)
        {
            audioClips.Add(ac.soundEffect, ac.audioClip);
        }
    }

    /// <summary>
    /// Prueft welcher Soundeffekt gespielt werden soll
    /// </summary>
    /// <param name="soundEffect"></param>
    public static void PlaySound(Sound soundEffect)
    {

        switch (soundEffect)
        {
            case Sound.Walk:
                PlayWalkSoundEffect();
                break;

            default:
                break;
        }
    }

    /// <summary>
    /// Spielt den Walk Soundeffekt, sollte dieser nicht bereits aktiv sein
    /// </summary>
    private static void PlayWalkSoundEffect()
    {
        // Wenn noch kein Timer fuer walk bekannt ist (also noch kein Walk effekt spielt)
        if (!soundTimers.ContainsKey(Sound.Walk))
        {
            // erstelle einen Timer fuer walk und setze ihn auf 0
            soundTimers.Add(Sound.Walk, 0.0f);

            // Erstellte ein Gameobjekt das als sound quelle dient
            GameObject soundObject = new GameObject();
            AudioSource audioSource = soundObject.AddComponent<AudioSource>();
            audioSource.volume = 0.05f;
            // suche in der audioClip bibliothek nach dem passenden clip und spiele diesen exakt 1 mal ab
            audioSource.PlayOneShot(audioClips[Sound.Walk]);
            // Zerstoert das objekt das als audioquelle gesetzt wurde nachdem der soundeffekt beendet wurde
            Object.Destroy(soundObject, audioClips[Sound.Walk].length);
        }
        // Existiert bereits ein Timer fuer walk (also ist momentan ein walk-clip zu hoeren)
        else
        {
            // der timer bei walk wird in dem Dictionary um die deltatime erhöht
            soundTimers[Sound.Walk] += Time.deltaTime;

            // Wenn der clip nur noch 0.1 sek hat bis er zuende ist, soll der timer gelöscht werden
            // weil der clip am ende kurz stille hat wird auf diese weise bereits 0.2 sekunde vor ende das starten den neuen Clips erlaubt
            if (soundTimers[Sound.Walk] > audioClips[Sound.Walk].length - 0.4f)
            {
                // der timer wird gelöscht
                soundTimers.Remove(Sound.Walk);
            }
        }
    }
}

// eine klasse damit ich im inspector (Player.cs) soundeffekte und dazu passende audioclips entgegen nehmen kann, denn dictionarys sind im inspector nicht sichtbar
[System.Serializable]
public struct audioCollector
{
    [Tooltip("Use case of this sound effect")]
    public SoundManager.Sound soundEffect;
    [Tooltip("The audiofile")]
    public AudioClip audioClip;
}
