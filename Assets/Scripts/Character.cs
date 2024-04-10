using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public abstract class Character : MonoBehaviour
{
    public string characterName;
	public int hp;
	public int damage;
	public int xPosition;
    public int zPosition;    
    public bool isFriendly;

    public abstract bool CanMove(TileScript tile);

	private IEnumerator MoveToTile(TileScript targetTile)
	{
		Vector3 startPosition = transform.position; // Starting position
		float yOffset = startPosition.y - targetTile.transform.position.y;
		Vector3 endPosition = targetTile.transform.position + new Vector3(0, yOffset, 0); // Adjusted destination to maintain height

		float timeToMove = 0.8f; // Duration of the move in seconds, adjust as needed
		float elapsedTime = 0;

		while (elapsedTime < timeToMove)
		{
			float t = elapsedTime / timeToMove; // Normalized time
												// Apply easing function for smooth start and end
			float smoothStepT = t * t * (3f - 2f * t);

			// Interpolate position with easing
			transform.position = Vector3.Lerp(startPosition, endPosition, smoothStepT);
			elapsedTime += Time.deltaTime; // Update elapsed time
			yield return null; // Wait until next frame
		}

		// Ensure the character is exactly at the target position
		transform.position = endPosition;
	}

	public void Move(TileScript tile)
    {
        if (tile.IsOccupied() || !CanMove(tile))
        {
            Debug.LogWarning("Cant move here.");
            return;
        }
        
        TileScript originalTile = gameObject.transform.parent.GetComponent<TileScript>();
        
        if (isFriendly)
        {
            originalTile.SetFriendlyPresence(false);
            tile.SetFriendlyPresence(true);
        }
        else
        {
            originalTile.SetEnemyPresence(false);
            tile.SetEnemyPresence(true);
        }

        originalTile.IsSelected = false;//
        Debug.Log("Is Tile_" + originalTile.xPosition + "_" + originalTile.zPosition + " selected? " + originalTile.IsSelected);//

		StartCoroutine(MoveToTile(tile));

		gameObject.transform.SetParent(tile.gameObject.transform, false);

        xPosition = tile.xPosition;
        zPosition = tile.zPosition;

        int MovesLeft=PlayerPrefs.GetInt("MovesLeft");
        MovesLeft--;
        PlayerPrefs.SetInt("MovesLeft",MovesLeft);
    }
    
    public bool Attack(TileScript tile)
	{
        Enemy e = tile.GetComponentInChildren<Enemy>();
        if (!e)
        {
            Debug.Log("no enemy in attack tile");
            throw new Exception("no enemy in attack tile");
        }
        bool b=Attack(e);
        Debug.Log(b);
        tile.SetEnemyPresence(!b);
        return b;
    }
    private bool Attack(Enemy enemy)
	{
        hp--;
        bool isDead=enemy.TakeDamage(damage);
        int MovesLeft = PlayerPrefs.GetInt("MovesLeft");
        MovesLeft--;
        PlayerPrefs.SetInt("MovesLeft", MovesLeft);
        
        if(hp<=0)
        {
            Destroy(this.gameObject);
            this.GetComponentInParent<TileScript>().SetFriendlyPresence(false);
            this.GetComponentInParent<TileScript>().SetEnemyPresence(false);
        }
        return isDead;

    }
}
