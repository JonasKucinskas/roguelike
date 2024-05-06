using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public static bool IsPaused=false;
    public GameObject PauseMenuUI;
    public GameObject VictoryMenuUI;
    public GameObject DefeatMenuUI;
	public GameObject BonusSelectUI;
    public GameObject OpponentsTurnText;
    private BoardScript boardManager;
    private KeyCode pause = KeyCode.Escape;

    void Start()
    {
        boardManager=GameObject.Find("Board").GetComponent<BoardScript>();
        LoadKeybinds();
    }
	// Update is called once per frame
	void Update()
    {
        if(Input.GetKeyDown(pause))
        {
            if(IsPaused)
            {
                Resume();
            }
            else
            {
                Pause();
            }
        }
    }
    void LoadKeybinds()
    {
        string[] keys = { "Pause" };

        foreach (string key in keys)
        {
            if (PlayerPrefs.HasKey(key))
            {
                switch (key)
                {
                    case "Pause":
                        pause = (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString(key));
                        break;
                }
            }
        }
    }

    public void Resume()
    {
        boardManager.AllowPlayerInput=true;

        PauseMenuUI.SetActive(false);
        Time.timeScale=1f;
        IsPaused=false;
    }
    void Pause()
    {
        boardManager.AllowPlayerInput=false;

        PauseMenuUI.SetActive(true);
        Time.timeScale=0f;
        IsPaused=true;
    }

    public void ReturnToMenu()
    {
        Time.timeScale=1f;
        SceneManager.LoadScene("MainMenu");
    }

    public void RestartLevel()
    {
        SceneManager.LoadScene("GameScene");
    }
}
