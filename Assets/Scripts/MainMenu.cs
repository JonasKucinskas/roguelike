using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.Audio;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    public GameObject SuggestTutorialUI;
    public TMP_Dropdown ResolutionDropdown;
    Resolution[] Resolutions;
    int CurrentResolutionIndex = 0;
    List<Resolution> UniqueResolutionsList = new List<Resolution>();

    void Start()
    {
        Resolutions = Screen.resolutions;
        ResolutionDropdown.ClearOptions();

        HashSet<string> UniqueResolutions = new HashSet<string>();
        List<string> ScreenSizes = new List<string>();

        for (int i = 0; i < Resolutions.Length; i++)
        {
            string Option = Resolutions[i].width + " x " + Resolutions[i].height;

            if (UniqueResolutions.Add(Option))
            {
                ScreenSizes.Add(Option);
                UniqueResolutionsList.Add(Resolutions[i]);
            }

            if (Resolutions[i].width == Screen.currentResolution.width &&
                Resolutions[i].height == Screen.currentResolution.height)
            {
                CurrentResolutionIndex = UniqueResolutionsList.Count - 1;
            }
        }

        ResolutionDropdown.AddOptions(ScreenSizes);
        ResolutionDropdown.value = CurrentResolutionIndex;
        ResolutionDropdown.RefreshShownValue();
    }

    public void SuggestTutorial()
    {
        if (PlayerPrefs.GetInt("SuggestTutorial", 1) == 1)
        {
            GameObject.Find("MainMenu").SetActive(false);
            SuggestTutorialUI.SetActive(true);
        }
        else
        {
            PlayGame();
        }
    }

    public void TurnSuggestionOn(bool value)
    {
        if (value)
        {
            PlayerPrefs.SetInt("SuggestTutorial", 1);
        }
        else
        {
            PlayerPrefs.SetInt("SuggestTutorial", 0);
        }
    }

    public void PlayGame()
    {
        SceneManager.LoadScene("GameScene");
    }

    public void PlayTutorial()
    {
        SceneManager.LoadScene("Tutorial");
    }

    public void EndGame()
    {
        Debug.Log("App quit");
        Application.Quit();
    }

    public void SetFullscreen(bool IsFullscreen)
    {
        Debug.Log("Changed fullscreen");
        Screen.fullScreen = IsFullscreen;
    }

    public void SetResolution(int ResolutionIndex)
    {
        Resolution resolution = UniqueResolutionsList[ResolutionIndex];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
    }
}
