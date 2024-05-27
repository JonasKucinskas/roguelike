using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Deck : MonoBehaviour
{
    public List<GameObject> originalCards = new List<GameObject>();
    public List<GameObject> cards = new List<GameObject>();
    private BoardScript boardManager;
    private AudioManager audioManager;

    void Start()
    {

        boardManager = GameObject.Find("Board").GetComponent<BoardScript>();
        audioManager = GameObject.Find("AudioManager").GetComponent<AudioManager>();
        foreach(GameObject card in cards)
        {
            originalCards.Add(card);
        }
        UpdateHeight();
    }

    void Update()
    {
    }

    private void UpdateHeight()
    {
        float totalHeight = cards.Count * 0.1f; 

        //transform.position = new Vector3(transform.position.x, transform.position.y - 0.1f, transform.position.z);
        transform.localScale = new Vector3(transform.localScale.x, totalHeight, transform.localScale.z);
        if(cards.Count<=0)
        {
            transform.position = new Vector3(transform.position.x, transform.position.y - 1f, transform.position.z);
        }

    }

    public GameObject Pop()
    {
        if(!boardManager.AllowPlayerInput)
		{
			return null;
		}

        if (cards.Count == 0)
        {
            return null;
        }
        StartCoroutine(audioManager.PlaySound(audioManager.CardPickup, 0.0f));
        GameObject lastElement = cards[cards.Count - 1];
        cards.RemoveAt(cards.Count - 1);
        UpdateHeight();
        return lastElement;
    }

    public void SwitchToOriginalCards()
    {
        cards.Clear();
        foreach (GameObject card in originalCards)
        {
            cards.Add(card);
        }
    }

    public void AddACard(GameObject go)
    {
        cards.Add(go);
        UpdateHeight();
    }
}
