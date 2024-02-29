using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Card : MonoBehaviour
{
    Vector3 slotCoordinates;
    public bool IsDragging = false;
    private Vector3 objectScreenCoord;
    private Vector3 offset;
    private bool isPlaced = false; // Flag to indicate whether the card is placed
    private Vector3 originalPosition; // Store the original position of the card
    [SerializeReference]
    private LayerMask IgnoreMe;

    private void Start()
    {
        objectScreenCoord = Camera.main.WorldToScreenPoint(transform.position);
        originalPosition = transform.position; // Initialize original position
    }

    private void Update()
    {
        if (IsDragging)
        {
            DragCard();
            if (Input.GetMouseButtonDown(0))
            {
                AttemptToPlaceCard();
            }
        }
    }

    private void OnMouseDown()
    {
        if (!isPlaced)
        {
            Debug.Log("Card clicked: " + gameObject.name);
            IsDragging = true;
            originalPosition = transform.position; // Save the original position when dragging starts
            objectScreenCoord = Camera.main.WorldToScreenPoint(transform.position);
            offset = transform.position - Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, objectScreenCoord.z));
        }
    }

    private void OnMouseUp()
    {
        if (IsDragging)
        {
            IsDragging = false;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, 1000f, ~IgnoreMe))
            {
                TileScript cubeHighlighter = hit.collider.GetComponent<TileScript>();
                if (cubeHighlighter != null && !TileScript.IsSlotOccupied(hit.transform))
                {
                    PlaceCardOnCube(hit.transform);
                }
                else
                {
                    // If the block is occupied or not a valid placement, return to original position
                    transform.position = originalPosition;
                    Debug.Log("Cannot place card here, slot is already occupied or not valid.");
                }
            }
            else
            {
                // If no valid hit, return to original position
                transform.position = originalPosition;
            }
        }
    }

    private void DragCard()
    {
        Vector3 cursorPoint = new Vector3(Input.mousePosition.x, Input.mousePosition.y, objectScreenCoord.z);
        Vector3 cursorPosition = Camera.main.ScreenToWorldPoint(cursorPoint) + offset;
        transform.position = cursorPosition;
    }

    private void AttemptToPlaceCard()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            TileScript cubeHighlighter = hit.collider.GetComponent<TileScript>();
            if (cubeHighlighter != null && cubeHighlighter.IsHighlighted)
            {
                PlaceCardOnCube(hit.transform);
            }
        }
    }

    private void PlaceCardOnCube(Transform cubeTransform)
    {
        // Check if the slot is already occupied
        if (!TileScript.IsSlotOccupied(cubeTransform))
        {
            transform.position = cubeTransform.position + new Vector3(0.01f, 1f, 0);
            transform.localScale *= 2f;
            transform.SetParent(cubeTransform, worldPositionStays: true);
            IsDragging = false;
            isPlaced = true; // Mark the card as placed

            TileScript.OccupySlot(cubeTransform); // Mark the slot as occupied

            var dragObject = GetComponent<DragObject>();
            if (dragObject != null)
            {
                dragObject.enabled = false;
            }

            Debug.Log("Card placed on cube.");
        }
        else
        {
            // Optionally, add feedback to the player that the slot is occupied
            Debug.Log("Cannot place card here, slot is already occupied.");
        }
    }

    public void SetSlotCoordinates(Vector3 slot)
    {
        slotCoordinates = slot;
    }

    private void OnMouseEnter()
    {
        if(isPlaced==false)
        {
            transform.Translate(Vector3.up*0.01f);
            transform.Translate(Vector3.back*0.1f);            
        }
    }

    private void OnMouseExit()
    {
        if(isPlaced==false)
        {
            transform.Translate(Vector3.down*0.01f);
            transform.Translate(Vector3.forward*0.1f);             
        }
    }
}
