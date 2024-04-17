using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Deck : MonoBehaviour
{
    public List<GameObject> cards = new List<GameObject>();
    private BoardScript boardManager;

    void Start()
    {
        boardManager = GameObject.Find("Board").GetComponent<BoardScript>();
        UpdateHeight();
    }

    void Update()
    {
    }

    private void UpdateHeight()
    {
        float totalHeight = cards.Count * 0.1f; 

        if (cards.Count == 0)
        {
            //gameObject.SetActive(false);
            return;
        }
        //transform.position = new Vector3(transform.position.x, transform.position.y - 0.1f, transform.position.z);
        transform.localScale = new Vector3(transform.localScale.x, totalHeight, transform.localScale.z);
        transform.localPosition = new Vector3(transform.localPosition.x, transform.localPosition.y - 0.1f, transform.localPosition.z);

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

        GameObject lastElement = cards[cards.Count - 1];
        cards.RemoveAt(cards.Count - 1);
        UpdateHeight();
        return lastElement;
    }
}
