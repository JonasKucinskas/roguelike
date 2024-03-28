//Basic enemy type for now.
using System;
using TMPro;
using UnityEngine;

public class Enemy : Character
{
    public GameObject HpText;
	private void Start()
    {
		hp = 10;
        damage = 10;
        isFriendly = false;    
	}
    public override bool CanMove(TileScript tile)
    {
        int xMaxMovement = 2;

		if (Math.Abs(tile.zPosition - zPosition) > 0 || Math.Abs(tile.xPosition - xPosition) > xMaxMovement)
        {
            //move only in x axis by one tile.
            return false;
        }
        return true;
    }

    void Update()
    {
        HpText.GetComponentInChildren<TextMeshPro>().text=hp.ToString();
    }
}
