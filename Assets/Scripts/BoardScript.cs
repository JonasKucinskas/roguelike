using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using Random = UnityEngine.Random;

public class BoardScript : MonoBehaviour
{
	public GameObject TilePrefab;
	public GameObject enemyPrefab;


	public GameObject EnemySpawnParticle;

	private int X;
	private int Z;
	public GameObject[,] tiles;
	public List<Character> enemies;
	public List<Character> Frendlies;
	GameObject lastHighlightedTile = null;
	private Character characterToMove;
	private TurnManager turnManager;
	private Deck deck;
	private bool levelStarted = true; //made so that the game isnt won after completing a single level
	private int EnemyTurnCount = 2;
	private bool StartedEnemyTurn = false;
	public bool GameLost = false;
	public bool AllowPlayerInput = true;

	//this is so that while enemies are being spawned do not try to get win condition
	//since otherwise it insta-wins
	private bool EnemiesBeingSpawned=true;

	// Start is called before the first frame update
	void Start()
	{
		turnManager = GameObject.Find("TurnManager").GetComponent<TurnManager>();
		deck = GameObject.Find("Deck").GetComponent<Deck>();
		enemies = new List<Character>();
		MakeBoard();
		InitializeEnemies();
	}

	// Update is called once per frame
	void Update()
	{
		HandleFriendlyMovement();
		CheckForCancelMovement();
		HandleEnemyMovement();
		CheckWinConditions();
	}

	//Creates board
	void MakeBoard()
	{
		System.Random random = new System.Random();
		X = random.Next(3, 6);
		Z = random.Next(4, 8);
		tiles = new GameObject[X, Z]; 

		GameObject enviroment = transform.parent.gameObject;

		Renderer tileRenderer = TilePrefab.GetComponentInChildren<Renderer>();
		Vector3 tilePrefabSize = tileRenderer.bounds.size;

		Renderer boardRenderer = gameObject.GetComponentInChildren<Renderer>();
		Vector3 boardSize = boardRenderer.bounds.size;

		float boardX = X * tilePrefabSize.x;
		float boardZ = Z * tilePrefabSize.z;

		//scale scene's board to the board that we need to fit all the tiles.
		Vector3 boardScale = new Vector3(boardX / boardSize.x, 1f, boardZ / boardSize.z);
		enviroment.transform.localScale = boardScale;

		Vector3 startPos = new Vector3(-boardX / 2f + tilePrefabSize.z / 2f, -0.3f, -boardZ / 2f + tilePrefabSize.z / 3.5f);

		for (int i = 0; i < X; i++)
		{
			for (int j = 0; j < Z; j++)
			{
				Vector3 tilePosition = startPos + new Vector3(i * tilePrefabSize.x, 0, j * tilePrefabSize.z);

				GameObject tileGameObject = Instantiate(TilePrefab, tilePosition, Quaternion.identity);
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

				StartCoroutine(SpawnEnemy(i, j));
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
		if (EventSystem.current.IsPointerOverGameObject())
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
				if (clickedObject.transform.parent)
				{
					character = clickedObject.transform.parent.GetComponent<Character>();
				}
			}

			if (!character || !character.isFriendly)
			{
				return;
			}

			TileScript tileUnderCharacter = character.transform.parent.GetComponent<TileScript>();
			if (tileUnderCharacter != null)
			{
				//TileScript.HighlightTilesBasedOnOccupancy();
				TileScript.HighlightTilesBasedOnWalkable(character);

				tileUnderCharacter.Highlight();
				lastHighlightedTile = tileUnderCharacter.gameObject;
			}

			Debug.Log("Moving friendly");
			characterToMove = character;

			tileUnderCharacter.IsSelected = true;//
			Debug.Log("Is Tile_" + tileUnderCharacter.xPosition + "_" + tileUnderCharacter.zPosition + " selected? " + tileUnderCharacter.IsSelected);//
		}
		else
		{
			TileScript tile = clickedObject.GetComponent<TileScript>();
			if (!tile)
			{
				tile = clickedObject.transform.parent.GetComponentInChildren<TileScript>();
			}

			if (!tile)
			{
				return;
			}

			if (tile.IsEnemyOnTile())
			{
				if (characterToMove.CanMove(tile))
				{
					Character e = tile.transform.parent.GetComponentInChildren<Character>();
					lastHighlightedTile.GetComponentInChildren<Character>().Attack(e,characterToMove.damage);
					TileScript.ResetTileHighlights();

					if (lastHighlightedTile != null)
					{
						lastHighlightedTile = null;
					}

					characterToMove = null;
					tile.IsSelected = false;//
					Debug.Log("Is Tile_" + tile.xPosition + "_" + tile.zPosition + " selected? " + tile.IsSelected);//                    
				}
			}
			else
			{
				characterToMove.Move(tile);
				TileScript.ResetTileHighlights();

				if (lastHighlightedTile != null)
				{
					lastHighlightedTile = null;
				}
				characterToMove.GetComponentInParent<TileScript>().IsSelected=false;
				characterToMove = null;
				tile.IsSelected = false;//
				Debug.Log("Is Tile_" + tile.xPosition + "_" + tile.zPosition + " selected? " + tile.IsSelected);//
			}

		}

	}

