using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnManager : MonoBehaviour
{
    private bool isPlayersMove = true;
    public int initialPlayerMoves;

    int movesLeft;
    // Start is called before the first frame update
    void Start()
    {
        PlayerPrefs.SetInt("MovesLeft", initialPlayerMoves);
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
}