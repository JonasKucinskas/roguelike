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
    private GameObject[,] tiles;

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
        tiles = new GameObject[x, z]; // Initialize the array with the board dimensions

        float midx = ((x - 1) * Size + (x - 1) * Gap) / 2;
        float midz = ((z - 1) * Size + (z - 1) * Gap) / 2;
        for (int i = 0; i < x; i++)
        {
            for (int j = 0; j < z; j++)
            {
                Vector3 coordinates = new Vector3(i * Size + i * Gap - midx, 0, j * Size + j * Gap - midz);
                GameObject tile = Instantiate(TilePrefab, coordinates, Quaternion.identity);
                tile.transform.parent = transform;
                tile.name = "Tile_" + i.ToString() + "_" + j.ToString();

                tiles[i, j] = tile; // Store the tile reference in the array
            }
        }
    }

    void SpawnEnemies()
    {
        System.Random random = new System.Random();

        float midx = ((X - 1) * Size + (X - 1) * Gap) / 2;
        float midz = ((Z - 1) * Size + (Z - 1) * Gap) / 2;

        for (int i = X / 2; i < X; i++)
        {
            for (int j = 0; j < Z; j++)
            {
                int randomNumber = random.Next(100);

                // 25% chance to spawn an enemy.
                if (randomNumber <= 25)
                {
                    // Spawn enemy on top of the tile.
                    Vector3 coordinates = new Vector3(i * Size + i * Gap - midx, TilePrefab.transform.position.y + TilePrefab.transform.localScale.y, j * Size + j * Gap - midz);

                    // Set "Board" as parent.
                    GameObject enemyObject = Instantiate(enemyPrefab.gameObject, coordinates, Quaternion.Euler(0f, -90f, 0f), gameObject.transform);
                    Enemy enemyInstance = enemyObject.GetComponent<Enemy>();
                    enemyInstance.enemyName = $"enemy_{i}_{j}";
                    enemyInstance.hp = 10;
                    enemyInstance.damage = 10;
                    enemyInstance.x = i;
                    enemyInstance.y = j;

                    // Find the corresponding tile by name and mark it.
                    string tileName = "Tile_" + i + "_" + j;
                    GameObject tile = GameObject.Find(tileName);

                    // Debugging: Log whether the tile is found or not.
                    Debug.Log(tile != null ? $"Tile found: {tileName}" : $"Tile NOT found: {tileName}");

                    if (tile != null)
                    {
                        TileScript tileScript = tile.GetComponentInChildren<TileScript>();

                        if (tileScript != null)
                        {
                            tileScript.MarkEnemyPresence();
                            Debug.Log($"Marking enemy presence on: {tileName}"); // Confirm marking is intended.
                        }
                        else
                        {
                            Debug.LogError($"TileScript component not found on {tileName} or its children. Make sure it's attached.");
                        }
                    }
                }
            }
        }
    }
}