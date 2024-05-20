using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class TileScript : MonoBehaviour
{
	public static List<TileScript> AllTiles = new List<TileScript>();
	public bool IsHighlighted { get; private set; } = false;
    private Color originalColor;
    private Renderer rend;
    private bool IsEnemyPresent = false;
    private bool isFriendlyPresent = false;
	public int xPosition;
    public int zPosition;
	private bool isStateHighlighted = false;
	public bool IsSelected { get; set; } = false;
	private BoardScript boardManager;
	void Start()
    {
        rend = GetComponent<Renderer>();
        originalColor = rend.material.color;
		AllTiles.Add(this);
		boardManager = GameObject.Find("Board").GetComponent<BoardScript>();
	}

	void OnDestroy()
	{
		// Remove this tile from the list of all tiles
		AllTiles.Remove(this);
	}

	void OnMouseEnter()
    {
		if (EventSystem.current.IsPointerOverGameObject() || isStateHighlighted)
		{
			return;
		}
		IsHighlighted = true;
		rend.material.color = Color.cyan; // Change to the temporary highlight color.
	}

    void OnMouseExit()
    {
        //Checks if clicked on UI (PauseMenu)
        if(EventSystem.current.IsPointerOverGameObject())
        {
            return;
        }
		if (EventSystem.current.IsPointerOverGameObject() || isStateHighlighted)
		{
			return;
		}
		IsHighlighted = false;
        rend.material.color = originalColor; // Change back to the original color.
    }

    public bool IsOccupied()
    {
        return isFriendlyPresent || IsEnemyPresent;
    }

    public bool IsEnemyOnTile()
    {
        return IsEnemyPresent;
    }

    public bool IsFriendlyOnTile()
    {
        return isFriendlyPresent;    
    }

    public void SetFriendlyPresence(bool isPresent)
    {
        isFriendlyPresent = isPresent;
    }

    public void SetEnemyPresence(bool isPresent)
    {
        IsEnemyPresent = isPresent;
    }

	public void ClearCharacterPresence()
	{
		Debug.Log("Clearing presence");
		isFriendlyPresent=false;
		IsEnemyPresent=false;
	}

    public int GetXPosition()
    {
        return xPosition;
    }

    public int GetZPosition()
    {
        return zPosition;
    }

	public static void ClearAllTiles()
	{
		foreach (TileScript tile in AllTiles)
		{
			if (tile != null)
			{
				tile.RemoveChildren();
			}
		}
	}
	public void RemoveChildren()
	{
		Debug.Log("Starting to remove children from: " + gameObject.name + " with child count: " + transform.childCount);

		for (int i = transform.childCount - 1; i >= 0; i--)
		{
			GameObject child = transform.GetChild(i).gameObject;
			Debug.Log("Destroying child: " + child.name);
			Destroy(child);
		}

		SetEnemyPresence(false);
		SetFriendlyPresence(false);
	}

	public bool CheckForEnemyInLastRow()
    {
        if (xPosition == 0 && IsEnemyPresent) return true;
        else return false;
    }
	public void CheckForFalseEnemyPresent()
	{
		if (IsEnemyPresent && !transform.GetComponentInChildren<Enemy>()) IsEnemyPresent = false;
	}
	public static bool IsEnemyInLastRow()
	{
		foreach (var tile in AllTiles)
		{
			if (tile.CheckForEnemyInLastRow())
			{
				Debug.Log("Enemy detected on tile (" + tile.xPosition + "," + tile.zPosition + ")");
				tile.GetComponentInParent<BoardScript>().enemies.Remove(tile.GetComponentInChildren<Enemy>());
				tile.RemoveChildren();
				foreach(var tile2 in AllTiles)
				{
					tile2.CheckForFalseEnemyPresent();
				}
				return true; // Enemy is in the last row
			}
		}
		return false; // No enemy in the last row
	}

	

	public void Highlight()
	{
		Debug.Log("Highlighting tile");
		//IsHighlighted = true;
		var renderer = GetComponent<Renderer>();
		if (renderer != null)
		{
			renderer.material.color = Color.yellow;
		}
	}

	public void RemoveHighlight()
	{
		// Ensure the tile has a Renderer component
		IsHighlighted = false;
		var renderer = GetComponent<Renderer>();
		if (renderer != null)
		{
			// Restore the original color
			renderer.material.color = originalColor;
		}
	}

	public void HighlightBasedOnOccupancy()
	{
		isStateHighlighted = true; // Now tracking highlight state
		if (IsOccupied())
		{
			rend.material.color = Color.red; // Occupied tiles highlighted in red
		}
		else
		{
			rend.material.color = Color.cyan; // Unoccupied tiles highlighted in white
		}
	}

	public static void GetWalkableTiles(Character character)
    {
		List<TileScript>tiles = new List<TileScript>();
        foreach (var tile in AllTiles)
        {
			if (character.GetComponent<Character>().CanMove(tile))
			{
                tiles.Add(tile);
            }
        }
    }
    public static void HighlightTilesBasedOnWalkable(Character character)
    {
        foreach (var tile in AllTiles)
        {
			if (character.GetComponent<Character>().CanMove(tile))
			{
                tile.HighlightBasedOnOccupancy();
            }
			else
			{
				tile.RemoveHighlight();
			}
        }
    }
    // New static method to reset tile highlights
    public static void ResetTileHighlights()
    {
        foreach (var tile in AllTiles)
        {
            tile.RemoveHighlight();
        }
    }
	//if info window had been hiden but a caharcter on the board is is selected - show info window
	public static void ShowInfoWindowIfSthIsSelected()
    {
		foreach(TileScript tile in AllTiles)
        {
			if(tile.IsSelected)
            {
				Character character=tile.transform.parent.GetComponentInChildren<Character>();
				if(character !=null)
				{
					character.ShowCharacterInfoWindow();					
				}

			}
        }
    }

	private void HighlightBasedOnPlacable(BoardScript boardManager)
	{
		isStateHighlighted = true; // Now tracking highlight state
		if (xPosition >= boardManager.GetMaxPlaceableX() || IsOccupied())
		{
			rend.material.color = Color.red; // Occupied tiles highlighted in red
		}
		else
		{
			rend.material.color = Color.cyan; // Unoccupied tiles highlighted in white
		}
	}

	public static void HighlightTilesBasedOnCardPlacable(BoardScript boardManager)
    {
        foreach (var tile in AllTiles)
        {
			tile.HighlightBasedOnPlacable(boardManager);
        }
    }
	public static void HighlightAll()
	{
		foreach (var tile in AllTiles)
		{
			tile.Highlight();
		}
	}
}
