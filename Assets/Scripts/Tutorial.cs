using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class Tutorial : MonoBehaviour
{
    ///Tekstui butu geriausia naudoti tik viena gameobject, bet kazkodel kai
    ///movetextsmooth bando grazinti teksta atgal, jis nenueina i teisingas koordinates :shrug:
    ///Jeigu rasit kodel, pakeisti teksta i viena objekta
    public GameObject tutorialText;
    private TextMeshProUGUI textMesh;
	private BoardScript boardScript;
	private TurnManager turnManager;
	private PlayerHealth playerHealth;
	public GameObject deckObject;
    public GameObject cardManagerObject;
    private List<GameObject> cardsCopy;
    private List<GameObject> drawnCardsCopy;

    private Vector3 startingPosGo = new Vector3(0, -0.73f, 6.7978f);
    private Vector3 endPosGo = new Vector3(0, -0.73f, 27f);
    private Vector3 startingPosText = new Vector3(-700f, 710f, -1f);
    private Vector3 endPosText = new Vector3(0f, 710f, 1f);
	private Vector3 startingPosHeart = new Vector3(-430, 190f, 6f);
	private Vector3 endPosHeart = new Vector3(210, 200f, 6f);

	private bool ForceLose=false;

	void Start()
    {
		boardScript = GameObject.Find("Board").GetComponent<BoardScript>();
		turnManager=GameObject.Find("TurnManager").GetComponent<TurnManager>();
		playerHealth=GameObject.Find("PlayerHealthIndicator").GetComponent<PlayerHealth>();
        deckObject.transform.position = endPosGo;
		if(!ForceLose)
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
        StartCoroutine(MoveGameObjectSmooth(endPosText, 1000f, tutorialText));
        yield return new WaitForSeconds(6f);
        StartCoroutine(MoveGameObjectSmooth(startingPosText, 1000f, tutorialText));
        yield return new WaitForSeconds(1f);

		///Parodomas antrasis tekstas ir laukiama paspaudimo ant deck
		yield return new WaitForSeconds(1f);
		textMesh.text = "The deck will always be on the left side of the board.\n\nTry picking up a new card.";

        StartCoroutine(MoveGameObjectSmooth(endPosText, 1000f, tutorialText));
        StartCoroutine(MoveGameObjectSmooth(startingPosGo, 10f, deckObject));

        Deck deckScript = deckObject.GetComponent<Deck>();
        if(deckScript != null)
        {
            cardsCopy = deckScript.cards;
        }
        Debug.Log("Card count: " + cardsCopy.Count);
        while(cardsCopy.Count != 1)
        {
            yield return null;
        }
        Debug.Log("Card count: " + cardsCopy.Count);
        StartCoroutine(MoveGameObjectSmooth(startingPosText, 1000f, tutorialText));
        ///Yra sansas, kad imanoma paspausti ant kalades kol ji stutoriala atgal
        StartCoroutine(MoveGameObjectSmooth(endPosGo, 1000f, deckObject)); 
        yield return new WaitForSeconds(1f);

		///Parodomas treciasis tekstas
		boardScript.AllowPlayerInput=false;
		StartCoroutine(MoveAndScaleTileByCoordinatesSmooth(0, 0, -1.0f, 0.5f, 0.5f)); //hides tiles so the player only has one move
		StartCoroutine(MoveAndScaleTileByCoordinatesSmooth(0, 2, -1.0f, 0.5f, 0.5f));
		StartCoroutine(MoveAndScaleTileByCoordinatesSmooth(1, 0, -1.0f, 0.5f, 0.5f));
		StartCoroutine(MoveAndScaleTileByCoordinatesSmooth(1, 1, -1.0f, 0.5f, 0.5f));
		StartCoroutine(MoveAndScaleTileByCoordinatesSmooth(1, 2, -1.0f, 0.5f, 0.5f));
		StartCoroutine(MoveAndScaleTileByCoordinatesSmooth(3, 0, -1.0f, 0.5f, 0.5f));
		StartCoroutine(MoveAndScaleTileByCoordinatesSmooth(3, 2, -1.0f, 0.5f, 0.5f));
		StartCoroutine(MoveAndScaleTileByCoordinatesSmooth(2, 1, -1.0f, 0.5f, 0.5f));

		yield return new WaitForSeconds(1f);
		boardScript.AllowPlayerInput=true;
		textMesh.text = "You can drag cards from your hand onto the tiles on the board, which spawns your loyal ally.";

        StartCoroutine(MoveGameObjectSmooth(endPosText, 1000f, tutorialText));

		Card card = GameObject.Find("Neutrophil_card(Clone)").GetComponent<Card>();
		while (!card.cardPlaced)
		{
			yield return null;
		}

		yield return new WaitForSeconds(2f);
        StartCoroutine(MoveGameObjectSmooth(startingPosText, 1000f, tutorialText));

		///Parodomas judejimo paaiskinimas
		boardScript.AllowPlayerInput=false;
		StartCoroutine(MoveAndScaleTileByCoordinatesSmooth(1, 1, 1.0f, 2f, 0.5f));
		yield return new WaitForSeconds(1f);
		boardScript.AllowPlayerInput=true;
		textMesh.text = "You can select a friendly character. \n\nThen click an empty surrounding tile to move.";
		StartCoroutine(MoveGameObjectSmooth(endPosText, 1000f, tutorialText));

		Character character = GameObject.Find("Neutrofilas(Clone)").GetComponent<Character>();
		while (!character.hasMoved)
		{
			yield return null;
		}

		StartCoroutine(MoveGameObjectSmooth(startingPosText, 1000f, tutorialText));
		yield return new WaitForSeconds(3f);
		StartCoroutine(MoveAndScaleTileByCoordinatesSmooth(0, 1, -1.0f, 0.5f, 0.5f));

		///Parodomas atakavimo paaiskinimas
		textMesh.text = "Select a friendly character again. \n\nAttack an enemy by clicking on it.";
		StartCoroutine(MoveGameObjectSmooth(endPosText, 1000f, tutorialText));

		while (!character.hasAttacked)
		{
			yield return null;
		}

		boardScript.AllowPlayerInput=false;
		StartCoroutine(MoveAndScaleTileByCoordinatesSmooth(0, 0, 1.0f, 2f, 0.5f)); //shows tiles
		StartCoroutine(MoveAndScaleTileByCoordinatesSmooth(0, 2, 1.0f, 2f, 0.5f));
		StartCoroutine(MoveAndScaleTileByCoordinatesSmooth(1, 0, 1.0f, 2f, 0.5f));
		StartCoroutine(MoveAndScaleTileByCoordinatesSmooth(1, 2, 1.0f, 2f, 0.5f));
		StartCoroutine(MoveAndScaleTileByCoordinatesSmooth(3, 0, 1.0f, 2f, 0.5f));
		StartCoroutine(MoveAndScaleTileByCoordinatesSmooth(3, 2, 1.0f, 2f, 0.5f));
		StartCoroutine(MoveAndScaleTileByCoordinatesSmooth(2, 1, 1.0f, 2f, 0.5f));
		StartCoroutine(MoveAndScaleTileByCoordinatesSmooth(0, 1, 1.0f, 2f, 0.5f));

		yield return new WaitForSeconds(1f);
		boardScript.AllowPlayerInput=true;
		StartCoroutine(MoveGameObjectSmooth(startingPosText, 1000f, tutorialText));
		
		GameObject heart = GameObject.Find("Heart_8");
		StartCoroutine(MoveGameObjectSmooth(endPosHeart, 1000f, heart));
		yield return new WaitForSeconds(1f);

		//Parodomas priesu tikslas
		textMesh.text = "The enemies goal is to reach your end of the board. \n\nAfter reaching the end, you lose one (1) health point";
		StartCoroutine(MoveGameObjectSmooth(endPosText, 1000f, tutorialText));
		yield return new WaitForSeconds(7f);
		StartCoroutine(MoveGameObjectSmooth(startingPosText, 1000f, tutorialText));
		yield return new WaitForSeconds(2f);

		//Specialios atakos paaiskinimas
		textMesh.text = "Each friendly character has a special attack. \n\nYou can activate it by selecting an ally and pressing [space].";
		StartCoroutine(MoveGameObjectSmooth(endPosText, 1000f, tutorialText));
		StartCoroutine(boardScript.SpawnEnemy(1, 2));
		StartCoroutine(boardScript.SpawnEnemy(1, 0));

		NeutrophilCell neutrophil = GameObject.Find("Neutrofilas(Clone)").GetComponent<NeutrophilCell>();
		while (!neutrophil.hasUsedSpecialAttack)
		{
			yield return null;
		}

		yield return new WaitForSeconds(1f);
		StartCoroutine(MoveGameObjectSmooth(startingPosText, 1000f, tutorialText));
		yield return new WaitForSeconds(1f);

		//Parodoma laimejimo salyga
		textMesh.text = "Your goal is to defeat all viruses. \n\nKeep going and exterminate them.";
		StartCoroutine(MoveGameObjectSmooth(endPosText, 1000f, tutorialText));
		yield return new WaitForSeconds(5f);
		StartCoroutine(MoveGameObjectSmooth(startingPosText, 1000f, tutorialText));

		//Parodomas roguelike mechanikos paaiskinimas
		textMesh.text = "After beating a level, you get to choose a reward. \n\nThese rewards are for the current run only.";

		/*BoardScript boardScript = GameObject.Find("Board").GetComponent<BoardScript>();
		while (boardScript.enemies.Count != 0)
		{
			Debug.Log("enemies" + boardScript.enemies.Count);
			yield return null;
		}*/
		yield return new WaitForSeconds(1f);
		StartCoroutine(MoveGameObjectSmooth(endPosText, 1000f, tutorialText));
		yield return new WaitForSeconds(5f);
		StartCoroutine(MoveGameObjectSmooth(startingPosText, 1000f, tutorialText));

		boardScript.isTutorialLevel=2;
		ForceLose=true;
	}

	IEnumerator ForceLoseTutorial()
	{
		StartCoroutine(MoveGameObjectSmooth(startingPosGo, 10f, deckObject));
		yield return new WaitForSeconds(1f);
		turnManager.SubtractPlayerMove();
		turnManager.SubtractPlayerMove();
		playerHealth.currentHealth=1;
		yield return new WaitForSeconds(3f);
		textMesh.text = "But not every fight is going to be that easy";
        tutorialText.SetActive(true);
        StartCoroutine(MoveGameObjectSmooth(endPosText, 1000f, tutorialText));
        yield return new WaitForSeconds(3f);
        StartCoroutine(MoveGameObjectSmooth(startingPosText, 1000f, tutorialText));
	}
	IEnumerator MoveAndScaleTileByCoordinatesSmooth(int x, int y, float verticalShift, float scaleTarget, float speed)
	{
		// Construct the tile name based on provided coordinates
		string tileName = $"Tile_{x}_{y}";
		GameObject tile = GameObject.Find(tileName).GetComponentInChildren<TileScript>().gameObject;

		if (tile == null)
		{
			Debug.LogError("Tile not found: " + tileName);
			yield break;
		}

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
		float StartSlowingDown=distanceToTarget*0.25f;
		bool slowedDown= false;
		// Continue the loop as long as the distance to target is greater than a small value to avoid floating point precision issues.
		while (distanceToTarget > 0.001f)
		{
			go.transform.position = Vector3.Lerp(go.transform.position, target, speed * Time.deltaTime / distanceToTarget);

			distanceToTarget = Vector3.Distance(go.transform.position, target);

			if(slowedDown==false&&distanceToTarget < StartSlowingDown)
			{
				speed*=0.5f;
				slowedDown=true;
			}

			yield return null;
		}

		go.transform.position = target;
	}

    IEnumerator MoveTextSmooth(Vector3 target, float speed, TMP_Text text)
    {
        while (transform.position != target)
        {
            text.transform.position = Vector3.Lerp(text.transform.position, target, speed * Time.deltaTime);
            yield return null;
        }
        text.transform.position = target;
    }
}
