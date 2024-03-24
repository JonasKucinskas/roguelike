using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class CardManager : MonoBehaviour
{
    public GameObject MovesLeftText;
    public List<GameObject> initialCards = new List<GameObject>();
    private List<GameObject> drawnCards = new List<GameObject>();
    public GameObject canvas;
    int draggedCardIndex;
    void Start()
    {
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
    }

    private void HandleCardMovedFromHand(Card card)
    {
        draggedCardIndex = drawnCards.IndexOf(card.gameObject);
        drawnCards.Remove(card.gameObject);
        DrawCards(drawnCards, false);
    }

    private void HandleCardMovedToHand(Card card)
    {
        drawnCards.Insert(draggedCardIndex, card.gameObject);
        DrawCards(drawnCards, false);
    }
    
    private void DrawCards(List<GameObject> cardObjects, bool toInstantiate)
    {
        //should set gap dynamically based on the number of cards.
        float gap = -100f;
        float cardWidth = cardObjects[0].GetComponent<RectTransform>().sizeDelta.x;

        float totalWidth = (cardObjects.Count - 1) * (cardWidth + gap);
        float initialX = -totalWidth / 2f;

        int midpoint = cardObjects.Count / 2;

        for (int i = 0; i < cardObjects.Count; i++)
        {
            int rotation = 0;

            if (cardObjects.Count % 2 == 0)
            {
                if (i >= midpoint)
                {
                    rotation = (midpoint - i - 1) * 5 - 10;
                }
                else rotation = (midpoint - i) * 5 + 10;
            }
            else
            {
                if (i < midpoint)
                {
                    rotation = (midpoint - i) * 5 + 10;
                }
                else if (i > midpoint)
                {
                    rotation = (midpoint - i) * 5 - 10;
                }
            }

            //cards get lower the closer to edges they are.
            int posY = -250;

            if (cardObjects.Count % 2 == 0 && i >= midpoint)
            {
                posY -= Math.Abs(midpoint - i - 1) * 20;
            }
            else
            {
                posY -= Math.Abs(midpoint - i) * 20;
            }

            float posX = initialX + i * (cardWidth + gap);
            Vector3 coords = new Vector3(posX, posY, 0);

            if (toInstantiate)
            {
                GameObject newCard = Instantiate(cardObjects[i], coords, Quaternion.identity);
                newCard.transform.SetParent(canvas.transform.Find("Cards"), false);
                
                //resize, nes per dideles tos korteles.
                newCard.transform.localScale = new Vector3(0.7f, 0.7f, 1);
                newCard.transform.localRotation = Quaternion.Euler(0, 0, rotation);
                drawnCards.Add(newCard);
            }
            else
            {
                cardObjects[i].transform.localPosition = coords;
                cardObjects[i].transform.localRotation = Quaternion.Euler(0, 0, rotation);
            }
        }
    }
}
