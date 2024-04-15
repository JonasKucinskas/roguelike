using UnityEngine;
using UnityEngine.EventSystems;

public class Card : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
	public bool IsDragging = false;
	private bool isPlaced = false; 
	public Vector3 originalPosition; 
	public Vector3 originalScale;
	public Quaternion originalRotation;
	private LayerMask IgnoreMe;
	public GameObject CardModel;
	public GameObject Particle;
    private TurnManager turnManager;
	public delegate void CardMovedFromHandEventHandler(Card card);
    public event CardMovedFromHandEventHandler OnCardMovedFromHand;
	public delegate void CardMovedToHandEventHandler(Card card);
    public event CardMovedToHandEventHandler OnCardMovedToHand;

	void Awake()
	{
		turnManager = GameObject.Find("TurnManager").GetComponent<TurnManager>();
	}

	void Update()
	{
		if (IsDragging)
		{
			Drag();

			
			// Improved detection of mouse release, even outside the window
			if (!Input.GetMouseButton(0))
			{
				IsDragging = false;
				ResetCardToOriginalState(); // Reset card to original state when mouse is released
			}
		}
	}

	private void ResetCardToOriginalState()
	{
		// Reset transformations
		transform.position = originalPosition;
		transform.localScale = originalScale;
		transform.rotation = originalRotation;

		OnCardMovedToHand?.Invoke(this);
		Debug.Log("Card returned to original state.");
	}

	private void Drag()
	{
		transform.position = Input.mousePosition;

		Quaternion targetRotation = Quaternion.Euler(1, 1, 1);
		transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 5);
	}

	private void PlaceCardOnCube(TileScript tile)
	{
		Transform tileTransform = tile.gameObject.transform;

		IsDragging = false;
		isPlaced = true;

		Vector3 modelPosition = new Vector3(tileTransform.position.x, tileTransform.position.y + 0.6f, tileTransform.position.z);
		
		// Instantiate particle effect and character model
        Instantiate(Particle, modelPosition, Quaternion.Euler(0f, 90f, 0f), tileTransform);
        GameObject character = Instantiate(CardModel, modelPosition, Quaternion.Euler(0f, 90f, 0f), tileTransform);
        
        Character friendly = character.GetComponent<Character>();
        friendly.characterName = $"Friendly_{tile.xPosition}_{tile.zPosition}";
        friendly.xPosition = tile.xPosition;
        friendly.zPosition = tile.zPosition;
        tile.SetFriendlyPresence(true);
		turnManager.SubtractPlayerMove();
		
		Debug.Log("Card placed on cube.");
		Destroy(gameObject);//this card object won't be on a tile anymore
	}

    public void OnPointerDown(PointerEventData eventData)
    {
        //Checks if clicked on UI (PauseMenu)
		if (EventSystem.current.IsPointerOverGameObject())
		{
			//return;
		}
		if (!isPlaced)
		{
			IsDragging = true;
			OnCardMovedFromHand?.Invoke(this);
		}
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (IsDragging)
		{
			IsDragging = false;

			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			RaycastHit hit;

			if (Physics.Raycast(ray, out hit, 1000f, ~IgnoreMe))
			{
				TileScript tile = hit.collider.GetComponent<TileScript>();
				if (tile && !tile.IsOccupied())
				{
					PlaceCardOnCube(tile);
					transform.position = originalPosition;
					return;
				}
			}
			ResetCardToOriginalState();
		}
    }
}