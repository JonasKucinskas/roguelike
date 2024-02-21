using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CardManager : MonoBehaviour
{
  
    public List<Card> deck =new List<Card>();
    public Transform[] cardSlots;
    public bool[] freeCardSlots;

    public void DrawCard()
    {
        if(deck.Count >= 1)
        {
            Card RandomCard = deck[Random.Range(0,deck.Count)];
            
            for(int i = 0; i<freeCardSlots.Length; i++)
            {
                if(freeCardSlots[i]==true)
                {
                    RandomCard.gameObject.SetActive(true);
                    RandomCard.transform.position=cardSlots[i].position;
                    freeCardSlots[i]=false;
                    deck.Remove(RandomCard);
                    return;
                }
            }
        }
    }

    public void OnHover()
    {
        
    }
    // Start is called before the first frame update
    void Start()
    {
        for(int i=0;i<freeCardSlots.Length;i++)
        DrawCard();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
