using System;
using TMPro;
using UnityEngine;

public class Enemy : Character
{
    public GameObject HpText;

    public GameObject DamageTakenParticles;

    private float timeCounter = 0.0f;
    private float randomTime = 0.0f;

    private void Start()
    {
        damage = 7;
        isFriendly = false;    
	}
    private void Update()
    {
        if(hp>=0)
        {
            HpText.GetComponentInChildren<TextMeshPro>().text = hp.ToString();            
        }
        else
        {
            HpText.GetComponentInChildren<TextMeshPro>().text = "0";
        }

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
        bool zCoordsByOne = Math.Abs(tile.zPosition - zPosition) >= -1 && Math.Abs(tile.zPosition - zPosition) <= 1;
        bool xCoordsByOne = Math.Abs(tile.xPosition - xPosition) == 1;

		if (zCoordsByOne && xCoordsByOne)
        {
            //move forward only in x axis by one tile, and in z axis by one in each direction
            return true;
        }
        return false;
    }

    // public bool TakeDamage(int damage)
    // {
    //     //Debug.Log("Prieso hp pries ataka = " + hp);
    //     hp = hp - damage;
    //     //Debug.Log("Prieso hp po atakos = " + hp);

    //     if (hp <= 0)
    //     {
    //         Destroy(gameObject);
    //         return true;
    //     }
    //     return false;
    // }
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

    public void SpawnDamageTakenParticles()
    {
        Instantiate(DamageTakenParticles,transform.position,Quaternion.Euler(0f, 0f, 0f));
    }

    //show enemy's information window
    public override void ShowCharacterInfoWindow(){}
    public override void HideCharacterInfoWindow(){}

    public override void SpecialAttack()
    {
        throw new NotImplementedException();
    }
}
