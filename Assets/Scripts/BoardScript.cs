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
	GameObject lastHighlightedTile = null;
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
			if (!character)
			{
				// Try getting the Character component from the parent, assuming clickedObject is a child of the character
				character = clickedObject.transform.parent.GetComponent<Character>();
			}

			if (!character || !character.isFriendly)
			{
				Debug.Log("Clicked object is not a friendly character.");
				return; // Clicked not on a friendly character.
			}

			// Highlight the tile under the character
			TileScript tileUnderCharacter = character.transform.parent.GetComponent<TileScript>();
			if (tileUnderCharacter != null)
			{
				Debug.Log("Highlighting tile under character.");
				if (lastHighlightedTile != null)
				{
					lastHighlightedTile.GetComponent<TileScript>().RemoveHighlight();
				}
				tileUnderCharacter.Highlight();
				lastHighlightedTile = tileUnderCharacter.gameObject;
			}
			else
			{
				Debug.Log("Failed to find TileScript on character's parent.");
			}

			characterToMove = character;
			Debug.Log("Ready to move friendly object.");
		}
		else
		{
			TileScript tile = clickedObject.GetComponent<TileScript>();
			if (!tile)
			{
				// Try getting the TileScript component from the parent, assuming clickedObject is a child of the tile
				tile = clickedObject.transform.parent.GetComponent<TileScript>();
			}

			if (!tile)
			{
				Debug.Log("Clicked object is not a tile.");
				return; // Clicked not on a tile.
			}

			characterToMove.Move(tile); // Assuming this moves the character
			Debug.Log("Character moved.");
			if (lastHighlightedTile != null)
			{
				lastHighlightedTile.GetComponent<TileScript>().RemoveHighlight();
				lastHighlightedTile = null;
			}
			characterToMove = null;
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

			if (lastHighlightedTile != null)
			{
				// Access the TileScript component and call RemoveHighlight
				TileScript tileScript = lastHighlightedTile.GetComponent<TileScript>();
				if (tileScript != null)
				{
					tileScript.RemoveHighlight();
				}
				lastHighlightedTile = null; // Clear the reference to the last highlighted tile
			}

			characterToMove = null; // Clear the reference to the character to move
		}
	}

    void SpawnEnemy()
    {
        
    }
}