	void CheckForCancelMovement()
	{

		if (!characterToMove)
		{
			return;
		}

		if (!Input.GetMouseButtonDown(1)) // Right-click to cancel
		{
			return;
		}
		if (lastHighlightedTile != null)
		{
			// Access the TileScript component and call RemoveHighlight
			TileScript tileScript = lastHighlightedTile.GetComponent<TileScript>();
			if (tileScript != null)
			{
				tileScript.RemoveHighlight();
				TileScript.ResetTileHighlights();
				tileScript.IsSelected = false;//
				Debug.Log("Is Tile_" + tileScript.xPosition + "_" + tileScript.zPosition + " selected? " + tileScript.IsSelected);
			}
			lastHighlightedTile = null; // Clear the reference to the last highlighted tile
		}

		Debug.Log("Movement canceled");
		characterToMove = null; // Clear the reference to the character to move
	}

	void HandleEnemyMovement()
	{

		if (turnManager.isPlayersTurn() || enemies.Count == 0 || StartedEnemyTurn)
		{
			return;
		}
		AllowPlayerInput=false;
		StartedEnemyTurn = true;
		StartCoroutine(EnemyMovement());
	}

	IEnumerator SpawnEnemy(int i, int j)
	{
		// Spawn enemy on top of the tile.
		GameObject tile = tiles[i ,j];
		Vector3 coordinates = new Vector3(tile.transform.position.x, 0.25f, tile.transform.position.z);
		
		Vector3 ParticleCoordinates=new Vector3(coordinates.x-0.25f, coordinates.y+8f,coordinates.z);
		//Spawn the particles on spawn
		Instantiate(EnemySpawnParticle,ParticleCoordinates, Quaternion.Euler(90f,0f, 0f));
        yield return new WaitForSeconds(1f);
		GameObject enemyObject = Instantiate(enemyPrefab.gameObject, coordinates, Quaternion.Euler(0f, -90f, 0f), tile.transform);

		// Set tile as parent.
		GameObject parentTile = tiles[i, j];
		enemyObject.transform.SetParent(parentTile.transform);

		Enemy enemy = enemyObject.GetComponent<Enemy>();
		enemy.characterName = $"enemy_{i}_{j}";
		enemy.xPosition = i;
		enemy.zPosition = j;
		enemy.hp = Random.Range(5, 15);

		enemies.Add(enemy);

		TileScript tileScript = tile.GetComponentInChildren<TileScript>();
		if (tileScript != null)
		{
			tileScript.SetEnemyPresence(true);
			Debug.Log($"Marking enemy presence on: {tile.name}");
		}
		else
		{
			Debug.LogError($"TileScript component not found on {tile.name} or its children. Make sure it's attached.");
		}
		EnemiesBeingSpawned=false;
	}

	IEnumerator EnemyMovement()
	{
		StartCoroutine(MoveEnemyTurnTextAcrossScreen("Opponents turn"));
		yield return new WaitForSeconds(3f);
		for (int w = 0; w < EnemyTurnCount; w++)
		{
			bool EnemyMoved = false;
			foreach (Enemy enemy in enemies)
			{
				//enemy goes forward, if there are no friendly characters in the way,
				//if there is friendly character in the way, it looks for a free path towards the end of the board
				// and spawns enemy on that path.
				bool isObstacleInTheWay = false;

				for (int i = 0; i < enemy.xPosition; i++)
				{
					TileScript tileinfront = tiles[i, enemy.zPosition].GetComponentInChildren<TileScript>();

					if (tileinfront.IsOccupied())
					{
						isObstacleInTheWay = true;
						break;
					}
				}

				bool isEnemyOnTheEdge = enemy.xPosition - 1 >= 0;

				if (!isObstacleInTheWay && isEnemyOnTheEdge)
				{
					TileScript tile = tiles[enemy.xPosition - 1, enemy.zPosition].GetComponentInChildren<TileScript>();
					Debug.Log(enemy.xPosition + "X " + enemy.zPosition + "Y" + " Is moving");
					enemy.Move(tile);
					EnemyMoved = true;
					yield return new WaitForSeconds(1f);
					break;
				}
			}

			if (!EnemyMoved)
			{
				//all current enemies are blocked, look for a free path and if found, spawn enemy there.
				for (int i = 0; i < Z; i++)
				{
					bool isObstacleInTheWay = false;
					for (int j = 0; j < X; j++)
					{
						TileScript tile = tiles[j, i].GetComponentInChildren<TileScript>();

						if (tile.IsOccupied())
						{
							isObstacleInTheWay = true;
							break;
						}
					}

					if (!isObstacleInTheWay)
					{
						StartCoroutine(SpawnEnemy(X / 2, i));
						yield return new WaitForSeconds(1f);
						break;
					}
				}
			}
		}

		turnManager.EndEnemyTurn();
		StartCoroutine(MoveEnemyTurnTextAcrossScreen("Players turn"));
		AllowPlayerInput = true;
		StartedEnemyTurn = false;
	}

