//Basic enemy type for now.
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
        return true;
    }
}
