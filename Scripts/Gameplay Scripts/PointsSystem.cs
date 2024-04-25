using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointsSystem : MonoBehaviour
{
    private Dictionary<BoardCell, int> points = new Dictionary<BoardCell, int>();
    public Dictionary<BoardSide, int> totalPoints = new Dictionary<BoardSide, int>();
    public UIController uiController;

    public int currentMovesCount = 0;

    public void CellPointsUpdate(BoardCell boardCell, int point)
    {
        points[boardCell] += point;
        TotalPointsUpdate();
    }

    public void CellPointsSet(BoardCell boardCell, int point)
    {
        points[boardCell] = point;
        TotalPointsUpdate();
    }

    public void CellPointDouble(BoardCell boardCell, bool multiply)
    {
        if (multiply)
        {
            points[boardCell] *= 2;
        }
        else
        {
            points[boardCell] /= 2;
        }
        TotalPointsUpdate();
    }


    /// <summary>
    /// Updates the total points of the board after any change to the cells
    /// </summary>
    void TotalPointsUpdate()
    {
        totalPoints[BoardSide.Top] = 0;
        totalPoints[BoardSide.Bottom] = 0;
        for (int i = 0; i < 6; i++)
        {
            if (i < 3)
            {
                totalPoints[BoardSide.Top] += points[(BoardCell)i];
                uiController.UpdatePointsInUI(BoardSide.Top, totalPoints[BoardSide.Top]);
            }
            else
            {
                totalPoints[BoardSide.Bottom] += points[(BoardCell)i];
                uiController.UpdatePointsInUI(BoardSide.Bottom, totalPoints[BoardSide.Bottom]);
            }
        }
    }

    void Start()
    {
        for (int i = 0; i < 6; i++)
        {
            points.Add((BoardCell)i, 0);
        }

        for (int i = 0; i < 2; i++)
        {
            totalPoints.Add((BoardSide)i, 0);
        }
        uiController = FindObjectOfType<UIController>();
    }

    public void NewRoundUpdate()
    {
        for (int i = 0; i < 6; i++)
        {
            points[(BoardCell)i] = 0;
        }
        if (totalPoints[BoardSide.Top] > totalPoints[BoardSide.Bottom])
        {
            GlobalVariables.roundWinners[GlobalVariables.round - 2] = BoardSide.Top; // -2 because round is already updated, and array starts from 0
        }
        else if (totalPoints[BoardSide.Top] < totalPoints[BoardSide.Bottom])
        {
            GlobalVariables.roundWinners[GlobalVariables.round - 2] = BoardSide.Bottom; // needs to be updated in case of draw
        }
        else
        {
            GlobalVariables.roundWinners[GlobalVariables.round - 2] = BoardSide.None;
        }
        TotalPointsUpdate();
        uiController.UpdateRoundInUI();
    }

    public void EndGame(){
        int top = 0;
        int bottom = 0;
        int draw = 0;
        foreach (var roundWinner in GlobalVariables.roundWinners) {
            switch(roundWinner){
                case BoardSide.Top:
                    top++;
                    break;
                case BoardSide.Bottom:
                    bottom++;
                    break;
                case BoardSide.None:
                    draw++;
                    break;
            }
        }
        if (top > bottom && top > draw){
            uiController.GameOver(BoardSide.Top);
        } else if (top < bottom && bottom > draw){
            uiController.GameOver(BoardSide.Bottom);
        } else if (draw > top && draw > bottom)
        {
            uiController.GameOver(BoardSide.None);
        }
    }

}
