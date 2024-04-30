using System;

public class TCell : Character
{
    private void Start()
    {
        hp = 16;
        damage = 15;
        isFriendly = true;
    }
    public override bool CanMove(TileScript tile)
    {
        throw new NotImplementedException();
    }

    public override void HideCharacterInfoWindow()
    {
        throw new NotImplementedException();
    }

    public override void IdleSound()
    {
        throw new NotImplementedException();
    }

    public override void NormalAttackSound()
    {
        throw new NotImplementedException();
    }

    public override void ShowCharacterInfoWindow()
    {
        throw new NotImplementedException();
    }

    public override void SpecialAttack()
    {
        throw new NotImplementedException();
    }
}
