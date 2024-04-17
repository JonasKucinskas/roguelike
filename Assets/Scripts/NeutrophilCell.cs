using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;

public class NeutrophilCell : Character
{
    public GameObject HpText;
    private bool isClicked = false;
    private TurnManager turnManager;
    [SerializeField] Animator neutrAnimator;

    private float timeCounter = 0.0f;
    private float randomTime = 0.0f;

    private void Start()
    {
        hp = 10;
        damage = 10;
        isFriendly = true;
        turnManager = GameObject.Find("TurnManager").GetComponent<TurnManager>();
        BoardManager=GameObject.Find("Board").GetComponent<BoardScript>();
    }

    public override bool CanMove(TileScript tile)
    {
        int zMaxMovement = 1;
        int xMaxMovement = 1;

        if (Math.Abs(tile.zPosition - zPosition) > zMaxMovement || Math.Abs(tile.xPosition - xPosition) > xMaxMovement)
        {
            //move only by x tiles in both directions.
            return false;
        }
        return true;
    }

    private void Update()
    {
        if (timeCounter > randomTime)
        {
            randomTime = 2.542f;
            timeCounter = 0.0f;
            IdleSound();
        }

        timeCounter += Time.deltaTime;

        HpText.GetComponentInChildren<TextMeshPro>().text=hp.ToString();
        string tileName = "Tile_" + xPosition + "_" + zPosition;
        GameObject tileObject = GameObject.Find(tileName);
        var tile = tileObject.GetComponent<TileScript>();

        if (Input.GetKey(KeyCode.Space ) && turnManager.isPlayersTurn() && tile.IsSelected && BoardManager.AllowPlayerInput)
        {
            ActivatePower();
            isClicked = true;
        }
        else
            isClicked = false;
    }

    public void ActivatePower()
    {
        if (isClicked == false)
        {
            if (audioManager != null)
                StartCoroutine(audioManager.PlaySound(audioManager.neutrophilSpecialAttack, 1.4f));
            else
                Debug.Log("AudioManager is null");

            turnManager.SubtractPlayerMove();
            neutrAnimator.Play("simpleAttack");
            //Einama, per visus 9 langelius (veikejo langeli ir 8 langelius aplink ji)
            for (int x = xPosition - 1; x <= xPosition + 1; x++)
            {
                for (int z = zPosition - 1; z <= zPosition + 1; z++)
                {
                    string tileName = "Tile_" + x + "_" + z;
                    //Debug.Log("Ie�komas langelis: " + tileName);

                    if (x == xPosition && z == zPosition) //veikejo langelis
                    {
                    }
                    else
                    {
                        GameObject tileObject = GameObject.Find(tileName); //ieskomas gretimas langelis

                        if (tileObject != null)
                        {
                            var tile = tileObject.GetComponent<TileScript>();

                            if (tile.IsEnemyOnTile() == true)
                            {
                                var characterObject = tileObject.transform.GetChild(0);
                                Debug.Log("Priesas atakuojamas x = " + x + " z = " + z);
                                Character enemy = characterObject.GetComponent<Character>();
                                enemy.TakeDamage(5);

                                BoardManager.FinishAtack();
                            }
                            if (tile.IsFriendlyOnTile() == true)
                            {
                                var characterObject = tileObject.transform.GetChild(0);
                                Debug.Log("Draugiskas veikejas atakuojamas x = " + x + " z = " + z);
                                Character friendly = characterObject.GetComponent<Character>();
                                friendly.TakeDamage(5);

                                BoardManager.FinishAtack();
                            }
                            BoardManager.FinishAtack();
                        }
                        else
                        {
                            Debug.Log("Nera tokio langelio");
                            BoardManager.FinishAtack();
                        }
                    }
                }
            }
            //DEAL DAMAGE TO SELF
            this.TakeDamage(4);
        }
    }

    // public bool TakeDamage(int damage)
    // {
    //     hp = hp - damage;

    //     if (hp <= 0)
    //     {
    //         this.GetComponentInParent<TileScript>().SetFriendlyPresence(false);
    //         Destroy(gameObject);
    //         return true;
    //     }
    //     return false;
    // }

    public override void NormalAttackSound()
    {
        if (audioManager != null)
        {
            StartCoroutine(audioManager.PlaySound(audioManager.neutrophilAttack, 0.0f));
        }
        else
            Debug.Log("AudioManager is null");
    }

    public override void IdleSound()
    {
        if (audioManager != null)
        {
            //StartCoroutine(audioManager.PlaySound(audioManager.neutrophilIdle1, 0.0f));
            StartCoroutine(audioManager.PlaySound(audioManager.neutrophilIdle2, 1.0f));
        }
        else
            Debug.Log("AudioManager is null");
    }
}
