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
	public int isTutorialLevel; //for a constant tutorial level (Changed this to int to correspond to different tutorial sections 0 = not tutorial, 1 = first tutorial, 2 = force losing condition tutorial)
	public int ChanceToSpawnEnemies;
	//this is so that while enemies are being spawned do not try to get win condition
	//since otherwise it insta-wins
	private bool EnemiesBeingSpawned=true;
	private KeyCode specialAttack = KeyCode.Space;
	public int boardCount;

	public AudioManager audioManager;
	private bool IsInitial = true;

	// Start is called before the first frame update
	void Start()
	{
		boardCount = 0;
		Debug.Log(boardCount + " AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA");
		audioManager = GameObject.Find("AudioManager").GetComponent<AudioManager>();
		turnManager = GameObject.Find("TurnManager").GetComponent<TurnManager>();
		deck = GameObject.Find("Deck").GetComponent<Deck>();
		enemies = new List<Character>();
		MakeBoard();
		InitializeEnemies();
		LoadKeybinds();
	}
	void LoadKeybinds()
	{
		string[] keys = { "SpecialAttack" };

		foreach (string key in keys)
		{
			if (PlayerPrefs.HasKey(key))
			{
				switch (key)
				{
					case "SpecialAttack":
						specialAttack = (KeyCode)System.Enum.Parse(typeof(KeyCode), PlayerPrefs.GetString(key));
						break;
				}
			}
		}
	}

	// Update is called once per frame
	void Update()
	{
		HandleFriendlyMovement();
		HandleFriendlyAttack();
		HandleEnemyMovement();
		CheckWinConditions();
	}

	private void HandleFriendlyAttack()
	{
		if (Input.GetKey(specialAttack) && turnManager.isPlayersTurn() && AllowPlayerInput && selectedCharacter)
		{
			selectedCharacter.SpecialAttack();
			selectedCharacter = null;
		}
	}

	void MakeBoard()
	{
		System.Random random = new System.Random();
		if(isTutorialLevel==0) 
		{
			Debug.Log("Board check success, board count: " + boardCount);
			if(boardCount == 0)
            {
				X = random.Next(4, 6);
				Z = random.Next(3, 4);
			}
			else if (boardCount >= 1 && boardCount <= 2)
			{
				X = random.Next(4, 6);
				Z = random.Next(4, 6);
            }
			else
            {
				X = random.Next(5, 7);
				Z = random.Next(5, 7);
            }

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

		Debug.Log("X size: " + X);
		Debug.Log("Z size: " + Z);
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
				
				//make last row darker to indicate that player would take damage if enemy reached it.
				if (i == 0)
				{
					Material material = tileGameObject.GetComponentInChildren<Renderer>().material;
					Color originalColor = material.color;

					float darkenAmount = 0.3f; 

					Color darkerColor = new Color(
						Mathf.Max(originalColor.r - darkenAmount, 0f), 
						Mathf.Max(originalColor.g - darkenAmount, 0f),
						Mathf.Max(originalColor.b - darkenAmount, 0f),
						originalColor.a
					);

					material.color = darkerColor;
				}

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
		if(isTutorialLevel==1) //constant enemy spawns for tutorial
		{
			StartCoroutine(SpawnEnemy(2, 0));
			StartCoroutine(SpawnEnemy(2, 2));
			StartCoroutine(SpawnEnemy(3, 1));
		}
		else if(isTutorialLevel==2)
		{
			StartCoroutine(SpawnEnemy(2, 0));
			StartCoroutine(SpawnEnemy(2, 1));
			StartCoroutine(SpawnEnemy(2, 2));
			StartCoroutine(SpawnEnemy(3, 1));
		}
		else
		{
			System.Random random = new System.Random();
			int enemiesToSpawn = 0;

			if (boardCount == 0)
			{
				enemiesToSpawn = random.Next(2, 4); // Spawns between 2 and 3 enemies
			}
			else if (boardCount >= 1 && boardCount <= 2)
			{
				enemiesToSpawn = random.Next(3, 5); // Spawns between 3 and 4 enemies
			}
			else
			{
				enemiesToSpawn = random.Next(5, 10); // Spawns between 5 and 9 enemies
			}

			List<(int, int)> possibleTiles = new List<(int, int)>();

			for (int i = GetMaxPlaceableX(); i < X; i++)
			{
				for (int j = 0; j < Z; j++)
				{
					possibleTiles.Add((i, j));
				}
			}

			for (int i = 0; i < enemiesToSpawn; i++)
			{
				if (possibleTiles.Count == 0)
				{
					break;
				}

				int randomIndex = random.Next(possibleTiles.Count);
				(int x, int z) = possibleTiles[randomIndex];
				possibleTiles.RemoveAt(randomIndex);

				StartCoroutine(SpawnEnemy(x, z));
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

		Character character;
		if (clickedObject.GetComponent<TileScript>()) character = clickedObject.GetComponentInChildren<Character>(); //when tile is clicked
		else character = clickedObject.GetComponent<Character>(); //when character is clicked

		if (!selectedCharacter)
		{
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


				if (!tile || (selectedCharacter.xPosition == tile.xPosition &&
							 selectedCharacter.zPosition == tile.zPosition))
				{
					UnselectCharacter();
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
			else if(!selectedCharacter.CanMove(tile))
			{
				UnselectCharacter();
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

	private void UnselectCharacter()
	{

		if (!selectedCharacter)
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

	public IEnumerator SpawnEnemy(int i, int j)
	{
		// Spawn enemy on top of the tile.
		GameObject tile = tiles[i ,j];
		Vector3 coordinates = new Vector3(tile.transform.position.x, 0.25f, tile.transform.position.z);
		
		Vector3 ParticleCoordinates=new Vector3(coordinates.x-0.25f, coordinates.y+8f,coordinates.z);
		//Spawn the particles on spawn
		Instantiate(EnemySpawnParticle,ParticleCoordinates, Quaternion.Euler(90f,0f, 0f));
		if (IsInitial)
		{
			StartCoroutine(audioManager.PlaySound(audioManager.spawning, 0.0f));
			IsInitial = false;
		}
		yield return new WaitForSeconds(1f);
		GameObject enemyObject = Instantiate(enemyPrefab.gameObject, coordinates, Quaternion.Euler(0f, -90f, 0f), tile.transform);

		// Set tile as parent.
		GameObject parentTile = tiles[i, j];
		enemyObject.transform.SetParent(parentTile.transform);

		Enemy enemy = enemyObject.GetComponent<Enemy>();
		enemy.characterName = $"enemy_{i}_{j}";
		enemy.xPosition = i;
		enemy.zPosition = j;
		if (boardCount == 0)
		{
			enemy.hp = Random.Range(5, 10);
		}
		else if (boardCount >= 1 && boardCount <= 2)
		{
			enemy.hp = Random.Range(8, 12);
		}
		else
		{
			enemy.hp = Random.Range(10, 25);
		}
		

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
	/// takes random enemy from two closest possible rows.
	/// </summary>
	/// <param name="xCoord">starting row</param>
	/// <returns></returns>
	private Character GetRandomEnemy(int xCoord)
	{
		int enemiesToChooseFrom = 2;
		if (isTutorialLevel == 1)
		{
			enemiesToChooseFrom = 1;
		}

		List<Character> filteredList = enemies.FindAll(obj => obj.xPosition <= xCoord);

		if (filteredList.Count == enemiesToChooseFrom || xCoord > X)
		{
			System.Random rand = new System.Random();
			int randomIndex = rand.Next(0, filteredList.Count);
			return filteredList[randomIndex];
		}
		else return GetRandomEnemy(xCoord + 1);
	}

	IEnumerator EnemyMovement()
	{
		StartCoroutine(MoveTextAcrossScreen("Opponents turn"));
		int temp = 0;
		if (turnManager.effectActive[1])
		{
			temp = 1;
		}
		
		int attackChance = 70;

		if (isTutorialLevel == 1)
		{
			attackChance = 0;
			//this is made so that enemy has enough moves to harm player in the tutorial.
			//enemies are hard coded to spawn at x = 2, to just subtract 2 from x.
			EnemyTurnCount = X - 2;
		}

		yield return new WaitForSeconds(3f);
		for (int w = 0; w < EnemyTurnCount-temp; w++)
		{
			Character enemy = GetRandomEnemy(0);
			
			/*	
				enemy checks tiles like this :
				ZZZ
				 X
				
				X - enemy position
				Z - checked tiles
				
				enemy goes forward. if it finds friendly characters, theres a chance it attacks them
			*/
			
			List<TileScript> freeTiles = new List<TileScript>();
			bool enemyAttacked = false;

			/*
				this loop moves through Z tiles
				ZZZ
				 X
			*/
			System.Random random = new System.Random();
			for (int j = enemy.zPosition - 1; j <= enemy.zPosition + 1; j++)
			{
				if (j < 0 || j >= Z)
				{
					continue;
				}

				TileScript tile = tiles[enemy.xPosition - 1, j].GetComponentInChildren<TileScript>();

				Character character = tile.GetComponentInChildren<Character>();
				if (character && character is not Enemy)
				{
					int randomNum = random.Next(100);

					//chance to attack friendly character
					if (randomNum < attackChance)
					{
						enemy.Attack(character);
						enemyAttacked = true;
						yield return new WaitForSeconds(2f);
						break;
					}
				}

				if (!tile.IsOccupied())
				{
					freeTiles.Add(tile);
				}
			}

			if (!enemyAttacked)
			{
				if (freeTiles.Count == 0)
				{
					yield return new WaitForSeconds(1f);
					continue;
				}
        		int randomIndex = random.Next(freeTiles.Count);
        		TileScript randomtile = freeTiles[randomIndex];

				enemy.Move(randomtile);

				//if enemy moved to last tile, despawn it
				if (randomtile.xPosition == 0)
				{
					//wait for as long as enemy takes to move to the tile
					yield return new WaitForSeconds(1.8f);
					randomtile.ClearCharacterPresence();
					RemoveEnemy(enemy);
					AllowPlayerInput=false;
					//IT DESTROYS THE ENEMY OBJECT IN THE METHOD BELOW
					StartCoroutine(enemy.GetComponent<ParticleTest>().StartImplosionEffect());
					StartCoroutine(Camera.main.GetComponent<CameraEffects>().ShakeCamera());
					AllowPlayerInput=true;
				}

				yield return new WaitForSeconds(1f);
			}
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
		///These cause the turns to overflow into the negatives
		cardManager.LevelStart = true;
        cardManager.ResetTheDeck();
		///cardManager.DrawACard();
        cardManager.LevelStart = false;
        boardCount++;
		Debug.Log(boardCount + " <- board count!");
	}

	void CheckWinConditions()
	{
		//THIS IS THE LOSE CONDITION
		if (GameObject.Find("Cards").transform.childCount == 0 && deck.cards.Count == 0 && Frendlies.Count == 0)
		{
			StartCoroutine(ShowLoseScreenAfterDelay());
		}
		if (enemies.Count == 0&&!EnemiesBeingSpawned)
		{
			PlayerHealth playerHealth =FindFirstObjectByType<PlayerHealth>();
			if (playerHealth!=null&&playerHealth.currentHealth!=0)
			{
				StartCoroutine(ShowWinScreenAfterDelay());
			}
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
			GameObject DefeatMenu = FindAnyObjectByType<PauseMenu>().GetComponent<PauseMenu>().DefeatMenuUI;			
			yield return new WaitForSeconds(2f);
			if(isTutorialLevel!=2)
			{
				DefeatMenu.SetActive(true);
			}
			else
			{
				DefeatMenu.SetActive(true);
				Transform TopButton = GetChildWithTag(DefeatMenu.transform,"GameEndMenuTopButton");
				TopButton.gameObject.SetActive(false);
			}
						
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
				Debug.Log("Is Tile_" + tileScript.GetXPosition() + "_" + tileScript.GetZPosition() + " selected? " + tileScript.IsSelected);
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
		Vector3 StartingPosition = ObjTransform.localPosition;
		Vector3 MiddlePosition = new Vector3(0,ObjTransform.localPosition.y,ObjTransform.localPosition.z);
		Vector3 FinalPosition = new Vector3(1000,ObjTransform.localPosition.y,ObjTransform.localPosition.z);

		TextObject.SetActive(true);

		float timeToMove = 0.5f;
		float elapsedTime = 0;

		while (elapsedTime < timeToMove)
		{
			float t = elapsedTime / timeToMove;
			float smoothStepT = t * t * (3f - 2f * t);
			ObjTransform.localPosition = Vector3.Lerp(StartingPosition,MiddlePosition,smoothStepT);
			elapsedTime += Time.deltaTime; // Update elapsed time
			yield return null; // Wait until next frame		
		}
		elapsedTime = 0;
		yield return new WaitForSeconds(1f);
		
		while (elapsedTime < timeToMove)
		{
			float t = elapsedTime / timeToMove;
			float smoothStepT = t * t * (3f - 2f * t);
			ObjTransform.localPosition = Vector3.Lerp(MiddlePosition,FinalPosition,smoothStepT);
			elapsedTime += Time.deltaTime; // Update elapsed time
			yield return null; // Wait until next frame		
		}
		yield return new WaitForSeconds(.5f);
		ObjTransform.localPosition=StartingPosition;
		TextObject.SetActive(false);
	}
	public void dmgAll(int damage)
	{
		foreach(Enemy en in enemies)
		{
			en.TakeDamage(damage);
		}
		foreach (Character ch in Frendlies)
		{
			ch.TakeDamage(damage);
		}
	}

	public int GetMaxPlaceableX()
	{
		return X / 2;
	}

	//Find the first child with the specified tag.
    public Transform GetChildWithTag(Transform transform,string tag)
    {
        if (transform.childCount == 0)
        {
            return null;
        }
        for (int i = 0; i < transform.childCount; i++)
        {
            if (transform.GetChild(i).CompareTag(tag))
            {
                return transform.GetChild(i);
            }
        }
        return null;
    }

}