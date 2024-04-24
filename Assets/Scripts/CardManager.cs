using System;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class CardManager : MonoBehaviour
{
    public GameObject MovesLeftText;
    public List<GameObject> initialCards = new List<GameObject>();
    private List<GameObject> drawnCards = new List<GameObject>();
    public GameObject canvas;
    int draggedCardIndex;
    private TurnManager turnManager;
    public bool isDragging = false;
    public bool ExtraCardDrawBonusChosen = false;
    private Deck deck;

    void Start()
    {
        turnManager = GameObject.Find("TurnManager").GetComponent<TurnManager>();
        deck = GameObject.Find("Deck").GetComponent<Deck>();
        DrawCards(initialCards, true);
    }

    void Update()
    {
        MovesLeftText.GetComponent<TextMeshProUGUI>().text="Moves left: "+ PlayerPrefs.GetInt("MovesLeft").ToString();
        DrawCardCheck();
    }

    private void HandleCardMovedFromHand(Card card)
    {
        //if card is returned to hand after used dragged it out.
        draggedCardIndex = drawnCards.IndexOf(card.gameObject);
        drawnCards.Remove(card.gameObject);
        isDragging = true;
        DrawCards(drawnCards);
    }

    private void HandleCardMovedToHand(Card card)
    {
        //card returned from hand after being dragged.
        isDragging = false;
        drawnCards.Insert(draggedCardIndex, card.gameObject);
        DrawCards(drawnCards);
    }
    
    private void InitiateCardDraw(Deck deck, bool isExtraDraw)
    {
		if (deck)
		{
			Debug.Log("Attempting to draw. Cards left: " + deck.cards.Count);
			GameObject card = deck.Pop();
			Debug.Log("Card drawn: " + (card != null ? card.name : "None") + ". Cards left now: " + deck.cards.Count);

			if (!card)
			{
				return;
			}

			InstantiateCardInHand(drawnCards, card);
			DrawCards(drawnCards);
			if (!isExtraDraw) turnManager.SubtractPlayerMove();
		}
	}

    private bool RollTheDiceForExtraCardDraw()
    {
		System.Random random = new System.Random();
		int randomNumber = random.Next(1, 11);

        if(randomNumber > 0) return true; //for now the chance is set to 100%
        else return false;
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

        if(hit.collider==deck.gameObject.GetComponent<Collider>())

        InitiateCardDraw(deck, false);
        if(deck.cards.Count > 0 && RollTheDiceForExtraCardDraw() && ExtraCardDrawBonusChosen) InitiateCardDraw(deck, true);
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

            //update original status of the card.

            Card card = cardObjects[i].GetComponent<Card>();
            card.originalPosition = coords;
            card.originalScale = new Vector3(0.7f, 0.7f, 1);
            card.originalRotation = Quaternion.Euler(0, 0, rotation);
        }
    }

    private Vector3 CalculateCardPosition(int index, int totalCards)
    {
        float gap = -100f;
        
        //if initial cards list is empty, this will break, but its fine for now
        float cardWidth = initialCards[0].GetComponent<RectTransform>().sizeDelta.x;
        float totalWidth = (totalCards - 1) * (cardWidth + gap);

        int midpoint = totalCards / 2;
        int posY = -250;
        float canvasWidth = canvas.GetComponent<RectTransform>().rect.width;

        //if cards dont fit into the screen, make gap smaller.
        if (totalWidth > canvasWidth)
        {
            gap = -((cardWidth * totalCards) - canvasWidth) / totalCards;   
            totalWidth = (totalCards - 1) * (cardWidth + gap);
        }
        
        if (totalCards % 2 == 0 && index >= midpoint)
        {
            posY -= Mathf.Abs(midpoint - index - 1) * 20;
        }
        else
        {
            posY -= Mathf.Abs(midpoint - index) * 20;
        }
        
        float initialX = -totalWidth / 2f;
        float posX = initialX + index * (cardWidth + gap);
        float posZ = 0;

        //make every other card slightly behind its neighbor.
        //this prevents cards gliching when camera is moving.
        if (index % 2 == 0)
        {
            posZ = -0.1f;
        }

        return new Vector3(posX, posY, posZ);
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
            //else this is the midpoint and rotation is 0.
        }

        return rotation;
    }

    private void InstantiateOrUpdateCard(GameObject cardObject, Vector3 position, int rotation)
    {
        GameObject newCard = Instantiate(cardObject, position, Quaternion.identity);
        newCard.transform.SetParent(canvas.transform.Find("Cards"), false);
        newCard.transform.localScale = new Vector3(0.7f, 0.7f, 1);
        newCard.transform.localRotation = Quaternion.Euler(0, 0, rotation);
        
        //sub to events.
        newCard.GetComponent<Card>().OnCardMovedFromHand += HandleCardMovedFromHand;
        newCard.GetComponent<Card>().OnCardMovedToHand += HandleCardMovedToHand;
        
        drawnCards.Add(newCard);
    }

    //is any card being dragged?
    public bool IsDragging()
    {
        return isDragging;
    }

    //draws a card. used when a new board is prepared
    public void DrawACard()
    {
        InitiateCardDraw(deck, false);
    }
}
