using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class KeyBindManager : MonoBehaviour
{
    public Dictionary<string, KeyCode> keys = new Dictionary<string, KeyCode>();
    public Dictionary<string, KeyCode> defaultKeys = new Dictionary<string, KeyCode>();
    public TMPro.TMP_Text rotateCameraLeft, rotateCameraRight, pause, specialAttack;
    private GameObject currentKey;
    public Button backButton;
    void Start()
    {
        keys.Add("RotateCameraLeft", (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("RotateCameraLeft", "Q")));
        keys.Add("RotateCameraRight", (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("RotateCameraRight", "E")));
        keys.Add("Pause", (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("Pause", "Escape")));
        keys.Add("SpecialAttack", (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString("SpecialAttack", "Space")));

        defaultKeys.Add("RotateCameraLeft", KeyCode.Q);
        defaultKeys.Add("RotateCameraRight", KeyCode.E);
        defaultKeys.Add("Pause", KeyCode.Escape);
        defaultKeys.Add("SpecialAttack", KeyCode.Space);

        UiUpdate();
    }

    void UiUpdate()
    {
        rotateCameraLeft.text = keys["RotateCameraLeft"].ToString();
        rotateCameraRight.text = keys["RotateCameraRight"].ToString();
        pause.text = keys["Pause"].ToString();
        specialAttack.text = keys["SpecialAttack"].ToString();
    }

    void OnGUI()
    {
        if (currentKey != null)
        {
            Event e = Event.current;
            if (e.isKey)
            {
                keys[currentKey.name] = e.keyCode;
                if (!IsKeyAssigned(e.keyCode))
                {
                    currentKey.transform.GetChild(0).GetComponent<TMPro.TMP_Text>().text = e.keyCode.ToString();
                    currentKey.transform.GetChild(0).GetComponent<TMPro.TMP_Text>().color = Color.black;
                    CheckForDuplicates();
                }
                else
                {
                    currentKey.transform.GetChild(0).GetComponent<TMPro.TMP_Text>().text = e.keyCode.ToString();
                    currentKey.transform.GetChild(0).GetComponent<TMPro.TMP_Text>().color = Color.red;
                    CheckForDuplicates();
                }
                currentKey = null;

            }
            else if (e.isMouse)
            {
                if (Input.GetMouseButtonDown(0))
                {
                    keys[currentKey.name] = KeyCode.Mouse0;
                    if (!IsKeyAssigned(KeyCode.Mouse0))
                    {
                        Debug.Log("Left mouse button was pressed");
                        currentKey.transform.GetChild(0).GetComponent<TMPro.TMP_Text>().text = KeyCode.Mouse0.ToString();
                        currentKey.transform.GetChild(0).GetComponent<TMPro.TMP_Text>().color = Color.black;
                        currentKey = null;
                        CheckForDuplicates();
                    }
                    else
                    {
                        Debug.Log("Left mouse button was pressed");
                        currentKey.transform.GetChild(0).GetComponent<TMPro.TMP_Text>().text = KeyCode.Mouse0.ToString();
                        currentKey.transform.GetChild(0).GetComponent<TMPro.TMP_Text>().color = Color.red;
                        currentKey = null;
                        CheckForDuplicates();
                    }

                }

                if (Input.GetMouseButtonDown(1))
                {
                    keys[currentKey.name] = KeyCode.Mouse1;
                    if (!IsKeyAssigned(KeyCode.Mouse1))
                    {
                        Debug.Log("Right mouse button was pressed");
                        currentKey.transform.GetChild(0).GetComponent<TMPro.TMP_Text>().text = KeyCode.Mouse1.ToString();
                        currentKey.transform.GetChild(0).GetComponent<TMPro.TMP_Text>().color = Color.black;
                        currentKey = null;
                        CheckForDuplicates();

                    }
                    else
                    {
                        Debug.Log("Right mouse button was pressed");
                        currentKey.transform.GetChild(0).GetComponent<TMPro.TMP_Text>().text = KeyCode.Mouse1.ToString();
                        currentKey.transform.GetChild(0).GetComponent<TMPro.TMP_Text>().color = Color.red;
                        currentKey = null;
                        CheckForDuplicates();
                    }

                }

                if (Input.GetMouseButtonDown(2))
                {
                    keys[currentKey.name] = KeyCode.Mouse2;
                    if (!IsKeyAssigned(KeyCode.Mouse2))
                    {
                        Debug.Log("Middle mouse button was pressed");

                        currentKey.transform.GetChild(0).GetComponent<TMPro.TMP_Text>().text = KeyCode.Mouse2.ToString();
                        currentKey.transform.GetChild(0).GetComponent<TMPro.TMP_Text>().color = Color.black;
                        currentKey = null;
                        CheckForDuplicates();
                    }
                    else
                    {
                        Debug.Log("Middle mouse button was pressed");
                        currentKey.transform.GetChild(0).GetComponent<TMPro.TMP_Text>().text = KeyCode.Mouse2.ToString();
                        currentKey.transform.GetChild(0).GetComponent<TMPro.TMP_Text>().color = Color.red;
                        currentKey = null;
                        CheckForDuplicates();
                    }

                }
            }
        }
    }
    void CheckForDuplicates()
    {
        bool duplicatesExist = false;
        Dictionary<KeyCode, int> keyCodeCounts = new Dictionary<KeyCode, int>();
        TMPro.TMP_Text[] textVariables = { rotateCameraLeft, rotateCameraRight, pause, specialAttack };

        foreach (var textVar in textVariables)
        {
            KeyCode keyCode;
            if (System.Enum.TryParse(textVar.text, out keyCode))
            {
                if (keyCodeCounts.ContainsKey(keyCode))
                    keyCodeCounts[keyCode]++;
                else
                {
                    keyCodeCounts[keyCode] = 1;
                }
            }
            else
            {
                Debug.LogError("Failed to parse KeyCode from text: " + textVar.text);
            }
        }

        foreach (var textVar in textVariables)
        {
            KeyCode keyCode;
            if (System.Enum.TryParse(textVar.text, out keyCode))
            {
                if (keyCodeCounts.ContainsKey(keyCode))
                {
                    if (keyCodeCounts[keyCode] == 1)
                        textVar.color = Color.black;
                    else
                    {
                        textVar.color = Color.red;
                        duplicatesExist = true;
                    }

                }
            }
            else
            {
                Debug.LogError("Failed to parse KeyCode from text: " + textVar.text);
            }
        }

        backButton.interactable = !duplicatesExist;
    }
    bool IsKeyAssigned(KeyCode keyCode)
    {
        int count = 0;
        foreach (var key in keys.Values)
        {
            if (key == keyCode)
            {
                count++;
            }
        }
        return count > 1;
    }

    public void ChangeKey(GameObject clicked)
    {
        if (currentKey != null)
        {
            currentKey.transform.GetChild(0).GetComponent<TMPro.TMP_Text>().color = Color.black;
        }
        currentKey = clicked;
        currentKey.transform.GetChild(0).GetComponent<TMPro.TMP_Text>().color = Color.white;
    }

    public void SaveKeys()
    {
        foreach (var key in keys)
        {
            PlayerPrefs.SetString(key.Key, key.Value.ToString());
        }
        PlayerPrefs.Save();
    }

    public void ResetKeys()
    {
        foreach (var def in defaultKeys)
        {
            keys[def.Key] = def.Value;
        }
        UiUpdate();
        CheckForDuplicates();
    }
}
