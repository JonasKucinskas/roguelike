using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class Card : MonoBehaviour
{
	Vector3 slotCoordinates;
	public bool IsDragging = false;
	private Vector3 objectScreenCoord;
	private Vector3 offset;
	private bool isPlaced = false; // Flag to indicate whether the card is placed
	private Vector3 originalPosition; // Store the original position of the card
	private float startYPosition; // Add this field to track the start Y position of the drag
	private Vector3 originalScale; // To store the original scale
	private Quaternion originalRotation; // To store the original rotation
	private float initialMouseY; // Store initial Y position of the mouse for calculating drag distance
	private float initialZDistance; // Store the initial Z distance from the camera to the object
	private Vector3 cumulativeTranslation = Vector3.zero; // Track cumulative translations
	[SerializeReference]
	private LayerMask IgnoreMe;
	public GameObject CardModel;
	public GameObject Particle;
    private TurnManager turnManager;
	private bool hasMovedAboveThreshold = false;

	public delegate void CardMovedFromHandEventHandler(Card card);
    public event CardMovedFromHandEventHandler OnCardMovedFromHand;

	public delegate void CardMovedToHandEventHandler(Card card);
    public event CardMovedToHandEventHandler OnCardMovedToHand;


	void Awake()
	{
		turnManager = GameObject.Find("TurnManager").GetComponent<TurnManager>();
		objectScreenCoord = Camera.main.WorldToScreenPoint(transform.position);
	}

	void Update()
	{
		if (IsDragging)
		{
			DragCard();

			//Checks if clicked on UI (PauseMenu)
			if (EventSystem.current.IsPointerOverGameObject())
			{
				return;
			}
			// Improved detection of mouse release, even outside the window
			if (!Input.GetMouseButton(0))
			{
				IsDragging = false;
				ResetCardToOriginalState(); // Reset card to original state when mouse is released
			}
		}
	}

	private void OnMouseDown()
	{
		//Checks if clicked on UI (PauseMenu)
		if (EventSystem.current.IsPointerOverGameObject())
		{
			return;
		}
		if (!isPlaced)
		{
			IsDragging = true;

			// Factor in cumulative translations to get the correct original position
			originalPosition = transform.position - cumulativeTranslation;

			originalPosition = transform.position;
			originalScale = transform.localScale;
			originalRotation = transform.rotation;

			objectScreenCoord = Camera.main.WorldToScreenPoint(transform.position);
			offset = transform.position - Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, objectScreenCoord.z));

			startYPosition = Input.mousePosition.y;
			initialMouseY = Input.mousePosition.y; // Store the initial mouse Y position for drag calculations
			initialZDistance = objectScreenCoord.z; // Store the initial Z distance from the camera to the object

			Debug.Log("Card clicked: " + gameObject.name);
		}
	}

	private void TranslateWithTracking()
	{
		Vector3 upTranslation = Vector3.up * 0.01f;
		Vector3 backTranslation = Vector3.back * 0.1f;

		// Apply translations
		transform.Translate(upTranslation);
		transform.Translate(backTranslation);

		// Update cumulative translations
		cumulativeTranslation += upTranslation + backTranslation;
	}

	private void OnMouseUp()
	{
		if (IsDragging)
		{
			hasMovedAboveThreshold = false;
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
					ResetCardToOriginalState();
				}
			}
			else
			{
				ResetCardToOriginalState();
			}
		}
	}

	private void ResetCardToOriginalState()
	{
		// Adjust original position to factor in cumulative translations
		transform.position = originalPosition + cumulativeTranslation;
		OnCardMovedToHand?.Invoke(this);
		// Reset transformations
		transform.localScale = originalScale;
		transform.rotation = originalRotation;

		// Reset cumulative translations
		cumulativeTranslation = Vector3.zero;

		Debug.Log("Card returned to original state.");
	}

	private void DragCard()
	{
		//Checks if clicked on UI (PauseMenu)
		if (EventSystem.current.IsPointerOverGameObject())
		{
			return;
		}
		float currentMouseY = Input.mousePosition.y;
		float deltaY = currentMouseY - initialMouseY; // Calculate the difference in mouseY position from when dragging started

		float zAdjustment = deltaY * 0.01f; // Adjust the depth based on the vertical mouse movement

		// Ensure the card does not get closer to the camera than its original depth
		float adjustedZ = Mathf.Max(initialZDistance, initialZDistance + zAdjustment);

		// Use the adjusted Z for the objectScreenCoord.z value
		objectScreenCoord.z = adjustedZ;

		Vector3 cursorScreenPoint = new Vector3(Input.mousePosition.x, Input.mousePosition.y, objectScreenCoord.z);
		Vector3 cursorPosition = Camera.main.ScreenToWorldPoint(cursorScreenPoint) + offset;

		transform.position = cursorPosition;

		Quaternion targetRotation = Quaternion.Euler(1, originalRotation.eulerAngles.y, 1);
		transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 5);


		if (!hasMovedAboveThreshold)
		{
			OnCardMovedFromHand?.Invoke(this);
			hasMovedAboveThreshold = true;
		}       
	}

	private void AttemptToPlaceCard()
	{
		//Checks if clicked on UI (PauseMenu)
		if (EventSystem.current.IsPointerOverGameObject())
		{
			return;
		}
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

		if (tile.IsOccupied())
		{
			Debug.Log("Cannot place card here, slot is already occupied or has an enemy.");

			// Optionally, return the card to its original position or handle as desired
			transform.position = originalPosition;
			return;
		}

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
        
        NeutrophilCell friendly = character.GetComponent<NeutrophilCell>();
        friendly.characterName = $"Friendly_{tile.xPosition}_{tile.zPosition}";
        friendly.xPosition = tile.xPosition;
        friendly.zPosition = tile.zPosition;
        tile.SetFriendlyPresence(true);
		turnManager.SubtractPlayerMove();
		
		Debug.Log("Card placed on cube.");
		Destroy(gameObject);//this card object won't be on a tile anymore
	}
	
	public void SetSlotCoordinates(Vector3 slot)
	{
		slotCoordinates = slot;
	}

	private void OnMouseEnter()
	{
		//Checks if clicked on UI (PauseMenu)
		if (EventSystem.current.IsPointerOverGameObject())
		{
			return;
		}
		if (isPlaced == false)
		{
			transform.Translate(Vector3.up * 0.01f);
			transform.Translate(Vector3.back * 0.1f);
		}
	}

	private void OnMouseExit()
	{
		//Checks if clicked on UI (PauseMenu)
		if (EventSystem.current.IsPointerOverGameObject())
		{
			return;
		}
		if (isPlaced == false)
		{
			transform.Translate(Vector3.down * 0.01f);
			transform.Translate(Vector3.forward * 0.1f);
		}
	}
}
