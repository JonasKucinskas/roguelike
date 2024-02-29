using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileScript : MonoBehaviour
{
    public bool IsHighlighted { get; private set; } = false;
    private Color originalColor;
    private Renderer rend;
    public bool IsEnemyPresent = false;

    void Start()
    {
        rend = GetComponent<Renderer>();
        originalColor = rend.material.color;
    }

    void OnMouseEnter()
    {
        IsHighlighted = true;
        rend.material.color = Color.cyan; // Change to the highlight color.
    }

    void OnMouseExit()
    {
        IsHighlighted = false;
        rend.material.color = originalColor; // Change back to the original color.
    }

    public static HashSet<Transform> OccupiedSlots = new HashSet<Transform>();

    public static bool IsSlotOccupied(Transform slotTransform)
    {
        return OccupiedSlots.Contains(slotTransform);
    }

    public static void OccupySlot(Transform slotTransform)
    {
        OccupiedSlots.Add(slotTransform);
    }

    public static void FreeSlot(Transform slotTransform)
    {
        OccupiedSlots.Remove(slotTransform);
    }

    // Call this method when you spawn an enemy on this tile
    public void MarkEnemyPresence()
    {
        IsEnemyPresent = true;
        Debug.Log("Marking enemy presence on: " + gameObject.name);
    }

    // Utility method to check if this tile has an enemy
    public static bool IsEnemyOnTile(Transform tileTransform)
    {
        TileScript tileScript = tileTransform.GetComponent<TileScript>();
        if (tileScript != null)
        {
            return tileScript.IsEnemyPresent;
        }
        return false;
    }
}
