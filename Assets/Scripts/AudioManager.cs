using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [SerializeField]
    private AudioSource soundEffect;

    [Header("General sound effects")]
    public AudioClip moving;

    [Header("Virus sound effects")]
    public AudioClip virusIdle1;
    public AudioClip virusIdle2;

    public AudioClip virusAttack;

    [Header("Neutrophil sound effects")]
    public AudioClip neutrophilIdle1;
    public AudioClip neutrophilIdle2;
    public AudioClip neutrophilAttack;
    public AudioClip neutrophilSpecialAttack;

    public IEnumerator PlaySound(AudioClip clip, float delay)
    {
        yield return new WaitForSeconds(delay);
        Debug.Log("Sound is playing");
        soundEffect.PlayOneShot(clip); //does not cancel clips that are already being played 
    }
}
