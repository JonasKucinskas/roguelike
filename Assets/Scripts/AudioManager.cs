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

    [Header("Dendritic sound effects")]
    public AudioClip dendriticIdle1;
    public AudioClip dendriticIdle2;
    public AudioClip dendriticAttack;
    public AudioClip dendriticSpecialAttack;

    [Header("Card sound effects")]
    public AudioClip CardJump;
    public AudioClip CardPickup;
    public IEnumerator PlaySound(AudioClip clip, float delay)
    {
        yield return new WaitForSeconds(delay);
        //Debug.Log("Sound is playing");
        soundEffect.PlayOneShot(clip); //does not cancel clips that are already being played 
    }
}
