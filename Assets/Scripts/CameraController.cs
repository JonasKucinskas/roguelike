using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float speed;
    public float minYRotation = 180f;
    public float maxYRotation = 270f;

    void Update()
    {
        if(Input.GetKey(KeyCode.A))
        {
            transform.Rotate(0, speed * Time.deltaTime, 0);
        }
        
        if(Input.GetKey(KeyCode.D))
        {
            transform.Rotate(0, -speed * Time.deltaTime, 0);
        }

        Vector3 currentRotation = transform.localRotation.eulerAngles;
        float clampedYRotation = Mathf.Clamp(currentRotation.y, minYRotation, maxYRotation);
        transform.localRotation = Quaternion.Euler(currentRotation.x, clampedYRotation, currentRotation.z);
    }
}
