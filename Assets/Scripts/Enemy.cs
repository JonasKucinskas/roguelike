//Basic enemy type for now.
using System;
using TMPro;
using UnityEngine;

public class Enemy : Character
{
    public GameObject HpText;

    private float timeCounter = 0.0f;
    private float randomTime = 0.0f;

    private void Start()
    {
        damage = 10;
        isFriendly = false;    
	}
    private void Update()
    {
        HpText.GetComponentInChildren<TextMeshPro>().text = hp.ToString();

        if (timeCounter > randomTime)
        {
            randomTime = UnityEngine.Random.Range(5.0f, 20.0f);
            timeCounter = 0.0f;
            IdleSound();
        }

        timeCounter += Time.deltaTime;
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

    public bool TakeDamage(int damage)
    {
        //Debug.Log("Prieso hp pries ataka = " + hp);
        hp = hp - damage;
        //Debug.Log("Prieso hp po atakos = " + hp);

        if (hp <= 0)
        {
            Destroy(gameObject);
            return true;
        }
        return false;
    }
    public override void NormalAttackSound()
    {
        if (audioManager != null)
        {
            StartCoroutine(audioManager.PlaySound(audioManager.virusAttack, 0.0f));
        }
        else
            Debug.Log("AudioManager is null");
    }

    public override void IdleSound()
    {
        if (audioManager != null)
        {
            StartCoroutine(audioManager.PlaySound(audioManager.virusIdle1, 0.0f));
            StartCoroutine(audioManager.PlaySound(audioManager.virusIdle2, UnityEngine.Random.Range(3.0f, 8.0f)));
        }
        else
            Debug.Log("AudioManager is null");
    }
}
