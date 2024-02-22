using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        if (!isGlued) // Check if not glued
        {
            offset = gameObject.transform.position - GetMouseWorldPos();
        }
    }

    void OnMouseDrag()
    {
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
