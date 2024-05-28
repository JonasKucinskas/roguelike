using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class Tutorial : MonoBehaviour
{
	public GameObject tutorialText;
	private TextMeshProUGUI textMesh;
	private BoardScript boardScript;
	private TurnManager turnManager;
	private PlayerHealth playerHealth;
	public GameObject deckObject;
	public GameObject cardManagerObject;
	public GameObject temperatureText;
	public GameObject tempretureEffectText;
	private List<GameObject> cardsCopy;
	private List<GameObject> drawnCardsCopy;

	private Vector3 startingPosGo = new Vector3(0, -0.73f, 6.7978f);
	private Vector3 endPosGo = new Vector3(0, -0.73f, 27f);
	private Vector3 startingPosText = new Vector3(-1224f, 315f, -1f);
	private Vector3 endPosText = new Vector3(-683f, 315f, 0f);
	private Vector3 startingPosHeart = new Vector3(-430, 200f, 6f);
	private Vector3 endPosHeart = new Vector3(210, 200f, 6f);

	private bool ForceLose = false;

	void Start()
	{
		boardScript = GameObject.Find("Board").GetComponent<BoardScript>();
		turnManager = GameObject.Find("TurnManager").GetComponent<TurnManager>();
		playerHealth = GameObject.Find("PlayerHealthIndicator").GetComponent<PlayerHealth>();

		deckObject.transform.position = endPosGo;
		if (!ForceLose)
		{
			StartCoroutine(StartTutorial());
		}
		else
		{
			StartCoroutine(ForceLoseTutorial());
		}

		textMesh = tutorialText.GetComponentInChildren<Image>().GetComponentInChildren<TextMeshProUGUI>();
	}

	IEnumerator StartTutorial()
	{
		///===========Pirma tutorial dalis===============
		///Parodomas pirmasis tekstas
		yield return new WaitForSeconds(3f);
		textMesh.text = "You can pull cards from the deck to gain more cards. \n\nThis uses up one(1) move in your turn.";
		tutorialText.SetActive(true);
		StartCoroutine(MoveTextSmooth(endPosText, 1000f, tutorialText.GetComponent<RectTransform>()));
		yield return new WaitForSeconds(6f);
		StartCoroutine(MoveTextSmooth(startingPosText, 1000f, tutorialText.GetComponent<RectTransform>()));
		yield return new WaitForSeconds(1f);

		///Parodomas antrasis tekstas ir laukiama paspaudimo ant deck
		yield return new WaitForSeconds(1f);
		textMesh.text = "The deck will always be on the left side of the board.\n\nTry picking up a new card.";

		StartCoroutine(MoveTextSmooth(endPosText, 1000f, tutorialText.GetComponent<RectTransform>()));
		StartCoroutine(MoveGameObjectSmooth(startingPosGo, 10f, deckObject));

		Deck deckScript = deckObject.GetComponent<Deck>();
		if (deckScript != null)
		{
			cardsCopy = deckScript.cards;
		}
		Debug.Log("Card count: " + cardsCopy.Count);
		while (cardsCopy.Count != 1)
		{
			yield return null;
		}
		Debug.Log("Card count: " + cardsCopy.Count);
		StartCoroutine(MoveTextSmooth(startingPosText, 1000f, tutorialText.GetComponent<RectTransform>()));
		///Yra sansas, kad imanoma paspausti ant kalades kol ji stutoriala atgal
		StartCoroutine(MoveGameObjectSmooth(endPosGo, 1000f, deckObject));
		deckObject.SetActive(false);
		yield return new WaitForSeconds(1f);

		///Parodomas treciasis tekstas
		boardScript.AllowPlayerInput = false;
		StartCoroutine(MoveAndScaleTileByCoordinatesSmooth(0, 0, -1.0f, 0.5f, 0.5f, true)); //hides tiles so the player only has one move
		StartCoroutine(MoveAndScaleTileByCoordinatesSmooth(0, 2, -1.0f, 0.5f, 0.5f, true));
		StartCoroutine(MoveAndScaleTileByCoordinatesSmooth(1, 0, -1.0f, 0.5f, 0.5f, true));
		StartCoroutine(MoveAndScaleTileByCoordinatesSmooth(1, 1, -1.0f, 0.5f, 0.5f, true));
		StartCoroutine(MoveAndScaleTileByCoordinatesSmooth(1, 2, -1.0f, 0.5f, 0.5f, true));
		StartCoroutine(MoveAndScaleTileByCoordinatesSmooth(3, 0, -1.0f, 0.5f, 0.5f, true));
		StartCoroutine(MoveAndScaleTileByCoordinatesSmooth(3, 2, -1.0f, 0.5f, 0.5f, true));
		StartCoroutine(MoveAndScaleTileByCoordinatesSmooth(2, 1, -1.0f, 0.5f, 0.5f, true));

		yield return new WaitForSeconds(1f);
		boardScript.AllowPlayerInput = true;
		textMesh.text = "You can drag cards from your hand onto the tiles on the board, which spawns your loyal ally.";

		StartCoroutine(MoveTextSmooth(endPosText, 1000f, tutorialText.GetComponent<RectTransform>()));

		Card card = GameObject.Find("Neutrophil_card(Clone)").GetComponent<Card>();
		while (!card.cardPlaced)
		{
			yield return null;
		}

		yield return new WaitForSeconds(2f);
		StartCoroutine(MoveTextSmooth(startingPosText, 1000f, tutorialText.GetComponent<RectTransform>()));

		///Parodomas judejimo paaiskinimas
		boardScript.AllowPlayerInput = false;
		StartCoroutine(MoveAndScaleTileByCoordinatesSmooth(1, 1, 1.0f, 2f, 0.5f, false));
		yield return new WaitForSeconds(1f);
		boardScript.AllowPlayerInput = true;
		textMesh.text = "You can select a friendly character. \n\nThen click an empty surrounding tile to move.";
		StartCoroutine(MoveTextSmooth(endPosText, 1000f, tutorialText.GetComponent<RectTransform>()));

		Character character = GameObject.Find("Neutrofilas(Clone)").GetComponent<Character>();
		while (!character.hasMoved)
		{
			yield return null;
		}
		StartCoroutine(MoveTextSmooth(startingPosText, 1000f, tutorialText.GetComponent<RectTransform>()));
		yield return new WaitForSeconds(3f);
		StartCoroutine(MoveAndScaleTileByCoordinatesSmooth(0, 1, -1.0f, 0.5f, 0.5f, true));

		///Parodomas atakavimo paaiskinimas
		textMesh.text = "Select a friendly character again. \n\nAttack an enemy by clicking on it.";
		StartCoroutine(MoveTextSmooth(endPosText, 1000f, tutorialText.GetComponent<RectTransform>()));

		while (!character.hasAttacked)
		{
			yield return null;
		}
		StartCoroutine(MoveAndScaleTileByCoordinatesSmooth(0, 0, 1.0f, 2f, 0.5f, false)); //shows tiles
		StartCoroutine(MoveAndScaleTileByCoordinatesSmooth(0, 2, 1.0f, 2f, 0.5f, false));
		StartCoroutine(MoveAndScaleTileByCoordinatesSmooth(0, 1, 1.0f, 2f, 0.5f, false));
		StartCoroutine(MoveAndScaleTileByCoordinatesSmooth(1, 0, 1.0f, 2f, 0.5f, false));
		StartCoroutine(MoveAndScaleTileByCoordinatesSmooth(1, 2, 1.0f, 2f, 0.5f, false));
		StartCoroutine(MoveAndScaleTileByCoordinatesSmooth(3, 0, 1.0f, 2f, 0.5f, false));
		StartCoroutine(MoveAndScaleTileByCoordinatesSmooth(3, 2, 1.0f, 2f, 0.5f, false));
		StartCoroutine(MoveAndScaleTileByCoordinatesSmooth(2, 1, 1.0f, 2f, 0.5f, false));
		

		yield return new WaitForSeconds(1f);
		boardScript.AllowPlayerInput = true;
		StartCoroutine(MoveTextSmooth(startingPosText, 1000f, tutorialText.GetComponent<RectTransform>()));

		GameObject heart = GameObject.Find("Heart_8");
		StartCoroutine(MoveGameObjectSmooth(startingPosHeart, 10000f, heart));
		StartCoroutine(MoveGameObjectSmooth(endPosHeart, 1000f, heart));
		yield return new WaitForSeconds(1f);

		//Parodomas priesu tikslas
		textMesh.text = "The enemies goal is to reach your end of the board. \n\nAfter reaching the end, you lose one (1) health point";
		StartCoroutine(MoveTextSmooth(endPosText, 1000f, tutorialText.GetComponent<RectTransform>()));
		yield return new WaitForSeconds(7f);
		StartCoroutine(MoveTextSmooth(startingPosText, 1000f, tutorialText.GetComponent<RectTransform>()));
		yield return new WaitForSeconds(2f);

		//Pristatoma temperatūra
		textMesh.text = "Temperature is a resource that gives different game effects depending on its value. \n\nActive effect can be seen written below temperature.";
		StartCoroutine(MoveTextSmooth(endPosText, 1000f, tutorialText.GetComponent<RectTransform>()));
		StartCoroutine(SetObjectActive(temperatureText));
		StartCoroutine(SetObjectActive(tempretureEffectText));
		yield return new WaitForSeconds(7f);
		StartCoroutine(MoveTextSmooth(startingPosText, 1000f, tutorialText.GetComponent<RectTransform>()));
		yield return new WaitForSeconds(2f);

		//Specialios atakos paaiskinimas
		textMesh.text = "Each friendly character has a special attack. \n\nYou can activate it by selecting an ally and pressing [space].";
		StartCoroutine(MoveTextSmooth(endPosText, 1000f, tutorialText.GetComponent<RectTransform>()));
		StartCoroutine(boardScript.SpawnEnemy(1, 2));
		StartCoroutine(boardScript.SpawnEnemy(1, 0));

		StartCoroutine(MoveAndScaleTileByCoordinatesSmooth(0, 0, -1.0f, 0.5f, 0.5f, true));
		StartCoroutine(MoveAndScaleTileByCoordinatesSmooth(0, 1, -1.0f, 0.5f, 0.5f, true));
		StartCoroutine(MoveAndScaleTileByCoordinatesSmooth(0, 2, -1.0f, 0.5f, 0.5f, true));
		StartCoroutine(MoveAndScaleTileByCoordinatesSmooth(2, 0, -1.0f, 0.5f, 0.5f, true));
		StartCoroutine(MoveAndScaleTileByCoordinatesSmooth(2, 1, -1.0f, 0.5f, 0.5f, true));
		StartCoroutine(MoveAndScaleTileByCoordinatesSmooth(2, 2, -1.0f, 0.5f, 0.5f, true));

		character.basicAttackDisabled = true; //disables basic attack

		NeutrophilCell neutrophil = GameObject.Find("Neutrofilas(Clone)").GetComponent<NeutrophilCell>();
		while (!neutrophil.hasUsedSpecialAttack)
		{
			yield return null;
		}

		yield return new WaitForSeconds(1f);
		StartCoroutine(MoveTextSmooth(startingPosText, 1000f, tutorialText.GetComponent<RectTransform>()));
		StartCoroutine(MoveAndScaleTileByCoordinatesSmooth(0, 0, 1.0f, 2f, 0.5f, false));
		StartCoroutine(MoveAndScaleTileByCoordinatesSmooth(0, 1, 1.0f, 2f, 0.5f, false));
		StartCoroutine(MoveAndScaleTileByCoordinatesSmooth(0, 2, 1.0f, 2f, 0.5f, false));
		StartCoroutine(MoveAndScaleTileByCoordinatesSmooth(2, 0, 1.0f, 2f, 0.5f, false));
		StartCoroutine(MoveAndScaleTileByCoordinatesSmooth(2, 1, 1.0f, 2f, 0.5f, false));
		StartCoroutine(MoveAndScaleTileByCoordinatesSmooth(2, 2, 1.0f, 2f, 0.5f, false));
		yield return new WaitForSeconds(1f);

		character.basicAttackDisabled = false; //reenables basic attack

		//Paaiškinama temperatura
		textMesh.text = "Special attack activation increases temperature. \n\nWhen temperature reaches 45 degrees Celsius, the game is over.";
		StartCoroutine(MoveTextSmooth(endPosText, 1000f, tutorialText.GetComponent<RectTransform>()));
		yield return new WaitForSeconds(7f);
		StartCoroutine(MoveTextSmooth(startingPosText, 1000f, tutorialText.GetComponent<RectTransform>()));
		yield return new WaitForSeconds(2f);
		textMesh.text = "Medicine card can help to bring temperature down. \n\nWhen in need you can always drag it to the board. \n\nTry taking the card from the deck.";
		StartCoroutine(MoveTextSmooth(endPosText, 1000f, tutorialText.GetComponent<RectTransform>()));
		deckObject.SetActive(true);
		StartCoroutine(MoveGameObjectSmooth(startingPosGo, 10f, deckObject));
		//yield return new WaitForSeconds(7f);
		Debug.Log("Card count: " + cardsCopy.Count);
		while (cardsCopy.Count != 0)
		{
			yield return null;
		}
		Debug.Log("Card count: " + cardsCopy.Count);
		StartCoroutine(MoveTextSmooth(startingPosText, 1000f, tutorialText.GetComponent<RectTransform>()));
		yield return new WaitForSeconds(3f);

		//Parodoma laimejimo salyga
		textMesh.text = "Your goal is to defeat all viruses. \n\nKeep going and exterminate them.";
		StartCoroutine(MoveTextSmooth(endPosText, 1000f, tutorialText.GetComponent<RectTransform>()));
		yield return new WaitForSeconds(5f);
		StartCoroutine(MoveTextSmooth(startingPosText, 1000f, tutorialText.GetComponent<RectTransform>()));

		//Parodomas roguelike mechanikos paaiskinimas
		textMesh.text = "After beating a level, you get to choose a reward. \n\nThese rewards are for the current run only.";

		/*BoardScript boardScript = GameObject.Find("Board").GetComponent<BoardScript>();
		while (boardScript.enemies.Count != 0)
		{
			Debug.Log("enemies" + boardScript.enemies.Count);
			yield return null;
		}*/
		yield return new WaitForSeconds(1f);
		StartCoroutine(MoveTextSmooth(endPosText, 1000f, tutorialText.GetComponent<RectTransform>()));
		yield return new WaitForSeconds(5f);
		StartCoroutine(MoveTextSmooth(startingPosText, 1000f, tutorialText.GetComponent<RectTransform>()));

		boardScript.isTutorialLevel = 2;
		ForceLose = true;
	}

	IEnumerator ForceLoseTutorial()
	{
		StartCoroutine(MoveGameObjectSmooth(startingPosGo, 10f, deckObject));
		yield return new WaitForSeconds(1f);
		turnManager.SubtractPlayerMove();
		turnManager.SubtractPlayerMove();
		playerHealth.currentHealth = 1;
		yield return new WaitForSeconds(3f);
		textMesh.text = "But not every fight is going to be that easy";
		tutorialText.SetActive(true);
		StartCoroutine(MoveTextSmooth(endPosText, 1000f, tutorialText.GetComponent<RectTransform>()));
		yield return new WaitForSeconds(3f);
		StartCoroutine(MoveTextSmooth(startingPosText, 1000f, tutorialText.GetComponent<RectTransform>()));
	}

	IEnumerator MoveAndScaleTileByCoordinatesSmooth(int x, int y, float verticalShift, float scaleTarget, float speed, bool disable)
	{
		// Construct the tile name based on provided coordinates
		string tileName = $"Tile_{x}_{y}";
		GameObject tile = GameObject.Find(tileName).GetComponentInChildren<TileScript>().gameObject;

		if (tile == null)
		{
			Debug.LogError("Tile not found: " + tileName);
			yield break;
		}

		TileScript tilescript = GameObject.Find(tileName).GetComponentInChildren<TileScript>();
		tilescript.DisableForTutorial(disable);

		Vector3 startPosition = tile.transform.position;
		Vector3 endPosition = new Vector3(startPosition.x, startPosition.y + verticalShift, startPosition.z);
		Vector3 startScale = tile.transform.localScale;
		Vector3 endScale = startScale * scaleTarget;

		float distanceToTarget = Vector3.Distance(tile.transform.position, endPosition);
		float scaleDistance = Vector3.Distance(startScale, endScale);

		// Continue the loop as long as the distance to target and scale distance are greater than a small value to avoid precision issues.
		while (distanceToTarget > 0.001f || scaleDistance > 0.001f)
		{
			tile.transform.position = Vector3.Lerp(tile.transform.position, endPosition, speed * Time.deltaTime / distanceToTarget);
			tile.transform.localScale = Vector3.Lerp(tile.transform.localScale, endScale, speed * Time.deltaTime / scaleDistance);

			distanceToTarget = Vector3.Distance(tile.transform.position, endPosition);
			scaleDistance = Vector3.Distance(tile.transform.localScale, endScale);
			yield return null;
		}

		tile.transform.position = endPosition; // Ensure the position is exactly at the target
		tile.transform.localScale = endScale; // Ensure the scale is exactly at the target
	}

	IEnumerator MoveGameObjectSmooth(Vector3 target, float speed, GameObject go)
	{
		float distanceToTarget = Vector3.Distance(go.transform.position, target);
		float StartSlowingDown = distanceToTarget * 0.25f;
		bool slowedDown = false;
		// Continue the loop as long as the distance to target is greater than a small value to avoid floating point precision issues.
		while (distanceToTarget > 0.001f)
		{
			go.transform.position = Vector3.Lerp(go.transform.position, target, speed * Time.deltaTime / distanceToTarget);

			distanceToTarget = Vector3.Distance(go.transform.position, target);

			if (slowedDown == false && distanceToTarget < StartSlowingDown)
			{
				speed *= 0.5f;
				slowedDown = true;
			}

			yield return null;
		}

		go.transform.position = target;
	}

	IEnumerator MoveTextSmooth(Vector3 target, float speed, RectTransform rectTransform)
	{
		Vector3 distanceToTarget = rectTransform.anchoredPosition - (Vector2)target;
		while (distanceToTarget.magnitude > 0.001f)
		{
			rectTransform.anchoredPosition = Vector3.Lerp(rectTransform.anchoredPosition, target, speed * Time.deltaTime / distanceToTarget.magnitude);
			distanceToTarget = rectTransform.anchoredPosition - (Vector2)target;
			yield return null;
		}

		rectTransform.anchoredPosition = target;
	}

	IEnumerator SetObjectActive(GameObject go)
    {
		if(go == null)
        {
			yield return null;
        }

		if (go.active == false)
		{
			go.SetActive(true);
		}
		else if (go.active == true)
		{
			go.SetActive(false);
		}
	}

	IEnumerator AddACardToTheDeck(GameObject card)
    {
		if (card == null)
			yield return null;

		deckObject.GetComponent<Deck>().AddACard(card);
    }
}