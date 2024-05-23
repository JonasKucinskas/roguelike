using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Bestiary : MonoBehaviour
{
    public GameObject[] entries;
    public GameObject[] levelEntries;
    public Button[] buttons;
    public Button[] levelButtons;
    int isVirusUnlocked = 0;
    int isDendrUnlocked = 0;
    int isNeutrsUnlocked = 0;
    int isTcellUnlocked = 0;
    int isEpidermisUnlocked = 0;


    private void Start()
    {
        LoadPrefs();
        EnableButtons();
        PlayerPrefs.Save();
    }
    public void LoadPrefs()
    {
        isVirusUnlocked = PlayerPrefs.GetInt("IsVirusUnlocked");
        isDendrUnlocked = PlayerPrefs.GetInt("IsDendrUnlocked");
        isNeutrsUnlocked = PlayerPrefs.GetInt("IsNeutrsUnlocked");
        isTcellUnlocked = PlayerPrefs.GetInt("IsTcellUnlocked");
        isEpidermisUnlocked = PlayerPrefs.GetInt("IsEpidermisUnlocked");

        Debug.Log("PlayerPref virus check: " + isVirusUnlocked);
        Debug.Log("PlayerPref dend check: " + isDendrUnlocked);
        Debug.Log("PlayerPref neutr check: " + isNeutrsUnlocked);
        Debug.Log("PlayerPref t check: " + isTcellUnlocked);
        Debug.Log("PlayerPref level check: " + isEpidermisUnlocked);

    }
    public void EnableButtons()
    {
        if(isVirusUnlocked == 1)
        {
            buttons[3].interactable = true;
            buttons[3].GetComponentInChildren<TextMeshProUGUI>().text = "Flu Virus";
        }
        if (isDendrUnlocked == 1)
        {
            buttons[0].interactable = true;
            buttons[0].GetComponentInChildren<TextMeshProUGUI>().text = "Dendritic Cell";
        }
        if (isNeutrsUnlocked == 1)
        {
            buttons[1].interactable = true;
            buttons[1].GetComponentInChildren<TextMeshProUGUI>().text = "Neutrophil";
        }
        if (isTcellUnlocked == 1)
        {
            buttons[2].interactable = true;
            buttons[2].GetComponentInChildren<TextMeshProUGUI>().text = "T Cell";
        }
        if(isEpidermisUnlocked == 1)
        {
            levelButtons[0].interactable = true;
            levelButtons[0].GetComponentInChildren<TextMeshProUGUI>().text = "Epidermis";
        }
    }

    public void EnableEntry(int id)
    {
        for(int i = 0; i < entries.Length; i++)
        {
            entries[i].SetActive(false);
        }
        entries[id].SetActive(true);
    }

    public void EnableLevelEntry(int id)
    {
        for (int i = 0; i < levelEntries.Length; i++)
        {
            levelEntries[i].SetActive(false);
        }
        levelEntries[id].SetActive(true);
    }
}
