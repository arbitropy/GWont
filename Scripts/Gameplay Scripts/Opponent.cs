using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using Object = System.Object;
using Random = UnityEngine.Random;

public class Opponent : MonoBehaviour
{
    private PointsSystem pointsSystem;
    private bool depthLimitReached;
    private Dictionary<BoardCell, List<GameObject>> simulatedBoardState;
    private Dictionary<BoardSide, int> simulatedTotalPoints;
    private List<GameObject> simulatedHand;
    private List<GameObject> tempHand;
    private int targetDepth = 3;
    private List<GameObject> allPossibleCards;
    private SimulatePoints simulatePoints;

    private void Awake()
    {
        LoadAllCards();
        pointsSystem = GetComponent<PointsSystem>();
        simulatePoints = GetComponent<SimulatePoints>();
    }


    void LoadAllCards()
    {
        allPossibleCards = new List<GameObject>();
        Object[] numberCardPrefabs = Resources.LoadAll("Prefabs/NumberPrefabs");
        Object[] specialCardPrefabs = Resources.LoadAll("Prefabs/SpecialPrefabs");
        foreach (Object VARIABLE in numberCardPrefabs)
        {
            GameObject temp = Instantiate((GameObject)VARIABLE);
            allPossibleCards.Add(temp);
            temp.SetActive(false);
        }

        foreach (var VARIABLE in specialCardPrefabs)
        {
            GameObject temp = Instantiate((GameObject)VARIABLE);
            allPossibleCards.Add(temp);
            temp.SetActive(false);
        }
    }

    /// <summary>
    /// Currently the AIMove is called when it is not player's turn and the game is not in moving state, but the processing still happens so there is a delay while minimax is running, maybe find a way so that when the AIMove processing and minimax is running simultaneously, understand coroutines.
    /// </summary>
    private void Update()
    {
        if (!GlobalVariables.playerTurn && !GlobalVariables.moving)
        {
            AIMove();
            GlobalVariables.playerTurn = true;
        }
    }

    /// <summary>
    /// Executes the AI move by choosing the best card to play and placing it on the board.
    /// </summary>
    void AIMove()
    {
        simulatedBoardState = new Dictionary<BoardCell, List<GameObject>>(BoardManagement.boardState);
        simulatedTotalPoints = new Dictionary<BoardSide, int>(pointsSystem.totalPoints);
        simulatedHand = new List<GameObject>(BoardManagement.cardsInHand[BoardSide.Top]);
        tempHand = new List<GameObject>();
        foreach (var VARIABLE in simulatedHand)
        {
            var temp = Instantiate(VARIABLE);
            tempHand.Add(temp);
            temp.SetActive(false);
        }

        //print("AIMove");
        int cardToPlayIndex = ChooseBestCard(tempHand);
        //print(cardToPlayIndex);
        GameObject card = BoardManagement.cardsInHand[BoardSide.Top][cardToPlayIndex];
        Card cardScript = card.GetComponent<Card>();
        cardScript.boardSide = BoardSide.Top;

        // this is to support proper simulating in AI
        GlobalVariables.turnCount++;
        cardScript.playedAtTurn = GlobalVariables.turnCount;

        cardScript.ToBoard();
    }

    /// <summary>
    /// eval if no card left in the minimax simulation
    /// </summary>
    /// <returns></returns>
    bool IsGameOver()
    {
        // print("no more cards in ai hand");
        if (tempHand.Count == 0)
        {
            return true;
        }

        return false;
    }

