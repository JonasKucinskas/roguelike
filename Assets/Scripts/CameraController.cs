using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float speed;
    public float minYRotation = 180f;
    public float maxYRotation = 270f;

    private KeyCode rotateCameraLeft = KeyCode.Q;
    private KeyCode rotateCameraRight = KeyCode.E;

    private void Start()
    {
        LoadKeybinds();
    }
    void Update()
    {
        if(Input.GetKey(rotateCameraLeft))
        {
            transform.Rotate(0, speed * Time.deltaTime, 0);
        }
        
        if(Input.GetKey(rotateCameraRight))
        {
            transform.Rotate(0, -speed * Time.deltaTime, 0);
        }

        Vector3 currentRotation = transform.localRotation.eulerAngles;
        float clampedYRotation = Mathf.Clamp(currentRotation.y, minYRotation, maxYRotation);
        transform.localRotation = Quaternion.Euler(currentRotation.x, clampedYRotation, currentRotation.z);
    }
    void LoadKeybinds()
    {
        string[] keys = { "RotateCameraLeft", "RotateCameraRight"};

        foreach (string key in keys)
        {
            if (PlayerPrefs.HasKey(key))
            {
                switch (key)
                {
                    case "RotateCameraLeft":
                        rotateCameraLeft = (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString(key));
                        break;
                    case "RotateCameraRight":
                        rotateCameraRight = (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString(key));
                        break;
                }
            }
        }
    }
}
