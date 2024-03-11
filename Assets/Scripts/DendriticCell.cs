using System;

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
        isFriendly = true;

        int zMaxMovement = 2;
        int xMaxMovement = 2;

		if (Math.Abs(tile.zPosition - zPosition) > zMaxMovement || Math.Abs(tile.xPosition - xPosition) > xMaxMovement)
        {
            //move only by x tiles in both directions.
            return false;
        }
        return true;
    }
}
