using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GlobalVariables
{
    public static bool showingInfo;
    public static Vector3 instantiationPosition = new Vector3(0, 5.5f);
    public static bool moving;
    public static List<bool> weather = new List<bool>(3);
    public static bool playerTurn = true;
    public static int round = 1;
    public static BoardSide[] roundWinners = new BoardSide[3]; // three round array
    public static bool playerSkip = false;
    public static int turnCount = 0;
}
