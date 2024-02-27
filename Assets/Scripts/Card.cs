using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Card : MonoBehaviour
{
    Vector3 slotCoordinates;
    private bool IsDragging = false;
    private Vector3 objectScreenCoord;
    private Vector3 offset;
    private bool isPlaced = false; // Flag to indicate whether the card is placed

    [SerializeField]
    float cardJumpSize = 6f;


    private void Start()
    {
        objectScreenCoord = Camera.main.WorldToScreenPoint(transform.position);
    }

    private void OnMouseEnter()
    {
        transform.Translate(Vector3.up*cardJumpSize);
    }

    private void OnMouseExit()
    {
        transform.Translate(Vector3.down*cardJumpSize);
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
        // Only allow dragging if the card has not been placed yet
        if (!isPlaced)
        {
            Debug.Log("Card clicked: " + gameObject.name);
            IsDragging = true;
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

            if (Physics.Raycast(ray, out hit))
            {
                TileScript cubeHighlighter = hit.collider.GetComponent<TileScript>();
                if (cubeHighlighter != null && cubeHighlighter.IsHighlighted)
                {
                    PlaceCardOnCube(hit.transform);
                }
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
        transform.position = cubeTransform.position + new Vector3(0.01f, 1f, 0);
        transform.localScale *= 0.05f;
        transform.SetParent(cubeTransform, worldPositionStays: true);
        IsDragging = false;
        isPlaced = true; // Mark the card as placed

        var dragObject = GetComponent<DragObject>();
        if (dragObject != null)
        {
            dragObject.enabled = false;
        }

        Debug.Log("Card placed on cube.");
    }

    public void SetSlotCoordinates(Vector3 slot)
    {
        slotCoordinates = slot;
    }
}
