//Basic enemy type for now.
using System;

public class Enemy : Character
{
	private void Start()
    {
		hp = 10;
        damage = 10;
        isFriendly = false;    
	}
    public override bool CanMove(TileScript tile)
    {
        int xMaxMovement = 1;

		if (Math.Abs(tile.zPosition - zPosition) > 0 || Math.Abs(tile.xPosition - xPosition) > xMaxMovement)
        {
            //move only in x axis by one tile.
            return false;
        }
        return true;
    }
}
