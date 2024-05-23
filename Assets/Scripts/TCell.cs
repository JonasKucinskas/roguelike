using System;
using TMPro;
using UnityEngine;

public class TCell : Character
{
    public GameObject HpText;
    public GameObject DamageTakenParticles;
    [SerializeField] Animator animator;

    private float timeCounter = 0.0f;
    private float randomTime = 0.0f;

    private void Start()
    {
        hp = 16;
        damage = 15;
        isFriendly = true;
    }

    void Update()
    {
        if (timeCounter > randomTime)
        {
            randomTime = 2.542f;
            timeCounter = 0.0f;
            //IdleSound();
        }

        timeCounter += Time.deltaTime;
        HpText.GetComponentInChildren<TextMeshPro>().text=hp.ToString();

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

    public override void NormalAttackSound()
    {
        if (audioManager != null)
        {
            StartCoroutine(audioManager.PlaySound(audioManager.dendriticAttack, 0f));
        }
        else
            Debug.Log("AudioManager is null");
    }

    public void SpawnDamageTakenParticles()
    {
        Instantiate(DamageTakenParticles,transform.position,Quaternion.Euler(0f, 0f, 0f));
    }

    public void playDamageAnimation()
    {
        animator.Play("hurt");
    }

    public override void HideCharacterInfoWindow()
    {
    }

    public override void IdleSound()
    {
    }

    public override void ShowCharacterInfoWindow()
    {
    }

    public override void SpecialAttack()
    {
    }
}