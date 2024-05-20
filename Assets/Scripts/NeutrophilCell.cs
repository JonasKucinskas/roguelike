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
    private int SpecialAttackDamageToEnemies=5;
    private int SpecialAttackDamageToFrendlies=5;
	private BoardScript boardScript; // for tutorial usage
    public bool hasUsedSpecialAttack; //for tutorial usage
	public static bool SpecialAttackIgnoresFriendlies = false;
	[SerializeField] Animator neutrAnimator;

    private float timeCounter = 0.0f;
    private float randomTime = 0.0f;

    private float animOffset;

    private void Start()
    {
        hp = 10;

		boardScript = GameObject.Find("Board").GetComponent<BoardScript>();

		if (boardScript.isTutorialLevel==1) damage = 100; //extra damage in the tutorial
        else damage = 10 + TimesExtraDamageAdded * DamageAdded;

		isFriendly = true;
        turnManager = GameObject.Find("TurnManager").GetComponent<TurnManager>();
        BoardManager = GameObject.Find("Board").GetComponent<BoardScript>();

        neutrAnimator = GetComponentInChildren<Animator>();
        animOffset = UnityEngine.Random.Range(0f, 20f);
        timeCounter = timeCounter - animOffset;
        if (neutrAnimator != null)
            neutrAnimator.Play("idle", 0, animOffset);
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
            //IdleSound();
        }

        timeCounter += Time.deltaTime;

        HpText.GetComponentInChildren<TextMeshPro>().text=hp.ToString();
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

	public override void SpecialAttack()
    {
        //kelia temperatura
        turnManager.AddTemperature(2f);//laikinai pakeista is 0.5 i 2

        hasUsedSpecialAttack = true;

        bool diceRollResult = false;
		if (SpecialAttackIgnoresFriendlies) diceRollResult = RollTheDice();

        if (isClicked == false)
        {
            if (audioManager != null)
                StartCoroutine(audioManager.PlaySound(audioManager.neutrophilSpecialAttack, 1.4f));
            else
                Debug.Log("AudioManager is null");

            turnManager.SubtractPlayerMove();
            
            //Einama, per visus 9 langelius (veikejo langeli ir 8 langelius aplink ji)
            for (int x = xPosition - 1; x <= xPosition + 1; x++)
            {
                for (int z = zPosition - 1; z <= zPosition + 1; z++)
                {
                    string tileName = "Tile_" + x + "_" + z;

                    if (x == xPosition && z == zPosition) //veikejo langelis
                    {
                        continue;
                    }
                    
                    GameObject tileObject = GameObject.Find(tileName); //ieskomas gretimas langelis
                    
                    if (tileObject != null)
                    {
                        var tile = tileObject.GetComponentInChildren<TileScript>();
                        if (tile.IsEnemyOnTile())
                        {
                            Debug.Log("Priesas atakuojamas x = " + x + " z = " + z);
                            Character enemy = tileObject.GetComponentInChildren<Character>();
							if (boardScript.isTutorialLevel==1) enemy.TakeDamage(100); //one shot units for the tutorial
							else enemy.TakeDamage(SpecialAttackDamageToEnemies);

							BoardManager.FinishAtack();
                        }
                        if (tile.IsFriendlyOnTile() == true)
                        {
							if (!SpecialAttackIgnoresFriendlies || !diceRollResult)
                            {
								Debug.Log("Draugiskas veikejas atakuojamas x = " + x + " z = " + z);
								Character friendly = tileObject.GetComponentInChildren<Character>();
								if (boardScript.isTutorialLevel==1) friendly.TakeDamage(100); //one shot units for the tutorial
                                else friendly.TakeDamage(SpecialAttackDamageToFrendlies);
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
			//DEAL DAMAGE TO SELF
			if (!SpecialAttackIgnoresFriendlies || !diceRollResult) this.TakeDamage(4);
            neutrAnimator.Play("simpleAttack");
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
    //    if (audioManager != null)
    //    {
    //        //StartCoroutine(audioManager.PlaySound(audioManager.neutrophilIdle1, 0.0f));
    //        StartCoroutine(audioManager.PlaySound(audioManager.neutrophilIdle2, 1.0f));
    //    }
    //    else
    //        Debug.Log("AudioManager is null");
    }

    public void SpawnDamageTakenParticles()
    {
        Instantiate(DamageTakenParticles,transform.position,Quaternion.Euler(0f, 0f, 0f));
    }

    public void playDamageAnimation()
    {
        neutrAnimator.Play("hurt");
    }

    //displays neutrophil's information window
    public override void ShowCharacterInfoWindow()
    {
        GameObject characterInfoWindow = GameObject.Find("MenuUI's").transform.Find("NeutrophilCardInformation").gameObject;
        characterInfoWindow.SetActive(true);

        Transform Child=GetChildWithName(characterInfoWindow.transform,"HealthValue");
        Child.gameObject.GetComponent<TextMeshProUGUI>().text=hp.ToString();

        Child=GetChildWithName(characterInfoWindow.transform,"AttackValue");
        Child.gameObject.GetComponent<TextMeshProUGUI>().text=damage.ToString();

        // Child=GetChildWithName(characterInfoWindow.transform,"ToFriendlyValue");
        // Child.gameObject.GetComponent<TextMeshProUGUI>().text=SpecialAttackDamageToFrendlies.ToString();

        // Child=GetChildWithName(characterInfoWindow.transform,"ToEnemyValue");
        // Child.gameObject.GetComponent<TextMeshProUGUI>().text=SpecialAttackDamageToEnemies.ToString();
    }

    public override void HideCharacterInfoWindow()
    {
        GameObject characterInfoWindow = GameObject.Find("MenuUI's").transform.Find("NeutrophilCardInformation").gameObject;
        characterInfoWindow.SetActive(false);
    }

    
}
