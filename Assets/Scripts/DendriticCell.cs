using System;
using TMPro;
using UnityEngine;

public class DendriticCell : Character
{
    public GameObject HpText;
    private float timeCounter = 0.0f;
    private float randomTime = 0.0f;
    public static int TimesExtraDamageAdded = 0;
    private int DamageAdded = 2;

    private void Start()
    {
        hp = 10;
        damage = 5 + TimesExtraDamageAdded * DamageAdded;
        isFriendly = true;
    }

    void Update()
    {
        if (timeCounter > randomTime)
        {
            randomTime = 2.542f;
            timeCounter = 0.0f;
            IdleSound();
        }

        timeCounter += Time.deltaTime;
        HpText.GetComponentInChildren<TextMeshPro>().text=hp.ToString();
    }

	public static void AddExtraDamage()
	{
        TimesExtraDamageAdded++;
	}

	public override void NormalAttackSound()
    {
        if (audioManager != null)
        {
            StartCoroutine(audioManager.PlaySound(audioManager.dendriticAttack, 0f));
        }
        else
            Debug.Log("AudioManager is null");
    }

    public override void IdleSound()
    {
        if (audioManager != null)
        { 
            StartCoroutine(audioManager.PlaySound(audioManager.dendriticIdle1, 0f));
        }
        else
            Debug.Log("AudioManager is null");
    }

    public override bool CanMove(TileScript tile)
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
    }
}
