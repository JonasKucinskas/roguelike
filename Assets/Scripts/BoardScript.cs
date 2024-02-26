using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class BoardScript : MonoBehaviour
{
    public GameObject TilePrefab;
    public GameObject enemyPrefab;

    public float Gap;
    public float Size;
    public int X;
    public int Z;
    // Start is called before the first frame update
    void Start()
    {
        MakeBoard(X, Z);
        SpawnEnemies();
    }

    // Update is called once per frame
    void Update()
    {

    }
    //Creates board
    void MakeBoard(int x, int z)
    {
        float midx = ((x-1) * Size + (x-1) * Gap) / 2;
        float midz = ((z-1) * Size + (z-1) * Gap) / 2;
        for (int i = 0; i < x; i++)
        {
            for (int j = 0; j < z; j++)
            {
                Vector3 coordinates = new Vector3(i*Size+i*Gap-midx, 0, j*Size+j*Gap-midz);
                GameObject tile = Instantiate(TilePrefab, coordinates, Quaternion.identity);
                tile.transform.parent = transform;
                tile.name = "Tile_" + i.ToString() + "_" + j.ToString();
            }
        }
    }

    void SpawnEnemies()
    {
        System.Random random = new System.Random();

        float midx = ((X-1) * Size + (X-1) * Gap) / 2;
        float midz = ((Z-1) * Size + (Z-1) * Gap) / 2;

        for (int i = 0; i < X; i++)
        {
            for (int j = 0; j < Z; j++)
            {
                int randomNumber = random.Next(100);

                //25% chance to spawn an enemy.
                if (randomNumber <= 25)
                {
                    //spawn enemy on top of the tile.
                    Vector3 coordinates = new Vector3(i*Size+i*Gap-midx, TilePrefab.transform.position.y + TilePrefab.transform.localScale.y, j*Size+j*Gap-midz);
                    
                    //set "Board" as parent.
                    GameObject enemyObject = Instantiate(enemyPrefab.gameObject, coordinates, Quaternion.identity, gameObject.transform);
                    Enemy enemyInstance = enemyObject.GetComponent<Enemy>();
                    enemyInstance.enemyName = $"enemy_{i}_{j}";
                    enemyInstance.hp = 10;
                    enemyInstance.damage = 10;
                    enemyInstance.x = coordinates.x;
                    enemyInstance.y = coordinates.y;
                }
            }
        }
    }
}