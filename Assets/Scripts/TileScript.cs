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

	void Start()
    {
        rend = GetComponent<Renderer>();
        originalColor = rend.material.color;
		AllTiles.Add(this);
	}

	void OnDestroy()
	{
		// Remove this tile from the list of all tiles
		AllTiles.Remove(this);
	}

	void OnMouseEnter()
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

    public int GetXPosition()
    {
        return xPosition;
    }

    public int GetZPosition()
    {
        return zPosition;
    }

	public void RemoveChildren()
	{
		// Iterate over all children and destroy them
		for (int i = transform.childCount - 1; i >= 0; i--)
		{
			Destroy(transform.GetChild(i).gameObject);
		}

		// After removing children, reset presence flags
		SetEnemyPresence(false);
	}

	public bool CheckForEnemyInLastRow()
    {
        if (xPosition == 0 && IsEnemyPresent) return true;
        else return false;
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
}
