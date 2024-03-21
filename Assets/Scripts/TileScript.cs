using System.Collections.Generic;
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

	public static void HighlightTilesBasedOnOccupancy()
    {
        foreach (var tile in AllTiles)
        {
            tile.HighlightBasedOnOccupancy();
        }
    }
    public static void HighlightTilesBasedOnWalkable(Character character)
    {
        foreach (var tile in AllTiles)
        {
			if (character.GetComponent<DendriticCell>().CanMove(tile))
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
