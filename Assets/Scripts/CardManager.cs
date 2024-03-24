using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CardManager : MonoBehaviour
{
    public GameObject MovesLeftText;
    public List<GameObject> initialCards = new List<GameObject>();
    private List<GameObject> drawnCards = new List<GameObject>();
    public GameObject canvas;
    int draggedCardIndex;
    private TurnManager turnManager;

    void Start()
    {
        turnManager = GameObject.Find("TurnManager").GetComponent<TurnManager>();
        DrawCards(initialCards, true);
        
        //sub to events.
        foreach (GameObject go in drawnCards)
        {
            Card card = go.GetComponent<Card>();
            card.OnCardMovedFromHand += HandleCardMovedFromHand;
            card.OnCardMovedToHand += HandleCardMovedToHand;
        }
    }

    void Update()
    {
        MovesLeftText.GetComponent<TextMeshProUGUI>().text="Moves left: "+ PlayerPrefs.GetInt("MovesLeft").ToString();
        DrawCardCheck();
    }

    private void HandleCardMovedFromHand(Card card)
    {
        //if card is returned to hand after used dragged it out..
        draggedCardIndex = drawnCards.IndexOf(card.gameObject);
        drawnCards.Remove(card.gameObject);
        DrawCards(drawnCards);
    }

    private void HandleCardMovedToHand(Card card)
    {
        drawnCards.Insert(draggedCardIndex, card.gameObject);
        DrawCards(drawnCards);
    }
    
    private void DrawCardCheck()
    {
        if (!Input.GetMouseButtonDown(0))
        {
            //mouse not clicked
            return;
        }

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (!Physics.Raycast(ray, out hit))
        {
            //no raycast
            return;
        }

        GameObject clickedObject = hit.collider.gameObject;

        Deck deck = clickedObject.GetComponent<Deck>();

        if (deck)
        {
            GameObject card = deck.Pop();

            if (!card)
            {
                Debug.Log("no cad");
                return;
            }

            InstantiateCardInHand(drawnCards, card);
            DrawCards(drawnCards);
            turnManager.SubtractPlayerMove();
        }
    }

    private void InstantiateCardInHand(List<GameObject> cardObjects, GameObject card)
    {
        //instantiate at last position.
        int i = cardObjects.Count;
        Vector3 coords = CalculateCardPosition(i, cardObjects.Count);
        int rotation = CalculateCardRotation(i, cardObjects.Count);
        InstantiateOrUpdateCard(card, coords, rotation);
    }

    private void DrawCards(List<GameObject> cardObjects, bool toInstantiate = false)
    {
        for (int i = 0; i < cardObjects.Count; i++)
        {
            Vector3 coords = CalculateCardPosition(i, cardObjects.Count);
            int rotation = CalculateCardRotation(i, cardObjects.Count);

            if (toInstantiate)
            {
                InstantiateOrUpdateCard(cardObjects[i], coords, rotation);
            }
            else
            {
                cardObjects[i].transform.localPosition = coords;
                cardObjects[i].transform.localRotation = Quaternion.Euler(0, 0, rotation);
            }
        }
    }

    private Vector3 CalculateCardPosition(int index, int totalCards)
    {
        //todo calculate gap dynamically based on number of cards.
        float gap = -100f;
        
        //if initial cards list is empty, this will break, but its fine for now
        float cardWidth = initialCards[0].GetComponent<RectTransform>().sizeDelta.x;
        float totalWidth = (totalCards - 1) * (cardWidth + gap);
        float initialX = -totalWidth / 2f;

        int midpoint = totalCards / 2;
        int posY = -250;

        if (totalCards % 2 == 0 && index >= midpoint)
        {
            posY -= Mathf.Abs(midpoint - index - 1) * 20;
        }
        else
        {
            posY -= Mathf.Abs(midpoint - index) * 20;
        }

        float posX = initialX + index * (cardWidth + gap);
        return new Vector3(posX, posY, 0);
    }

    private int CalculateCardRotation(int index, int totalCards)
    {
        int midpoint = totalCards / 2;
        int rotation = 0;

        if (totalCards % 2 == 0)
        {
            if (index >= midpoint)
            {
                rotation = (midpoint - index - 1) * 5 - 10;
            }
            else rotation = (midpoint - index) * 5 + 10;
        }
        else
        {
            if (index < midpoint)
            {
                rotation = (midpoint - index) * 5 + 10;
            }
            else if (index > midpoint)
            {
                rotation = (midpoint - index) * 5 - 10;
            }
        }

        return rotation;
    }

    private void InstantiateOrUpdateCard(GameObject cardObject, Vector3 position, int rotation)
    {
        GameObject newCard = Instantiate(cardObject, position, Quaternion.identity);
        newCard.transform.SetParent(canvas.transform.Find("Cards"), false);
        newCard.transform.localScale = new Vector3(0.7f, 0.7f, 1);
        newCard.transform.localRotation = Quaternion.Euler(0, 0, rotation);
        drawnCards.Add(newCard);
    }
}
