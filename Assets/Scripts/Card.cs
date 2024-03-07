using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using Unity.VisualScripting;
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
    public GameObject CardModel;
    public GameObject Particle;
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
                TileScript tile = hit.collider.GetComponent<TileScript>();
                if (tile != null && !tile.IsOccupied())
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
            TileScript tile = hit.collider.GetComponent<TileScript>();
            if (tile != null && tile.IsHighlighted)
            {
                if (tile.IsEnemyOnTile())
                {
                    Debug.Log("Attempted to place card on a tile with an enemy present.");
                    return; // Early return to prevent placing the card
                }
                PlaceCardOnCube(hit.transform);
            }
        }
    }


    private void PlaceCardOnCube(Transform cubeTransform)
    {
        TileScript tile = cubeTransform.GetComponent<TileScript>();

        if (!tile.IsOccupied())
        {
            transform.position = cubeTransform.position + new Vector3(0.01f, 1f, 0);
            transform.localScale *= 2f;
            transform.SetParent(cubeTransform, worldPositionStays: true);
            IsDragging = false;
            isPlaced = true;
            
            var dragObject = GetComponent<DragObject>();
            if (dragObject != null)
            {
                dragObject.enabled = false;
            }

            // Optionally, deactivate the card and spawn a character model or effect
            this.gameObject.SetActive(false);
            GameObject model = CardModel;
            Vector3 modelPosition = new Vector3(this.gameObject.transform.position.x, this.gameObject.transform.position.y - 0.3f, this.gameObject.transform.position.z);

            // Instantiate particle effect and character model
            Instantiate(Particle, modelPosition, Quaternion.Euler(0f, 90f, 0f), cubeTransform);
            GameObject character = Instantiate(model.gameObject, modelPosition, Quaternion.Euler(0f, 90f, 0f), cubeTransform);
            
            Friendly friendly = character.GetComponent<Friendly>();
            friendly.friendlyName = $"Friendly_{tile.xPosition}_{tile.zPosition}";
            friendly.hp = 10;
            friendly.damage = 10;
            friendly.xPosition = tile.xPosition;
            friendly.zPosition = tile.zPosition;

            tile.SetFriendlyPresence(true);

            Debug.Log("Card placed on cube.");
        }
        else
        {
            Debug.Log("Cannot place card here, slot is already occupied or has an enemy.");

            // Optionally, return the card to its original position or handle as desired
            transform.position = originalPosition;
        }
    }


    private void SpawnCardModel()
    {

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
