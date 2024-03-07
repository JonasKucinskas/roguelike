using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class TileScript : MonoBehaviour
{
    public bool IsHighlighted { get; private set; } = false;
    private Color originalColor;
    private Renderer rend;
    private bool IsEnemyPresent = false;
    private bool isFriendlyPresent = false;

    void Start()
    {
        rend = GetComponent<Renderer>();
        originalColor = rend.material.color;
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
}