	public void StartNewLevel()
	{
		EnemiesBeingSpawned=true;
		turnManager.NewLevelPlayerTurnReset();
		TileScript.ClearAllTiles();
		enemies = new List<Character>();
		Frendlies = new List<Character>();
		levelStarted = true;
		InitializeEnemies();
		
	}

	void CheckWinConditions()
	{
		if (enemies.Count == 0&&!EnemiesBeingSpawned)
		{
			if (FindFirstObjectByType<PlayerHealth>().currentHealth != 0)
			{
				if(levelStarted) FindAnyObjectByType<PauseMenu>().GetComponent<PauseMenu>().BonusSelectUI.SetActive(true);
				levelStarted = false;
			}
		}
		//THIS IS THE LOSE CONDITION
		if(GameObject.Find("Cards").transform.childCount==0 && deck.cards.Count == 0 && Frendlies.Count == 0)
		{
			FindAnyObjectByType<PauseMenu>().GetComponent<PauseMenu>().DefeatMenuUI.SetActive(true);
			GameLost=true;
		}
	}

	public void RemoveEnemy(Character enem)
	{
		enemies.Remove(enem);
	}

	public void RemoveFriendly(Character Char)
	{
		Frendlies.Remove(Char);
	}

	public void FinishAtack()
	{
		if (lastHighlightedTile != null)
		{
			// Access the TileScript component and call RemoveHighlight
			TileScript tileScript = lastHighlightedTile.GetComponent<TileScript>();
			if (tileScript != null)
			{
				tileScript.RemoveHighlight();
				TileScript.ResetTileHighlights();
				tileScript.IsSelected = false;//
				Debug.Log("Is Tile_" + tileScript.xPosition + "_" + tileScript.zPosition + " selected? " + tileScript.IsSelected);
			}
			lastHighlightedTile = null; // Clear the reference to the last highlighted tile
		}

		characterToMove = null;
		//Debug.Log("Attack done");
	}
	private IEnumerator MoveEnemyTurnTextAcrossScreen(string text)
	{
		if(GameLost)
		{
			yield break;
		}

		GameObject TextObject = FindAnyObjectByType<PauseMenu>().GetComponent<PauseMenu>().OpponentsTurnText;
		TextObject.GetComponent<TextMeshProUGUI>().text=text;
		RectTransform ObjTransform=TextObject.GetComponent<RectTransform>();
		Vector3 StartingPosition = ObjTransform.position;
		Vector3 MiddlePosition = new Vector3(554,ObjTransform.position.y,ObjTransform.position.z);
		Vector3 FinalPosition = new Vector3(1430,ObjTransform.position.y,ObjTransform.position.z);

		TextObject.SetActive(true);

		float timeToMove = 0.5f;
		float elapsedTime = 0;

		while (elapsedTime < timeToMove)
		{
			float t = elapsedTime / timeToMove;
			float smoothStepT = t * t * (3f - 2f * t);
			ObjTransform.position = Vector3.Lerp(StartingPosition,MiddlePosition,smoothStepT);
			elapsedTime += Time.deltaTime; // Update elapsed time
			yield return null; // Wait until next frame		
		}
		elapsedTime = 0;
		yield return new WaitForSeconds(1f);
		
		while (elapsedTime < timeToMove)
		{
			float t = elapsedTime / timeToMove;
			float smoothStepT = t * t * (3f - 2f * t);
			ObjTransform.position = Vector3.Lerp(MiddlePosition,FinalPosition,smoothStepT);
			elapsedTime += Time.deltaTime; // Update elapsed time
			yield return null; // Wait until next frame		
		}
		ObjTransform.position=StartingPosition;
		TextObject.SetActive(false);
	}
}