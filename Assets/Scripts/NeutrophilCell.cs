using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;

public class NeutrophilCell : Character
{
    public GameObject HpText;

    public GameObject DamageTakenParticles;

    private bool isClicked = false;
	public static int TimesExtraDamageAdded = 0;
	private int DamageAdded = 2;
    public static bool SpecialAttackIgnoresFriendlies = false;
	[SerializeField] Animator neutrAnimator;

    private float timeCounter = 0.0f;
    private float randomTime = 0.0f;

    private void Start()
    {
        hp = 10;
        damage = 10 + TimesExtraDamageAdded * DamageAdded;
        isFriendly = true;
        turnManager = GameObject.Find("TurnManager").GetComponent<TurnManager>();
        BoardManager = GameObject.Find("Board").GetComponent<BoardScript>();
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

    private void Update()
    {
        if (timeCounter > randomTime)
        {
            randomTime = 2.542f;
            timeCounter = 0.0f;
            IdleSound();
        }

        timeCounter += Time.deltaTime;

        HpText.GetComponentInChildren<TextMeshPro>().text=hp.ToString();
        string tileName = "Tile_" + xPosition + "_" + zPosition;
        GameObject tileObject = GameObject.Find(tileName);
        var tile = tileObject.GetComponentInChildren<TileScript>();

        if (Input.GetKey(KeyCode.Space ) && turnManager.isPlayersTurn() && tile.IsSelected && BoardManager.AllowPlayerInput)
        {
            ActivatePower();
            isClicked = true;
        }
        else
            isClicked = false;
    }

	public static void AddExtraDamage()
	{
		TimesExtraDamageAdded++;
	}

	private bool RollTheDice()
	{
		System.Random random = new System.Random();
		int randomNumber = random.Next(1, 11);

		if (randomNumber > 0) return true; //for now the chance is set to 100%
		else return false;
	}

	public void ActivatePower()
    {
        //kelia temperatura
        turnManager.AddTemperature(2f);//laikinai pakeista is 0.5 i 2

        //

        bool diceRollResult = false;
		if (SpecialAttackIgnoresFriendlies) diceRollResult = RollTheDice();

        if (isClicked == false)
        {
            if (audioManager != null)
                StartCoroutine(audioManager.PlaySound(audioManager.neutrophilSpecialAttack, 1.4f));
            else
                Debug.Log("AudioManager is null");

            turnManager.SubtractPlayerMove();
            neutrAnimator.Play("simpleAttack");
            //Einama, per visus 9 langelius (veikejo langeli ir 8 langelius aplink ji)
            for (int x = xPosition - 1; x <= xPosition + 1; x++)
            {
                for (int z = zPosition - 1; z <= zPosition + 1; z++)
                {
                    string tileName = "Tile_" + x + "_" + z;

                    if (x == xPosition && z == zPosition) //veikejo langelis
                    {
                    }
                    else
                    {
                        GameObject tileObject = GameObject.Find(tileName); //ieskomas gretimas langelis

                        if (tileObject != null)
                        {
                            var tile = tileObject.GetComponentInChildren<TileScript>();

                            if (tile.IsEnemyOnTile())
                            {
                                Debug.Log("Priesas atakuojamas x = " + x + " z = " + z);
                                Character enemy = tileObject.GetComponentInChildren<Character>();
                                enemy.TakeDamage(5);

                                BoardManager.FinishAtack();
                            }
                            if (tile.IsFriendlyOnTile() == true)
                            {
								if (!SpecialAttackIgnoresFriendlies || !diceRollResult)
                                {
									Debug.Log("Draugiskas veikejas atakuojamas x = " + x + " z = " + z);
									Character friendly = tileObject.GetComponentInChildren<Character>();
									friendly.TakeDamage(5);
								}

                                BoardManager.FinishAtack();
                            }
                            BoardManager.FinishAtack();
                        }
                        else
                        {
                            Debug.Log("Nera tokio langelio");
                            BoardManager.FinishAtack();
                        }
                    }
                }
            }
			//DEAL DAMAGE TO SELF
			if (!SpecialAttackIgnoresFriendlies || !diceRollResult) this.TakeDamage(4);
        }
    }

    // public bool TakeDamage(int damage)
    // {
    //     hp = hp - damage;

    //     if (hp <= 0)
    //     {
    //         this.GetComponentInParent<TileScript>().SetFriendlyPresence(false);
    //         Destroy(gameObject);
    //         return true;
    //     }
    //     return false;
    // }

    public override void NormalAttackSound()
    {
        if (audioManager != null)
        {
            StartCoroutine(audioManager.PlaySound(audioManager.neutrophilAttack, 0.0f));
        }
        else
            Debug.Log("AudioManager is null");
    }

    public override void IdleSound()
    {
        if (audioManager != null)
        {
            //StartCoroutine(audioManager.PlaySound(audioManager.neutrophilIdle1, 0.0f));
            StartCoroutine(audioManager.PlaySound(audioManager.neutrophilIdle2, 1.0f));
        }
        else
            Debug.Log("AudioManager is null");
    }

    public void SpawnDamageTakenParticles()
    {
        Instantiate(DamageTakenParticles,transform.position,Quaternion.Euler(0f, 0f, 0f));
    }

    //displays neutrophil's information window
    public override void ShowCharacterInfoWindow()
    {
        GameObject characterInfoWindow = GameObject.Find("MenuUI's").transform.Find("NeutrophilCardInformation").gameObject;
        characterInfoWindow.SetActive(true);
    }

    public override void HideCharacterInfoWindow()
    {
        GameObject characterInfoWindow = GameObject.Find("MenuUI's").transform.Find("NeutrophilCardInformation").gameObject;
        characterInfoWindow.SetActive(false);
    }
}
