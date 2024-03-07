using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DragObject : MonoBehaviour
{
    private Vector3 offset;
    public bool isGlued = false; // Flag to control drag functionality

    void Start()
    {
        var rb = gameObject.AddComponent<Rigidbody>();
        rb.isKinematic = true;
        rb.useGravity = false;
    }

    void OnMouseDown()
    {
        //Checks if clicked on UI (PauseMenu)
        if(EventSystem.current.IsPointerOverGameObject())
        {
            return;
        }
        if (!isGlued) // Check if not glued
        {
            offset = gameObject.transform.position - GetMouseWorldPos();
        }
    }

    void OnMouseDrag()
    {
        //Checks if clicked on UI (PauseMenu)
        if(EventSystem.current.IsPointerOverGameObject())
        {
            return;
        }
        if (!isGlued) // Check if not glued
        {
            transform.position = GetMouseWorldPos() + offset;
        }
    }

    private Vector3 GetMouseWorldPos()
    {
        Vector3 mousePoint = Input.mousePosition;
        mousePoint.z = Camera.main.WorldToScreenPoint(gameObject.transform.position).z;
        return Camera.main.ScreenToWorldPoint(mousePoint);
    }
}
