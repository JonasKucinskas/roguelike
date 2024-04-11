using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.Audio;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{

    public TMP_Dropdown ResolutionDropdown;
    Resolution[] Resolutions;
    int CurrentResolutionIndex = 0;
    public AudioMixer MainMixer;

    public Slider MasterSlider;
    public Slider MusicSlider;
    public Slider SFXSlider;

    void Start()
    {
        Resolutions=Screen.resolutions;
        ResolutionDropdown.ClearOptions();

        List<string> ScreenSizes =new List<string>();
        for(int i=0; i<Resolutions.Length; i++)
        {
            string Option = Resolutions[i].width + " x " + Resolutions[i].height;
            ScreenSizes.Add(Option);

            if(Resolutions[i].width == Screen.currentResolution.width &&
                Resolutions[i].height == Screen.currentResolution.height)
            {
                CurrentResolutionIndex = i;
            }
        }
        ResolutionDropdown.AddOptions(ScreenSizes);
        ResolutionDropdown.value=CurrentResolutionIndex;
        ResolutionDropdown.RefreshShownValue();
        SetMusicValueFromPP();
    }

    public void PlayGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
    public void EndGame()
    {
        Debug.Log("App quit");
        Application.Quit();
    }

    public void SetFullscreen(bool IsFullscreen)
    {
        Debug.Log("Changed fullscreen");
        Screen.fullScreen=IsFullscreen;
    }

    public void SetResolution(int ResolutionIndex)
    {
        Resolution resolution = Resolutions[ResolutionIndex];
        Screen.SetResolution(resolution.width,resolution.height, Screen.fullScreen);
    }

    private void SetMusicValueFromPP()
	{
        float Master = 1;
        if (PlayerPrefs.HasKey("MasterVolumeValue"))
        {
            Master = PlayerPrefs.GetFloat("MasterVolumeValue");
        }
        float SFX = 1;
        if (PlayerPrefs.HasKey("SFXVolumeValue"))
        {
            SFX = PlayerPrefs.GetFloat("SFXVolumeValue");
        }
        float Music = 1;
        if (PlayerPrefs.HasKey("MusicVolumeValue"))
        {
            Music = PlayerPrefs.GetFloat("MusicVolumeValue");
        }
        SetVolume(Master);
        SetVolumeSFX(SFX);
        SetVolumeMusic(Music);
    }
    public void SetSliderValueFromPP()
    {
        float Master = 1;
        if (PlayerPrefs.HasKey("MasterVolumeValue"))
        {
            Master = PlayerPrefs.GetFloat("MasterVolumeValue");
        }
        float SFX = 1;
        if (PlayerPrefs.HasKey("SFXVolumeValue"))
        {
            SFX = PlayerPrefs.GetFloat("SFXVolumeValue");
        }
        float Music = 1;
        if (PlayerPrefs.HasKey("MusicVolumeValue"))
        {
            Music = PlayerPrefs.GetFloat("MusicVolumeValue");
        }
        MasterSlider.value = Master;
        SFXSlider.value = SFX;
        MusicSlider.value = Music;
    }
    public void SetVolume(float volume)
    {
        MainMixer.SetFloat("Volume", Mathf.Log10(volume) * 20f);
        PlayerPrefs.SetFloat("MasterVolumeValue", volume);
    }
    public void SetVolumeSFX(float volume)
    {
        MainMixer.SetFloat("SFX", Mathf.Log10(volume) * 20f);
        PlayerPrefs.SetFloat("SFXVolumeValue", volume);
    }
    public void SetVolumeMusic(float volume)
    {
        MainMixer.SetFloat("Music", Mathf.Log10(volume) * 20f);
        PlayerPrefs.SetFloat("MusicVolumeValue", volume);
    }
}
