using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnManager : MonoBehaviour
{
    private bool isPlayersMove = true;
    public int initialPlayerMoves;
    public static int totalMovesMade = 0;
    private const float initialPlayerTemperature = 35.5f;
    private const float maxPlayerTemperature = 45f;
    public PlayerHealth ph;
    public float temperatureLowerBy;
    int movesLeft;

    private BoardScript bs;

    private float timer=0f;
    private float temperatureUpdateInterval=0.1f;
    // Start is called before the first frame update
    void Start()
    {
        PlayerPrefs.SetInt("MovesLeft", initialPlayerMoves);
        PlayerPrefs.SetFloat("Temperature", initialPlayerTemperature);
        PlayerPrefs.SetFloat("TemperatureDisplay", initialPlayerTemperature);
        effectActive = new bool[3] { false, false, false };
    }

    // Update is called once per frame
    void Update()
    {
        movesLeft = PlayerPrefs.GetInt("MovesLeft");
        if(movesLeft == 0)
        {
            //Debug.Log("ENEMY'S TURN");
            isPlayersMove = false;
        }

        //makes temperature change slower
        timer += Time.deltaTime;
        if (timer > temperatureUpdateInterval)
        {
            Debug.Log("Update!!!"); 
            float temperatureActual = PlayerPrefs.GetFloat("Temperature");
            float temperatureDisplayed = PlayerPrefs.GetFloat("TemperatureDisplay");
            if (temperatureDisplayed != temperatureActual)
            {
                if (temperatureDisplayed > temperatureActual)//krenta
                {
                    temperatureDisplayed -= Mathf.Min(temperatureDisplayed - temperatureActual, 0.1f);
                }
                else
                {
                    temperatureDisplayed += Mathf.Min(temperatureActual - temperatureDisplayed, 0.1f);
                }
                temperatureDisplayed = (float)System.Math.Round(temperatureDisplayed, 1);
            }   
            PlayerPrefs.SetFloat("TemperatureDisplay", temperatureDisplayed);
            timer = 0;
        }
    }

    public bool isPlayersTurn()
    {
        return isPlayersMove;
    }

    public void EndEnemyTurn()
    {
        isPlayersMove = true;
        totalMovesMade++;
        PlayerPrefs.SetInt("MovesLeft", initialPlayerMoves);
        LowerTemperature(temperatureLowerBy);
		if (effectActive[2])
		{
			if (!bs)
			{
                bs = GameObject.Find("Board").GetComponent<BoardScript>();
            }
            bs.dmgAll(2);
        }
    }

    public void SubtractPlayerMove()
    {
        movesLeft--;
        totalMovesMade++;
        PlayerPrefs.SetInt("MovesLeft", movesLeft);
    }

    public void NewLevelPlayerTurnReset()
    {
        if (!isPlayersMove) EndEnemyTurn();
        else PlayerPrefs.SetInt("MovesLeft", initialPlayerMoves);
        totalMovesMade = 0;
	}

    //temperature

    private void ChangeTemperature(float temperature)
	{
        float t = PlayerPrefs.GetFloat("Temperature");
        float tt = t;
        t += temperature;

        if (t < initialPlayerTemperature) t = initialPlayerTemperature;
        else if (t > maxPlayerTemperature) {
			if (!ph)
			{
                ph=GameObject.Find("PlayerHealthIndicator").GetComponent<PlayerHealth>();
            }
            ph.TakeDamage(ph.currentHealth); 
        }
        setEffect(tt, t);
        PlayerPrefs.SetFloat("Temperature", t);

    }
    public bool[] effectActive;
    private void setEffect(float oldT, float newT)
	{
        float[] temperatureSteps = new float[] { 37f, 39f, 41f };
        //if((oldT<37 && newT<37)||(oldT>41 && newT > 41)) { }
        if (oldT < newT)//kyla
		{
            for(int i=0; i<3; i++)
			{
                if(oldT<temperatureSteps[i] && newT > temperatureSteps[i])
				{
                    Debug.Log("Aktyvuojamas " + i.ToString() + " efektas");
                    effectActive[i] = true;
				}
			}
		}
		else//leidziasi
		{
            for (int i = 0; i < 3; i++)
            {
                if (oldT > temperatureSteps[i] && newT < temperatureSteps[i])
                {
                    effectActive[i] = false;
                }
            }
        }
	}
    public void AddTemperature(float temperature)
	{
        ChangeTemperature(temperature);
	}
    public void LowerTemperature(float temperature)
    {
        ChangeTemperature(-temperature);
    }

}