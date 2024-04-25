using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Tutorial : MonoBehaviour
{
    ///Tekstui butu geriausia naudoti tik viena gameobject, bet kazkodel kai
    ///movetextsmooth bando grazinti teksta atgal, jis nenueina i teisingas koordinates :shrug:
    ///Jeigu rasit kodel, pakeisti teksta i viena objekta
    public GameObject tutorialText1;
    public GameObject tutorialText1_2;
    public GameObject tutorialText2;
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
        Debug.Log(tutorialText1.transform.position);
        StartCoroutine(StartTutorial());
    }
    IEnumerator StartTutorial()
    {
        ///===========Pirma tutorial dalis===============
        ///Parodomas pirmasis tekstas
        yield return new WaitForSeconds(3f);
        tutorialText1.gameObject.SetActive(true);
        StartCoroutine(MoveGameObjectSmooth(endPosText, 5f, tutorialText1));
        yield return new WaitForSeconds(6f);
        StartCoroutine(MoveGameObjectSmooth(startingPosText, 5f, tutorialText1));
        yield return new WaitForSeconds(1f);
        tutorialText1.gameObject.SetActive(false);

        ///Parodomas antrasis tekstas ir laukiama paspaudimo ant deck
        yield return new WaitForSeconds(1f);
        tutorialText1_2.gameObject.SetActive(true);
        StartCoroutine(MoveGameObjectSmooth(endPosText, 5f, tutorialText1_2));        StartCoroutine(MoveGameObjectSmooth(startingPosGo, 10f, deckObject));

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
        StartCoroutine(MoveGameObjectSmooth(startingPosText, 5f, tutorialText1_2));
        ///Yra sansas, kad imanoma paspausti ant kalades kol ji stutoriala atgal
        StartCoroutine(MoveGameObjectSmooth(endPosGo, 5f, deckObject)); 
        yield return new WaitForSeconds(1f);
        tutorialText1_2.gameObject.SetActive(false);

        ///Parodomas treciasis tekstas
        yield return new WaitForSeconds(1f);
        tutorialText2.gameObject.SetActive(true);
        StartCoroutine(MoveGameObjectSmooth(endPosText, 5f, tutorialText2));

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
        StartCoroutine(MoveGameObjectSmooth(startingPosText, 5f, tutorialText2));

    }

    IEnumerator MoveGameObjectSmooth(Vector3 target, float speed, GameObject go)
    {
        while (transform.position != target)
        {
            go.transform.position = Vector3.Lerp(go.transform.position, target, speed * Time.deltaTime);
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
