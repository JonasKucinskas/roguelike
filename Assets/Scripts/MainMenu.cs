using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.Audio;
public class MainMenu : MonoBehaviour
{

    public TMP_Dropdown ResolutionDropdown;
    Resolution[] Resolutions;
    int CurrentResolutionIndex = 0;
    public AudioMixer MainMixer;    

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

    public void SetVolume(float volume)
    {
        MainMixer.SetFloat("Volume",volume);
    }
}
