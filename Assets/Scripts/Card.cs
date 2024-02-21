using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Card : MonoBehaviour
{
    private bool isDragging = false;
    private Vector3 slotCoordinates;

    Vector3 objectSreenCoord;

    private void OnMouseEnter()
    {
        transform.Translate(Vector3.up);
    }

    private void OnMouseExit()
    {
        transform.Translate(Vector3.down);
    }

    private void Start()
    {
        objectSreenCoord = Camera.main.WorldToScreenPoint(transform.position);
    }

   private void Update()
    {
        if (isDragging == true)
        {
            Vector3 objectCursorMoveScreen = new Vector3(Input.mousePosition.x, Input.mousePosition.y, objectSreenCoord.z);
            Vector3 objectCursorMoveWorld = Camera.main.ScreenToWorldPoint(objectCursorMoveScreen);
            transform.position = objectCursorMoveWorld;
        }

        if (Input.GetMouseButtonDown(1))
        {
            transform.position = slotCoordinates;
            isDragging = false;
        }
    }

    private void OnMouseDown()
    {
        isDragging = true;
    }

    public void SetSlotCoordinates(Vector3 slot)
    {
        slotCoordinates = slot;
    }
}