    /// <summary>
    /// Performs the MiniMax algorithm to determine the best score for the current game state.
    /// </summary>
    /// <param name="tempHand">The list of GameObjects representing the temporary hand of the player.</param>
    /// <param name="simulatedBoardState">The dictionary representing the simulated board state.</param>
    /// <param name="simulatedTotalPoints">The dictionary representing the simulated total points for each player.</param>
    /// <param name="isMaximizingPlayer">A boolean indicating whether the current player is the maximizing player.</param>
    /// <param name="currentDepth">The current depth of the MiniMax algorithm.</param>
    /// <returns>The best score for the current game state.</returns>
    int MiniMax(List<GameObject> tempHand, Dictionary<BoardCell, List<GameObject>> simulatedBoardState,
        Dictionary<BoardSide, int> simulatedTotalPoints, bool isMaximizingPlayer, int currentDepth)
    {
        if (currentDepth == targetDepth) depthLimitReached = true;

        if (IsGameOver() || depthLimitReached)
        {
            // print("eval" + IsGameOver() + currentDepth);
            int result = Evaluate(simulatedTotalPoints);
            if (depthLimitReached)
            {
                depthLimitReached = false;
            }
            return result;
        }

        if (isMaximizingPlayer)
        {
            int bestScore = int.MinValue;


            for (int i = 0; i < tempHand.Count; i++)
            {
                foreach (var VARIABLE in tempHand)
                {
                    //  print(VARIABLE.name);
                }
                // print("end" + i + " up" + currentDepth);
                GameObject temp = tempHand[i];
                simulatePoints.ApplyMove(tempHand[i], tempHand, simulatedBoardState, simulatedTotalPoints,
                    isMaximizingPlayer);

                int score = MiniMax(tempHand, simulatedBoardState, simulatedTotalPoints, !isMaximizingPlayer,
                    currentDepth + 1);


                simulatePoints.UndoMove(temp, tempHand, simulatedBoardState, simulatedTotalPoints, isMaximizingPlayer);
                foreach (var VARIABLE in tempHand)
                {
                    // print(VARIABLE.name);
                }
                // print("end" + i + " down" + currentDepth);

                bestScore = Mathf.Max(bestScore, score);
            }

            return bestScore;
        }
        else
        {
            int bestScore = int.MaxValue;

            foreach (GameObject card in BoardManagement.cardsInHand[BoardSide.Bottom])
            {
                simulatePoints.ApplyMove(card, tempHand, simulatedBoardState, simulatedTotalPoints, isMaximizingPlayer);

                int score = MiniMax(tempHand, simulatedBoardState, simulatedTotalPoints, !isMaximizingPlayer,
                    currentDepth + 1);

                simulatePoints.UndoMove(card, tempHand, simulatedBoardState, simulatedTotalPoints, isMaximizingPlayer);
                bestScore = Mathf.Min(bestScore, score);
            }

            return bestScore;
        }
    }

    /// <summary>
    /// Chooses the best card from the given hand based on simulating all possible moves and evaluating there outcome. Currently for simulating player move from AI side, we assume player has all the cards in the hand, so the AI move is as good as possible. 
    /// </summary>
    /// <param name="tempHand">The list of GameObjects representing the temporary hand.</param>
    /// <returns>The index of the best card in the hand.</returns>
    int ChooseBestCard(List<GameObject> tempHand)
    {
        int bestScore = int.MinValue;
        GameObject bestCard = null;

        for (int i = 0; i < tempHand.Count; i++)
        {
            GameObject temp = tempHand[i];
            simulatePoints.ApplyMove(tempHand[i], tempHand, simulatedBoardState, simulatedTotalPoints, true);

            int score = MiniMax(tempHand, simulatedBoardState,
                simulatedTotalPoints,
                false, 0);
            //print(i + " " + score);
            simulatePoints.UndoMove(temp, tempHand, simulatedBoardState, simulatedTotalPoints, true);
            if (score > bestScore)
            {
                bestScore = score;
                bestCard = tempHand[i];
            }
        }

        return tempHand.IndexOf(bestCard);
    }

    int Evaluate(Dictionary<BoardSide, int> simulatedTotalPoints)
    {
        //print(simulatedTotalPoints[BoardSide.Top]);
        return simulatedTotalPoints[BoardSide.Top] - simulatedTotalPoints[BoardSide.Bottom];
    }
}