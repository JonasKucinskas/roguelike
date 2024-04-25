using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Tutorial : MonoBehaviour
{
    ///Tekstui butu geriausia naudoti tik viena gameobject, bet kazkodel kai
    ///movetextsmooth bando grazinti teksta atgal, jis nenueina i teisingas koordinates :shrug:
    ///Jeigu rasit kodel, pakeisti teksta i viena objekta
    public GameObject tutorialText;
    private TextMeshProUGUI textMesh;

	public GameObject deckObject;
    public GameObject cardManagerObject;
    private List<GameObject> cardsCopy;
    private List<GameObject> drawnCardsCopy;

    private Vector3 startingPosGo = new Vector3(0, -0.73f, 6.7978f);
    private Vector3 endPosGo = new Vector3(0, -0.73f, 27f);
    private Vector3 startingPosText = new Vector3(-1066f, 710f, -1f);
    private Vector3 endPosText = new Vector3(0, 710f, 1f);


    void Start()
    {
        deckObject.transform.position = endPosGo;
        Debug.Log(tutorialText.transform.position);
        StartCoroutine(StartTutorial());

		textMesh = tutorialText.GetComponent<TextMeshProUGUI>();
	}
    IEnumerator StartTutorial()
    {
        ///===========Pirma tutorial dalis===============
        ///Parodomas pirmasis tekstas
		yield return new WaitForSeconds(3f);
		textMesh.text = "You can pull cards from the deck to gain more cards. \n\nThis uses up one move in your turn.";

        StartCoroutine(MoveGameObjectSmooth(endPosText, 1000f, tutorialText));
        yield return new WaitForSeconds(6f);
        StartCoroutine(MoveGameObjectSmooth(startingPosText, 1000f, tutorialText));
        yield return new WaitForSeconds(1f);

        ///Parodomas antrasis tekstas ir laukiama paspaudimo ant deck
		yield return new WaitForSeconds(1f);
		textMesh.text = "The deck will allways be on the left side of the board.\n\nTry picking up a new card.";

        StartCoroutine(MoveGameObjectSmooth(endPosText, 1000f, tutorialText));        StartCoroutine(MoveGameObjectSmooth(startingPosGo, 10f, deckObject));

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
        yield return new WaitForSeconds(1f);
		textMesh.text = "You can drag cards from your hand onto one of the tiles on the board, spawning a new ally.";

        StartCoroutine(MoveGameObjectSmooth(endPosText, 1000f, tutorialText));

        CardManager cardScript = cardManagerObject.GetComponent<CardManager>();
        if (cardScript != null)
        {
        drawnCardsCopy = cardScript.drawnCards;
        }
        Debug.Log("Card count in hand: " + drawnCardsCopy.Count);
        while (drawnCardsCopy.Count != 0)
        {
            yield return null;
        }
        yield return new WaitForSeconds(3f);
        StartCoroutine(MoveGameObjectSmooth(startingPosText, 1000f, tutorialText));


		///Parodomas judejimo paaiskinimas
		yield return new WaitForSeconds(1f);
		textMesh.text = "You can select a friedly character. \n\nThen click an empty surrounding tile to move.";
		StartCoroutine(MoveGameObjectSmooth(endPosText, 1000f, tutorialText));

		Character character = GameObject.Find("Neutrofilas(Clone)").GetComponent<Character>();
		while (!character.hasMoved)
		{
			yield return null;
		}

		StartCoroutine(MoveGameObjectSmooth(startingPosText, 1000f, tutorialText));
		yield return new WaitForSeconds(6f);

		///Parodomas atakavimo paaiskinimas
		textMesh.text = "Select a friedly character again. \n\nAttack an enemy by clicking on its tile.";
		StartCoroutine(MoveGameObjectSmooth(endPosText, 1000f, tutorialText));

		while (!character.hasAttacked)
		{
			yield return null;
		}
		yield return new WaitForSeconds(1f);
		StartCoroutine(MoveGameObjectSmooth(startingPosText, 1000f, tutorialText));
	}

	IEnumerator MoveGameObjectSmooth(Vector3 target, float speed, GameObject go)
    {
		float distanceToTarget = Vector3.Distance(go.transform.position, target);

		// Continue the loop as long as the distance to target is greater than a small value to avoid floating point precision issues.
		while (distanceToTarget > 0.001f)
		{
			go.transform.position = Vector3.Lerp(go.transform.position, target, speed * Time.deltaTime / distanceToTarget);

			distanceToTarget = Vector3.Distance(go.transform.position, target);

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
