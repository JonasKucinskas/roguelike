using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.EventSystems;

public class BoardScript : MonoBehaviour
{
    public GameObject TilePrefab;
    public GameObject enemyPrefab;

    public float Gap;
    public float Size;
    public int X;
    public int Z;
    public int friendlyMovementLimit; //limit how many tiles can friendly character move in each direction. 
    private GameObject[,] tiles;

    private Character characterToMove;
    
    // Start is called before the first frame update
    void Start()
    {
        MakeBoard(X, Z);
        SpawnEnemies();
    }

    // Update is called once per frame
    void Update()
    {
        HandleFriendlyMovement();
        CheckForCancelMovement();
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
                GameObject tileGameObject = Instantiate(TilePrefab, coordinates, Quaternion.identity);
                tileGameObject.transform.parent = transform;
                tileGameObject.name = "Tile_" + i.ToString() + "_" + j.ToString();
                
                tiles[i, j] = tileGameObject; // Store the tile reference in the array
            
                TileScript tile = tileGameObject.GetComponentInChildren<TileScript>();
                tile.xPosition = i;
                tile.zPosition = j;
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

                // 50% chance to spawn an enemy.
                if (randomNumber > 50)
                {
                    continue;
                }

                // Spawn enemy on top of the tile.
                Vector3 coordinates = new Vector3(i * Size + i * Gap - midx, TilePrefab.transform.position.y + TilePrefab.transform.localScale.y, j * Size + j * Gap - midz);
                
                GameObject enemyObject = Instantiate(enemyPrefab.gameObject, coordinates, Quaternion.Euler(0f, -90f, 0f));
                
                // Set tile as parent.
                GameObject parentTile = tiles[i, j];
                enemyObject.transform.SetParent(parentTile.transform);             
                
                Enemy enemy = enemyObject.AddComponent<Enemy>();
                enemy.characterName = $"enemy_{i}_{j}";
                enemy.xPosition = i;
                enemy.zPosition = j;

                TileScript tileScript = parentTile.GetComponentInChildren<TileScript>();
                if (tileScript != null)
                {
                    tileScript.SetEnemyPresence(true);
                    Debug.Log($"Marking enemy presence on: {parentTile.name}"); // Confirm marking is intended.
                }
                else
                {
                    Debug.LogError($"TileScript component not found on {parentTile.name} or its children. Make sure it's attached.");
                }
            }
        }
    }

    void HandleFriendlyMovement(){
        
        if (!Input.GetMouseButtonDown(0))
        {
            //mouse not clicked
            return;
        }
        //Checks if clicked on UI (PauseMenu)
        if(EventSystem.current.IsPointerOverGameObject())
        {
            return;
        }
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (!Physics.Raycast(ray, out hit))
        {
            //no raycast
            return;
        }

        GameObject clickedObject = hit.collider.gameObject;
        Debug.Log("Clicked on: " + clickedObject.name);
        

        if (!characterToMove)
        {
            Character character = clickedObject.GetComponent<Character>();

            if (!character || !character.isFriendly)
            {
                //clicked not on friendly character.
                return;
            }

            characterToMove = character;
            Debug.Log("moving friendly object");
        }
        else
        {
            TileScript tile = clickedObject.GetComponent<TileScript>();
            if (!tile)
            {
                //clicked not on a tile.
                return;
            }

            characterToMove.Move(tile);
            characterToMove = null;
        }
    }

    void CheckForCancelMovement()
    {

        if (!characterToMove)
        {
            return;
        }

        if (Input.GetMouseButtonDown(1))
        {
            Debug.Log("Movement canceled");
            characterToMove = null;
        }
    }

    void SpawnEnemy()
    {
        
    }
}