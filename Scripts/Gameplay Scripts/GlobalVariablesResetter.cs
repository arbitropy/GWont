using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalVariablesResetter : MonoBehaviour
{
    void Awake()
    {
        GlobalVariables.showingInfo = false;
        GlobalVariables.instantiationPosition = new Vector3(0, 5.5f);
        GlobalVariables.moving = false;
        GlobalVariables.weather = new List<bool>(3);
        GlobalVariables.playerTurn = true;
        GlobalVariables.round = 1;
        GlobalVariables.roundWinners = new BoardSide[3]; // three round array
        GlobalVariables.playerSkip = false;
        GlobalVariables.turnCount = 0;
    }
}
