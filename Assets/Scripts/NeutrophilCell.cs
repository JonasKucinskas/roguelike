using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class NeutrophilCell : Character
{
    private bool isClicked = false;

    private void Start()
    {
        hp = 10;
        damage = 10;
        isFriendly = true;
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
        if (Input.GetMouseButton(1))
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

            Debug.Log("-----------------");

            //Einama, per visus 9 langelius (veikejo langeli ir 8 langelius aplink ji)
            for (int x = xPosition - 1; x <= xPosition + 1; x++)
            {
                for (int z = zPosition - 1; z <= zPosition + 1; z++)
                {
                    string tileName = "Tile_" + x + "_" + z;
                    Debug.Log("Ieškomas langelis: " + tileName);

                    if (x == xPosition && z == zPosition) //veikejo langelis
                    {
                        //sunaikinti veikeja arba  nieko nedaryti
                    }
                    else
                    {
                        GameObject tileObject = GameObject.Find(tileName); //ieskomas gretimas langelis

                        if (tileObject != null)
                        {
                            var tile = tileObject.GetComponent<TileScript>();

                            if (tile.IsEnemyOnTile() == true)
                            {
                                var characterObject = tileObject.transform.GetChild(0); //3 pozicijoje yra draugiskas veikejas
                                Debug.Log("Priesas atakuojamas x = " + x + " z = " + z);
                                Enemy enemy = characterObject.GetComponent<Enemy>();
                                bool isDead = enemy.TakeDamage(5);

                                if(isDead == true)
                                {
                                    tile.SetEnemyPresence(false);
                                    BoardScript board = boardObject.GetComponent<BoardScript>();
                                    board.RemoveEnemy(enemy);
                                }
                            }
                            if (tile.IsFriendlyOnTile() == true)
                            {
                                var character = tileObject.transform.GetChild(0); //1 pozicijoje yra prieso veikejas
                                Debug.Log("Draugiskas veikejas atakuojamas x = " + x + " z = " + z);
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
}
