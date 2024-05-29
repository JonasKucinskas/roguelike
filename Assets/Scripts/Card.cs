using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public class Card : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
	public bool IsDragging = false;
	private bool isPlaced = false; 
	private bool CollidersOn = true;
	public Vector3 originalPosition; 
	public Vector3 originalScale;
	public Quaternion originalRotation;
	private LayerMask IgnoreMe;
	public GameObject CardModel;
	public GameObject Particle;
    private TurnManager turnManager;
	private BoardScript boardManager;
	private CardManager cardManager;
	public bool cardPlaced = false; //for tutorial use
	public delegate void CardMovedFromHandEventHandler(Card card);
    public event CardMovedFromHandEventHandler OnCardMovedFromHand;
	public delegate void CardMovedToHandEventHandler(Card card);
    public event CardMovedToHandEventHandler OnCardMovedToHand;

	public AudioManager audioManager;

	void Awake()
	{
		turnManager = GameObject.Find("TurnManager").GetComponent<TurnManager>();
		boardManager = GameObject.Find("Board").GetComponent<BoardScript>();
		cardManager = GameObject.Find("CardManager").GetComponent<CardManager>();
		audioManager = GameObject.Find("AudioManager").GetComponent<AudioManager>();
	}

	void Update()
	{
		if(!boardManager.AllowPlayerInput)
		{
			return;
		}

		if (IsDragging)
		{
			//Debug.Log("aaa1	"+transform.gameObject);
			//Debug.Log(transform.gameObject.tag);
			if (transform.gameObject.CompareTag("Card"))
			{
				TileScript.HighlightTilesBasedOnCardPlacable(boardManager);
			}
			else
			{
				TileScript.HighlightAll();
			}
			Drag();
			
			if(CollidersOn)
			{
				ChangeCharacterColliders(false);
				CollidersOn=false;
			}
			
			// Improved detection of mouse release, even outside the window
			if (!Input.GetMouseButton(0))
			{

				IsDragging = false;
				TileScript.ResetTileHighlights();
				ResetCardToOriginalState(); // Reset card to original state when mouse is released
			}
		}
	}

	private void ResetCardToOriginalState()
	{
		ChangeCharacterColliders(true);
		CollidersOn=true;

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

	private IEnumerator PlaceCardOnCube(TileScript tile)
	{
		Transform tileTransform = tile.gameObject.transform;

		IsDragging = false;
		isPlaced = true;
		if (transform.gameObject.CompareTag("Card"))
		{
			boardManager.AllowPlayerInput=false;
			Vector3 modelPosition = new Vector3(tileTransform.position.x, tileTransform.position.y + 0.6f, tileTransform.position.z);
			Vector3 ParticlePosition= new Vector3(modelPosition.x,modelPosition.y+8f, modelPosition.z);
			// Instantiate particle effect and character model
	        Instantiate(Particle, ParticlePosition, Quaternion.Euler(90f, 0f, 0f), tileTransform);
			StartCoroutine(audioManager.PlaySound(audioManager.spawning, 0f));
			yield return new WaitForSeconds(1f);
	        GameObject character = Instantiate(CardModel, modelPosition, Quaternion.Euler(0f, 90f, 0f), tileTransform);
	        
	        Character friendly = character.GetComponent<Character>();
	        friendly.characterName = $"Friendly_{tile.xPosition}_{tile.zPosition}";
	        friendly.xPosition = tile.xPosition;
	        friendly.zPosition = tile.zPosition;
			boardManager.Frendlies.Add(friendly);
	        tile.SetFriendlyPresence(true);
			turnManager.SubtractPlayerMove();
			
			ChangeCharacterColliders(true);
			CollidersOn=true;
			boardManager.AllowPlayerInput=true;
			cardPlaced = true;

			Debug.Log("Card placed on cube.");
		}
		else if (transform.gameObject.CompareTag("EffectCard"))
		{
				turnManager.LowerTemperature(1);
				GameObject cardInfoWindow = GameObject.Find("MenuUI's").transform.Find("TemperatureCardInformation").gameObject;
				cardInfoWindow.SetActive(false);
				Debug.Log("Temperature Lowered");
		}
		Destroy(gameObject);//this card object won't be on a tile anymore
	}

    public void OnPointerDown(PointerEventData eventData)
    {
		if(!boardManager.AllowPlayerInput)
		{
			return;
		}

		if (!isPlaced)
		{
			IsDragging = true;
			OnCardMovedFromHand?.Invoke(this);
		}
    }

    public void OnPointerUp(PointerEventData eventData)
    {
		if(!boardManager.AllowPlayerInput)
		{
			return;
		}
		TileScript.ResetTileHighlights();

        if (IsDragging)
		{
			IsDragging = false;

			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			RaycastHit hit;

			if (Physics.Raycast(ray, out hit, 1000f, ~IgnoreMe))
			{
				TileScript tile = hit.collider.GetComponent<TileScript>();
				if (tile && !tile.IsOccupied() && tile.xPosition < boardManager.GetMaxPlaceableX())
				{
					StartCoroutine(PlaceCardOnCube(tile));
					transform.position = originalPosition;
					return;
				}
			}
			ResetCardToOriginalState();
		}
    }

	private void ChangeCharacterColliders(bool ChangeTo)
	{
		foreach(Enemy e in boardManager.enemies)
		{
			e.GetComponent<BoxCollider>().enabled = ChangeTo;
		}

		foreach(Character e in boardManager.Frendlies)
		{
			e.GetComponent<BoxCollider>().enabled = ChangeTo;
		}
	}

	void OnDestroy()
	{
		cardManager.isDragging = false;
	}
}