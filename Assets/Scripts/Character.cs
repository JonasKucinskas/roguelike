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


    //Don't ask how this works i used chatgpt, i have no fucking clue.
    public List<Transform> GetTilesBetween(TileScript StartTile, TileScript EndTile)
    {
        List<Transform> tilesBetween = new List<Transform>();

        string[] StartTileCoords=StartTile.name.Split('_');
        string[] EndTileCoords=EndTile.name.Split('_');

        int[] Tilex=new int[2];
        int[] Tiley=new int[2];

        Tilex[0] = int.Parse(StartTileCoords[1]);
        Tiley[0] = int.Parse(StartTileCoords[2]);

        Tilex[1] = int.Parse(EndTileCoords[1]);
        Tiley[1] = int.Parse(EndTileCoords[2]);


        int dx = Math.Abs(Tilex[1] - Tilex[0]);
        int dy = Math.Abs(Tiley[1] - Tiley[0]);

        int sx = Tilex[0] < Tilex[1] ? 1 : -1;
        int sy = Tiley[0] < Tiley[1] ? 1 : -1;

        int err = dx - dy;
        int x = Tilex[0];
        int y = Tiley[0];

        while (true)
        {
            tilesBetween.Add(StartTile.transform.parent.Find("Tile_" + x + "_"+y));

            if (x == Tilex[1] && y == Tiley[1])
                break;

            int e2 = 2 * err;
            if (e2 > -dy)
            {
                err -= dy;
                x += sx;
            }
            if (e2 < dx)
            {
                err += dx;
                y += sy;
            }
        }

        return tilesBetween;
    }
    public void CheckMovePath(TileScript StartTile, TileScript EndTile)
    {
        List<Transform> TilesInBetween;
        TilesInBetween =GetTilesBetween(StartTile,EndTile);
        foreach(Transform tile in TilesInBetween)
        {
            if(isFriendly)
            {
                Transform Enemy=tile.Find("alien character(Clone)");
                if(Enemy!=null)
                {
                    tile.gameObject.GetComponent<TileScript>().SetEnemyPresence(false);
                    tile.parent.GetComponent<BoardScript>().enemies.Remove(Enemy.gameObject.GetComponent<Enemy>());
                    Destroy(Enemy.gameObject);
                }                
            }
            else
            {
                Transform Enemy=tile.Find("FootmanHP(Clone)");
                if(Enemy==null)
                {
                    Enemy=tile.Find("FootmanPBR(Clone)");                    
                }
                if(Enemy!=null)
                {
                    tile.gameObject.GetComponent<TileScript>().SetFriendlyPresence(false);
                    Destroy(Enemy.gameObject);
                }    
            }

        }
    }

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

        CheckMovePath(originalTile,tile);
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
        bool isDead=enemy.TakeDamage(damage);
        int MovesLeft = PlayerPrefs.GetInt("MovesLeft");
        MovesLeft--;
        PlayerPrefs.SetInt("MovesLeft", MovesLeft);
        
        return isDead;
    }
}
