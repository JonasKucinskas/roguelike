using System;
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

        Debug.Log("Start: "+StartTile.name);
        Debug.Log("End: "+EndTile.name);
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
            Transform Enemy=tile.Find("alien character(Clone)");
            if(Enemy!=null)
            {
                tile.gameObject.GetComponent<TileScript>().SetEnemyPresence(false);
                Destroy(Enemy.gameObject);
            }
        }
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

        gameObject.transform.SetParent(tile.gameObject.transform, false);

        int MovesLeft=PlayerPrefs.GetInt("MovesLeft");
        MovesLeft--;
        PlayerPrefs.SetInt("MovesLeft",MovesLeft);
    }
}
