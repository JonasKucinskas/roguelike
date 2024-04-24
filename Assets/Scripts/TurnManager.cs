using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnManager : MonoBehaviour
{
    private bool isPlayersMove = true;
    private const int initialPlayerMoves = 3;
    private const float initialPlayerTemperature = 35.5f;
    private const float maxPlayerTemperature = 45f;
    public PlayerHealth ph;
    public float temperatureLowerBy;
    int movesLeft;
    // Start is called before the first frame update
    void Start()
    {
        PlayerPrefs.SetInt("MovesLeft", initialPlayerMoves);
        PlayerPrefs.SetFloat("Temperature", initialPlayerTemperature);
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
    }

    public bool isPlayersTurn()
    {
        return isPlayersMove;
    }

    public void EndEnemyTurn()
    {
        isPlayersMove = true;
        PlayerPrefs.SetInt("MovesLeft", initialPlayerMoves);
        LowerTemperature(temperatureLowerBy);
    }

    public void SubtractPlayerMove()
    {
        movesLeft--;
        PlayerPrefs.SetInt("MovesLeft", movesLeft);
    }

    public void NewLevelPlayerTurnReset()
    {
        if (!isPlayersMove) EndEnemyTurn();
        else PlayerPrefs.SetInt("MovesLeft", initialPlayerMoves);
	}

    //temperature

    private void ChangeTemperature(float temperature)
	{
        float t = PlayerPrefs.GetFloat("Temperature");
        t += temperature;
        PlayerPrefs.SetFloat("Temperature", t);

        if (t < initialPlayerTemperature) t = initialPlayerTemperature;
        else if (t > maxPlayerTemperature) ph.TakeDamage(ph.currentHealth);
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