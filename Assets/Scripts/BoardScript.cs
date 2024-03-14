using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.EventSystems;
using System.Linq;

public class BoardScript : MonoBehaviour
{
    public GameObject TilePrefab;
    public GameObject enemyPrefab;

    public float Gap;
    public float Size;
    public int X;
    public int Z;
    private GameObject[,] tiles;
    private List<Enemy> enemies;
	GameObject lastHighlightedTile = null;
    private Character characterToMove;
    private TurnManager turnManager;
    
    // Start is called before the first frame update
    void Start()
    {
        turnManager = GameObject.Find("TurnManager").GetComponent<TurnManager>();
        enemies = new List<Enemy>();
        MakeBoard(X, Z);
        InitializeEnemies();
    }

    // Update is called once per frame
    void Update()
    {
        HandleFriendlyMovement();
        CheckForCancelMovement();
        HandleEnemyMovement();
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

    void InitializeEnemies()
    {
        System.Random random = new System.Random();

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

                SpawnEnemy(i, j);
            }
        }
    }

    void HandleFriendlyMovement()
    {
        
        if (!turnManager.isPlayersTurn())
        {
            return;
        }

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
			if (!character)
			{
				character = clickedObject.transform.parent.GetComponent<Character>();
			}

			if (!character || !character.isFriendly)
			{
				return;
			}

			TileScript tileUnderCharacter = character.transform.parent.GetComponent<TileScript>();
			if (tileUnderCharacter != null)
			{
				TileScript.HighlightTilesBasedOnOccupancy();
				tileUnderCharacter.Highlight();
				lastHighlightedTile = tileUnderCharacter.gameObject;
			}

			characterToMove = character;
		}
		else
		{
			TileScript tile = clickedObject.GetComponent<TileScript>();
			if (!tile)
			{
				tile = clickedObject.transform.parent.GetComponent<TileScript>();
			}

			if (tile)
			{
				characterToMove.Move(tile);
				TileScript.ResetTileHighlights();
				if (lastHighlightedTile != null)
				{
					lastHighlightedTile = null;
				}
				characterToMove = null;
			}
		}
	}

    void CheckForCancelMovement()
    {

        if (!characterToMove)
        {
            return;
        }

		if (Input.GetMouseButtonDown(1)) // Right-click to cancel
		{
			Debug.Log("Movement canceled");
        }
        if (lastHighlightedTile != null)
		{
			// Access the TileScript component and call RemoveHighlight
			TileScript tileScript = lastHighlightedTile.GetComponent<TileScript>();
			if (tileScript != null)
			{
				//tileScript.RemoveHighlight();
				TileScript.ResetTileHighlights();
			}
			lastHighlightedTile = null; // Clear the reference to the last highlighted tile
		}

		characterToMove = null; // Clear the reference to the character to move
	}

    void HandleEnemyMovement()
    {
        if (turnManager.isPlayersTurn())
        {
            return;
        }

        foreach (Enemy enemy in enemies)
        {
            //enemy goes forward, if there are no friendly characters in the way,
            //if there is friendly character in the way, 
            bool isObstacleInTheWay = false;
            for (int i = 0; i < enemy.xPosition; i++)
            {
                TileScript tileinfront = tiles[i, enemy.zPosition].GetComponent<TileScript>();

                if (tileinfront.IsFriendlyOnTile())
                {
                    isObstacleInTheWay = true;
                    break;
                }
            }

            if (!isObstacleInTheWay){
                TileScript tile = tiles[enemy.xPosition - 1, enemy.zPosition].GetComponent<TileScript>();
                enemy.Move(tile);
                turnManager.EndEnemyTurn();
                return;
            }
        }

        //all current enemies are blocked, look for a path and if found, spawn enemy there.
        for (int i = 0; i < Z; i++)
        {
            bool isObstacleInTheWay = false;
            for (int j = 0; j < X; j++)
            {
                TileScript tile = tiles[i, j].GetComponent<TileScript>();

                if (tile.IsFriendlyOnTile())
                {
                    isObstacleInTheWay = true;
                    break;
                }
            }

            if (!isObstacleInTheWay)
            {
                SpawnEnemy(X / 2, i);
                break;
            }
        }

        turnManager.EndEnemyTurn();
        
        //if all paths blocked for enemies, and there are no paths towards the end of the board, 
        //enemies do nothing.
    }

    void SpawnEnemy(int i, int j)
    {
        //this should not be here:
        float midx = ((X - 1) * Size + (X - 1) * Gap) / 2;
        float midz = ((Z - 1) * Size + (Z - 1) * Gap) / 2;

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
        
        enemies.Add(enemy);
        
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