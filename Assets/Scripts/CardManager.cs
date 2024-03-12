using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class CardManager : MonoBehaviour
{
    public GameObject MovesLeftText;
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
                    RandomCard.SetSlotCoordinates(cardSlots[i].position);

                    if (i == 0)
                    {
                        RandomCard.transform.Rotate(0f, 0f, 45f, Space.Self);
                        RandomCard.transform.position += RandomCard.transform.TransformDirection(Vector3.down * 0.2f);
                    }
                    else if (i == 1)
                        RandomCard.transform.Rotate(0f, 0f, 20f, Space.Self);
                    else if (i == 2)
                    {
                        RandomCard.transform.position += RandomCard.transform.TransformDirection(Vector3.up * 0.065f);
                    }
                    else if (i == 3)
                        RandomCard.transform.Rotate(0f, 0f, -20f, Space.Self);
                    else if (i == 4)
                    {
                        RandomCard.transform.Rotate(0f, 0f, -45f, Space.Self);
                        RandomCard.transform.position += RandomCard.transform.TransformDirection(Vector3.down * 0.2f);
                    }

                    freeCardSlots[i]=false;
                    deck.Remove(RandomCard);
                    return;
                }
            }
        }
    }

    void Start()
    {
        PlayerPrefs.SetInt("MovesLeft",3);
        for(int i=0;i<freeCardSlots.Length;i++)
        DrawCard();
    }

    void Update()
    {
        MovesLeftText.GetComponent<TextMeshProUGUI>().text="Moves left: "+ PlayerPrefs.GetInt("MovesLeft").ToString();
        if(PlayerPrefs.GetInt("MovesLeft")==0)
        {
            Debug.LogWarning("ENEMY'S TURN");

            //AFTER ENEMYS TURN GIVE 3 MOVES AGAIN
            PlayerPrefs.SetInt("MovesLeft",3);           
        }

    }
}
