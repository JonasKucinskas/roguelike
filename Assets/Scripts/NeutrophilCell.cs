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

    private void Start()
    {
        hp = 10;
        damage = 10;
        isFriendly = true;
        turnManager = GameObject.Find("TurnManager").GetComponent<TurnManager>();
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
        HpText.GetComponentInChildren<TextMeshPro>().text=hp.ToString();
        string tileName = "Tile_" + xPosition + "_" + zPosition;
        GameObject tileObject = GameObject.Find(tileName);
        var tile = tileObject.GetComponent<TileScript>();

        if (Input.GetKey(KeyCode.Space ) && turnManager.isPlayersTurn() && tile.IsSelected)
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
            GameObject boardObject = GameObject.Find("Board");
            BoardScript board = boardObject.GetComponent<BoardScript>();
            turnManager.SubtractPlayerMove();
            //Einama, per visus 9 langelius (veikejo langeli ir 8 langelius aplink ji)
            for (int x = xPosition - 1; x <= xPosition + 1; x++)
            {
                for (int z = zPosition - 1; z <= zPosition + 1; z++)
                {
                    string tileName = "Tile_" + x + "_" + z;
                    //Debug.Log("Ie�komas langelis: " + tileName);

                    if (x == xPosition && z == zPosition) //veikejo langelis
                    {
                        //veikejo langelis
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
                                Enemy enemy = characterObject.GetComponent<Enemy>();
                                bool isDead = enemy.TakeDamage(5);

                                if (isDead == true)
                                {
                                    tile.SetEnemyPresence(false);
                                    board.RemoveEnemy(enemy);
                                }

                                board.FinishAtack();
                            }
                            if (tile.IsFriendlyOnTile() == true)
                            {
                                var characterObject = tileObject.transform.GetChild(0);
                                Debug.Log("Draugiskas veikejas atakuojamas x = " + x + " z = " + z);
                                NeutrophilCell friendly = characterObject.GetComponent<NeutrophilCell>();
                                bool isDead = friendly.TakeDamage(5);

                                if (isDead == true)
                                {
                                    tile.SetFriendlyPresence(false);
                                }
                                board.FinishAtack();
                            }
                        }
                        else
                        {
                            Debug.Log("Nera tokio langelio");
                        }
                    }
                }
            }
        }
    }

    public bool TakeDamage(int damage)
    {
        hp = hp - damage;

        if (hp <= 0)
        {
            Destroy(gameObject);
            return true;
        }
        return false;
    }
}
