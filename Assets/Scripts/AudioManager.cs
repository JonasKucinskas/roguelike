using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [SerializeField]
    private AudioSource soundEffect;
    [SerializeField]
    private AudioSource backgroundMusic1;
    [SerializeField]
    private AudioSource backgroundMusic2;
    [SerializeField]
    private AudioSource backgroundMusic3;

    [Header("General sound effects")]
    public AudioClip moving;
    public AudioClip spawning;

    [Header("Virus sound effects")]
    public List<AudioClip> virusIdleSounds;
    public AudioClip virusAttack;

    [Header("Neutrophil sound effects")]
    public List<AudioClip> neutrophilIdleSounds;
    public AudioClip neutrophilAttack;
    public AudioClip neutrophilSpecialAttack;

    [Header("Dendritic sound effects")]
    public List<AudioClip> dendriticIdleSounds;
    public AudioClip dendriticAttack;
    public AudioClip dendriticSpecialAttack;

    [Header("Card sound effects")]
    public AudioClip CardJump;
    public AudioClip CardPickup;

    [Header("Scene music")]
    public AudioClip music;
    public AudioClip bubbles;
    public AudioClip heart;

    public IEnumerator PlaySound(AudioClip clip, float delay)
    {
        yield return new WaitForSeconds(delay);
        soundEffect.PlayOneShot(clip); //does not cancel clips that are already being played 
    }

    public List<AudioClip> GetAllVirusIdleSounds()
    {
        return virusIdleSounds;
    }

    public List<AudioClip> GetAllNeutropilIdleSounds()
    {
        return neutrophilIdleSounds;
    }

    public List<AudioClip> GetAllDendriticIdleSounds()
    {
        return dendriticIdleSounds;
    }
}
