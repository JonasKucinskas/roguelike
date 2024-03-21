using System;
using UnityEngine;

public class DendriticCell : Character
{
    private void Start()
    {
        hp = 10;
        damage = 10;
        isFriendly = true;
    }
    public override bool CanMove(TileScript tile)
    {
        int zMaxMovement = 1;
        int xMaxMovement = 1;

		if (Math.Abs(tile.zPosition - zPosition) > zMaxMovement || Math.Abs(tile.xPosition - xPosition) > xMaxMovement)
        {
            //move only by x tiles in both directions.
            return false;
        }
        return true;
    }

    /*public override bool CanMove(TileScript tile)
    {
        int zMaxMovement = 1;
        int xMaxMovement = 1;

        if (Math.Abs(tile.zPosition - zPosition) <= zMaxMovement && Math.Abs(tile.xPosition - xPosition) <= xMaxMovement)
        {
            //move only by x tiles in both directions.
            return true;
        }
        //zodziu patikrina ar tiesiai eina 2 atstumu (buves if tikrina 1 atstumu visom kryptim)
        if ((tile.zPosition == zPosition + 2 && tile.xPosition == xPosition) || (tile.zPosition == zPosition - 2 && tile.xPosition == xPosition)
            || (tile.xPosition == xPosition + 2 && tile.zPosition == zPosition) || (tile.xPosition == xPosition - 2 && tile.zPosition == zPosition)) return true;
        return false;
    }*/
}
