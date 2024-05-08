using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class SoundManager : MonoBehaviour
{
    public Slider masterVolumeSlider;
    public Slider musicVolumeSlider;
    public Slider sfxVolumeSlider;

    public AudioMixer MasterMixer;

    private float masterVolumeValue;
    private float musicVolumeValue;
    private float sfxVolumeValue;

    private string masterVolume = "MasterVolume";
    private string musicVolume = "MusicVolume";
    private string sfxVolume = "SFXVolume";
    void Start()
    {
        if (masterVolumeSlider != null)
        {
            if (PlayerPrefs.HasKey(masterVolume))
            {
                float savedMasterVolume = PlayerPrefs.GetFloat(masterVolume);
                masterVolumeSlider.value = savedMasterVolume;
                masterVolumeValue = savedMasterVolume;
            }
            else
            {
                masterVolumeSlider.value = 1f;
                masterVolumeValue = 1f;
            }
        }

        if (musicVolumeSlider != null)
        {
            if (PlayerPrefs.HasKey(musicVolume))
            {
                float savedMusicVolume = PlayerPrefs.GetFloat(musicVolume);
                musicVolumeSlider.value = savedMusicVolume;
                musicVolumeValue = savedMusicVolume;
            }
            else
            {
                musicVolumeSlider.value = 1f;
                musicVolumeValue = 1f;
            }
        }

        if (sfxVolumeSlider != null)
        {
            if (PlayerPrefs.HasKey(sfxVolume))
            {
                float savedSFXVolume = PlayerPrefs.GetFloat(sfxVolume);
                sfxVolumeSlider.value = savedSFXVolume;
                sfxVolumeValue = savedSFXVolume;
            }
            else
            {
                sfxVolumeSlider.value = 1f;
                sfxVolumeValue = 1f;
            }
        }
    }

    public void SetMasterVolume()
    {
        masterVolumeValue = masterVolumeSlider.value;
        MasterMixer.SetFloat("MasterVolume", Mathf.Log10(masterVolumeValue) * 20f);

        Debug.Log("Master volume: " + masterVolumeValue);
    }

    public void SetMusicVolume()
    {
        musicVolumeValue = musicVolumeSlider.value;
        MasterMixer.SetFloat(musicVolume, Mathf.Log10(musicVolumeValue) * 20f);

        Debug.Log("Music volume: " + musicVolumeValue);
    }

    public void SetSfxVolume()
    {
        sfxVolumeValue = sfxVolumeSlider.value;
        MasterMixer.SetFloat("SFXVolume", Mathf.Log10(sfxVolumeValue) * 20f);

        Debug.Log("SFX volume: " + sfxVolumeValue);
    }

    public void ApplyVolumeChanges()
    {
        PlayerPrefs.SetFloat(musicVolume, musicVolumeValue);
        PlayerPrefs.SetFloat(masterVolume, masterVolumeValue);
        PlayerPrefs.SetFloat(sfxVolume, sfxVolumeValue);
        PlayerPrefs.Save();
    }
}
