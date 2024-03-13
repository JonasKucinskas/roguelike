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
        IsHighlighted = true;
        rend.material.color = Color.cyan; // Change to the highlight color.
    }

    void OnMouseExit()
    {
        //Checks if clicked on UI (PauseMenu)
        if(EventSystem.current.IsPointerOverGameObject())
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
			originalColor = renderer.material.color;
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
}
