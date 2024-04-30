using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
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
	private Character selectedCharacter;
	private TurnManager turnManager;
	private Deck deck;
	private bool levelStarted = true; //made so that the game isnt won after completing a single level
	private int EnemyTurnCount = 2;
	private bool StartedEnemyTurn = false;
	public bool GameLost = false;
	public bool AllowPlayerInput = true;
	public bool isTutorialLevel; //for a constant tutorial level
	public int ChanceToSpawnEnemies;
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
		HandleFriendlyAttack();
		CheckForCancelMovement();
		HandleEnemyMovement();
		CheckWinConditions();
	}

	private void HandleFriendlyAttack()
	{
		if (Input.GetKey(KeyCode.Space) && turnManager.isPlayersTurn() && AllowPlayerInput && selectedCharacter)
		{
			selectedCharacter.SpecialAttack();
		}
	}

	void MakeBoard()
	{
		System.Random random = new System.Random();
		if(!isTutorialLevel) 
		{
			X = random.Next(4, 6);
			Z = random.Next(4, 8);
		}
		else //constant tutorial level
		{
			X = 4;
			Z = 3;
		}
		tiles = new GameObject[X, Z]; 

		GameObject enviroment = transform.parent.gameObject;

		Renderer tileRenderer = TilePrefab.GetComponentInChildren<Renderer>();
		Vector3 tilePrefabSize = tileRenderer.bounds.size;

		Renderer boardRenderer = gameObject.GetComponentInChildren<Renderer>();
		Vector3 boardSize = boardRenderer.bounds.size;

		float boardX = X * tilePrefabSize.x;
		float boardZ = Z * tilePrefabSize.z;

		//scale scene's board gameobject to the required size board so that we could fit all the tiles.
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
		if(isTutorialLevel) //constant enemy spawns for tutorial
		{
			StartCoroutine(SpawnEnemy(2, 1));
			StartCoroutine(SpawnEnemy(3, 0));
			StartCoroutine(SpawnEnemy(3, 2));
		}
		else
		{
			System.Random random = new System.Random();

			for (int i = X / 2; i < X; i++)
			{
				for (int j = 0; j < Z; j++)
				{
					int randomNumber = random.Next(100);

					// 50% chance to spawn an enemy.
					if (randomNumber > ChanceToSpawnEnemies)
					{
						continue;
					}

					StartCoroutine(SpawnEnemy(i, j));
				}
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


		if (!selectedCharacter)
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

				//when the character is clicked on it's info is shown
				character.ShowCharacterInfoWindow();
			}

			Debug.Log("Moving friendly");
			selectedCharacter = character;

			tileUnderCharacter.IsSelected = true;//
			Debug.Log("Is Tile_" + tileUnderCharacter.xPosition + "_" + tileUnderCharacter.zPosition + " selected? " + tileUnderCharacter.IsSelected);//
		
			
		}
		else
		{
			TileScript tile = clickedObject.GetComponent<TileScript>();
			if (!tile)
			{
 				tile = clickedObject.transform.parent.GetComponentInChildren<TileScript>();

				if (!tile)
				{
					return;
				}
			}

			

			if (tile.IsEnemyOnTile() && selectedCharacter.CanMove(tile))
			{
				Character enemy = tile.transform.parent.GetComponentInChildren<Character>();
				Character friendly = lastHighlightedTile.GetComponentInChildren<Character>();

				friendly.Attack(enemy);
				
				Character.HideAllInfoWindows();

				if (lastHighlightedTile != null)
				{
					lastHighlightedTile = null;
				}

				selectedCharacter = null;
				tile.IsSelected = false;//
				Debug.Log("Is Tile_" + tile.xPosition + "_" + tile.zPosition + " selected? " + tile.IsSelected);//                    
			}
			else
			{
				selectedCharacter.Move(tile);
				Character.HideAllInfoWindows();

				if (lastHighlightedTile != null)
				{
					lastHighlightedTile = null;
				}

				selectedCharacter.GetComponentInParent<TileScript>().IsSelected=false;
				selectedCharacter = null;
				tile.IsSelected = false;//
				Debug.Log("Is Tile_" + tile.xPosition + "_" + tile.zPosition + " selected? " + tile.IsSelected);//
			}

		}

	}

	void CheckForCancelMovement()
	{

		if (!selectedCharacter)
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
				Character.HideAllInfoWindows();
				tileScript.IsSelected = false;//
				Debug.Log("Is Tile_" + tileScript.xPosition + "_" + tileScript.zPosition + " selected? " + tileScript.IsSelected);
			}
			lastHighlightedTile = null; // Clear the reference to the last highlighted tile
		}

		Debug.Log("Movement canceled");
		selectedCharacter = null; // Clear the reference to the character to move
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

	/// <summary>
	/// takes closes row possible and gets random enemy from it.
	/// </summary>
	/// <param name="xCoord">starting row</param>
	/// <returns></returns>
	private Character GetRandomClosestEnemy(int xCoord)
	{
		List<Character> filteredList = enemies.FindAll(obj => obj.xPosition == xCoord);

		if (filteredList.Count > 0)
		{
			System.Random rand = new System.Random();
			int randomIndex = rand.Next(0, filteredList.Count);
			return filteredList[randomIndex];
		}
		else return GetRandomClosestEnemy(xCoord + 1);
	}

	IEnumerator EnemyMovement()
	{
		StartCoroutine(MoveTextAcrossScreen("Opponents turn"));
		yield return new WaitForSeconds(3f);
		for (int w = 0; w < 1; w++)
		{
			bool EnemyMoved = false;
			Character enemy = GetRandomClosestEnemy(0);
			
			/*	
				enemy checks tiles like this :
				ZZZ
				 X
				
				X - enemy position
				Z - checked tiles
				
				enemy goes forward. If there is an obsticle in a way, 
				it checks tiles next to it, if one of them is free, it moves there.
				if all of them are blocked, it attacks weakest friendly character.
			*/
			
			//index out of bounds should not happen since enemies despawn on the last tile.
			TileScript tileinfront = tiles[enemy.xPosition - 1, enemy.zPosition].GetComponentInChildren<TileScript>();
			
			if (!tileinfront.IsOccupied())
			{
				enemy.Move(tileinfront);
				yield return new WaitForSeconds(1f);
				continue;
				//next move.
			}

			/*
				this loop moves through Z tiles
				ZZZ
				 X
			*/

			int minHealth = int.MaxValue;
			int minHealthCharacterIndex = 0;

			for (int j = enemy.zPosition - 1; j <= enemy.zPosition + 1; j++)
			{
				if (j < 0 || j > Z)
				{
					continue;
				}

				TileScript tile = tiles[enemy.xPosition - 1, j].GetComponentInChildren<TileScript>();
				if (!tile.IsOccupied())
				{
					//free tile found, move there.
					enemy.Move(tile);
					EnemyMoved = true;
					yield return new WaitForSeconds(1f);
					break;
				}
				else 
				{
					Character character = tile.GetComponentInChildren<Character>();
					if (character.hp < minHealth)
					{
						minHealthCharacterIndex = j;
						minHealth = character.hp;
					}
				}
			}

			//no free tiles, attack weakest friendly character.
			if (!EnemyMoved)
			{
				Character weakest = tiles[enemy.xPosition - 1, minHealthCharacterIndex].GetComponentInChildren<Character>();
				enemy.Attack(weakest);
			}

			/*
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
			*/
		}

		turnManager.EndEnemyTurn();
		if(!GameLost)
		{
			StartCoroutine(MoveTextAcrossScreen("Players turn"));			
		}
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
		//give player a card
		CardManager cardManager = GameObject.Find("CardManager").GetComponent<CardManager>();
		cardManager.ResetTheDeck();
		cardManager.DrawACard();
	}

	void CheckWinConditions()
	{
		if (enemies.Count == 0&&!EnemiesBeingSpawned)
		{
			PlayerHealth playerHealth =FindFirstObjectByType<PlayerHealth>();
			if (playerHealth!=null&&playerHealth.currentHealth!=0)
			{
				StartCoroutine(ShowWinScreenAfterDelay());
			}
		}
		//THIS IS THE LOSE CONDITION
		if(GameObject.Find("Cards").transform.childCount==0 && deck.cards.Count == 0 && Frendlies.Count == 0)
		{
			StartCoroutine(ShowLoseScreenAfterDelay());
		}
	}


	public IEnumerator ShowWinScreenAfterDelay()
	{
		GameObject CardsUI= GameObject.Find("CardsUI");
		for(int i=0;i<CardsUI.transform.childCount;i++)
		{
			GameObject Child = CardsUI.transform.GetChild(i).gameObject;
			if(Child.name=="Cards")
			{
				SetActiveAllChildren(Child,false);
			}
			else
			{
				Child.SetActive(false);
			}
		}
		StartCoroutine(MoveTextAcrossScreen("You won!"));
		yield return new WaitForSeconds(3f);
		if(levelStarted) FindAnyObjectByType<PauseMenu>().GetComponent<PauseMenu>().BonusSelectUI.SetActive(true);
		levelStarted = false;
	}

	public void SetActiveAllChildren(GameObject gameObject, bool State)
	{
		for (int i=0;i<gameObject.transform.childCount;i++)
		{
			gameObject.transform.GetChild(i).gameObject.SetActive(State);
		}
	}
	public IEnumerator ShowLoseScreenAfterDelay()
	{
		if(!GameLost)
		{
			StartCoroutine(MoveTextAcrossScreen("You lost!"));
			GameLost=true;
			yield return new WaitForSeconds(5f);
			FindAnyObjectByType<PauseMenu>().GetComponent<PauseMenu>().DefeatMenuUI.SetActive(true);			
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
				Character.HideAllInfoWindows();
				tileScript.IsSelected = false;//
				Debug.Log("Is Tile_" + tileScript.xPosition + "_" + tileScript.zPosition + " selected? " + tileScript.IsSelected);
			}
			lastHighlightedTile = null; // Clear the reference to the last highlighted tile
		}

		selectedCharacter = null;
		//Debug.Log("Attack done");
	}
	private IEnumerator MoveTextAcrossScreen(string text)
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