using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimulatePoints : MonoBehaviour
{
    /// <summary>
    /// Applies a move by placing a card on the board and updating the board state and total points.
    /// </summary>
    /// <param name="card">The card to be placed on the board.</param>
    /// <param name="tempHand">The temporary hand of cards.</param>
    /// <param name="boardState">The current state of the board.</param>
    /// <param name="totalPoints">The total points for each board side.</param>
    /// <param name="isMaximizingPlayer">A flag indicating if the move is made by the maximizing player.</param>
    public void ApplyMove(GameObject card, List<GameObject> tempHand,
        Dictionary<BoardCell, List<GameObject>> boardState, Dictionary<BoardSide, int> totalPoints,
        bool isMaximizingPlayer)
    {
        if (isMaximizingPlayer)
        {
            tempHand.Remove(card);
            switch (card.GetComponent<Card>().cardColor)
            {
                case CardColor.Red:
                    boardState[BoardCell.TopRed].Add(card);
                    break;
                case CardColor.Blue:
                    boardState[BoardCell.TopBlue].Add(card);
                    break;
                case CardColor.Green:
                    boardState[BoardCell.TopGreen].Add(card);
                    break;
            }
        }
        else
        {
            switch (card.GetComponent<Card>().cardColor)
            {
                case CardColor.Red:
                    boardState[BoardCell.BottomRed].Add(card);
                    break;
                case CardColor.Blue:
                    boardState[BoardCell.BottomBlue].Add(card);
                    break;
                case CardColor.Green:
                    boardState[BoardCell.BottomGreen].Add(card);
                    break;
            }
        }

        PointsUpdate(boardState, totalPoints);
    }

    /// <summary>
    /// Updates the total points for each board side based on the current board state. doesn't take special cards into account
    /// </summary>
    /// <param name="boardState">The current state of the board.</param>
    /// <param name="totalPoints">The total points for each board side.</param>
    void PointsUpdate(Dictionary<BoardCell, List<GameObject>> boardState, Dictionary<BoardSide, int> totalPoints)
    {
        ApplySpecialCard(boardState);
        totalPoints[BoardSide.Top] = 0;
        totalPoints[BoardSide.Bottom] = 0;
        for (int i = 0; i < 3; i++)
        {
            foreach (var card in boardState[(BoardCell)i])
            {
                Card cardScript = card.GetComponent<Card>();
                totalPoints[BoardSide.Top] += cardScript.inGameCardPoints;

            }
        }

        for (int i = 3; i < 6; i++)
        {
            foreach (var card in boardState[(BoardCell)i])
            {
                Card cardScript = card.GetComponent<Card>();
                totalPoints[BoardSide.Bottom] += cardScript.inGameCardPoints;
            }
        }
    }

    /// <summary>
    /// We apply special cards live when being played, but when simulating, we directly apply them to the card points, and then calculate total point, maybe this is a better approach for the game too, inspect later 
    /// </summary>
    /// <param name="boardState"></param>
    void ApplySpecialCard(Dictionary<BoardCell, List<GameObject>> boardState)
    {
        for (int i = 0; i < 6; i++)
        {
            foreach (var card in boardState[(BoardCell)i])
            {
                Card cardScript = card.GetComponent<Card>();
                if (cardScript.special)
                {
                    switch (cardScript.powerCardType)
                    {
                        case PowerCardType.Assassin:
                            Assassin(boardState, cardScript.playedAtTurn);
                            break;
                        // case PowerCardType.Decoy:
                        //     instance.Decoy();
                        //     break;
                        // case PowerCardType.Medic:
                        //     instance.Medic(cardScript.boardSide);
                        //     break;
                        case PowerCardType.Weather:
                            Weather(boardState, cardScript.playedAtTurn, cardScript.cardColor);
                            break;
                        case PowerCardType.Morale:
                            Morale(boardState, cardScript.playedAtTurn, (BoardCell)i);
                            break;
                        // case PowerCardType.Spy:
                        //     instance.Spy();
                        //     break;
                        case PowerCardType.AirSupport:
                            AirSupport(boardState, cardScript.playedAtTurn, (BoardCell)i);
                            break;
                        case PowerCardType.ClearWeather:
                            ClearWeather(boardState, cardScript.playedAtTurn, cardScript.cardColor);
                            break;
                    }
                }

            }
        }
    }
    void Assassin(Dictionary<BoardCell, List<GameObject>> boardState, int playTurn)
    {
        int max = 0, maxIndex = -1;
        for (int i = 0; i < 6; i++)
        {
            foreach (var card in boardState[(BoardCell)i])
            {
                Card cardScript = card.GetComponent<Card>();
                int points = cardScript.inGameCardPoints;
                // only apply assassin if played after the card
                if (points > max && !cardScript.discared && playTurn > cardScript.playedAtTurn)
                {
                    max = points;
                    maxIndex = i;
                }
                {
                    max = points;
                    maxIndex = i;
                }
            }
        }
        if (maxIndex != -1)
        {
            for (int i = 0; i < 6; i++)
            {
                foreach (var card in boardState[(BoardCell)i])
                {
                    Card cardScript = card.GetComponent<Card>();
                    if (cardScript.inGameCardPoints == max && !cardScript.discared && playTurn > cardScript.playedAtTurn)
                    {
                        card.GetComponent<Card>().inGameCardPoints = 0;
                        card.GetComponent<Card>().discared = true;
                    }
                }
            }
        }
    }

    void Weather(Dictionary<BoardCell, List<GameObject>> boardState, int playTurn, CardColor cardColor)
    {
        if (cardColor == CardColor.Red)
        {
            foreach (var card in boardState[BoardCell.TopRed])
            {
                Card cardScript = card.GetComponent<Card>();
                if (!cardScript.special && !cardScript.discared && playTurn > cardScript.playedAtTurn)
                {
                    card.GetComponent<Card>().inGameCardPoints = 1;
                }
            }
            foreach (var card in boardState[BoardCell.BottomRed])
            {
                Card cardScript = card.GetComponent<Card>();
                if (!cardScript.special && !cardScript.discared && playTurn > cardScript.playedAtTurn)
                {
                    card.GetComponent<Card>().inGameCardPoints = 1;
                }
            }
        }
        else if (cardColor == CardColor.Blue)
            {
                foreach (var card in boardState[BoardCell.TopBlue])
                {
                    Card cardScript = card.GetComponent<Card>();
                    if (!cardScript.special && !cardScript.discared && playTurn > cardScript.playedAtTurn)
                    {
                        card.GetComponent<Card>().inGameCardPoints = 1;
                    }
                }
                foreach (var card in boardState[BoardCell.BottomBlue])
                {
                    Card cardScript = card.GetComponent<Card>();
                    if (!cardScript.special && !cardScript.discared && playTurn > cardScript.playedAtTurn)
                    {
                        card.GetComponent<Card>().inGameCardPoints = 1;
                    }
                }
            }
            else
            {
                foreach (var card in boardState[BoardCell.TopGreen])
                {
                    Card cardScript = card.GetComponent<Card>();
                    if (!cardScript.special && !cardScript.discared && playTurn > cardScript.playedAtTurn)
                    {
                        card.GetComponent<Card>().inGameCardPoints = 1;
                    }
                }
                foreach (var card in boardState[BoardCell.BottomGreen])
                {
                    Card cardScript = card.GetComponent<Card>();
                    if (!cardScript.special && !cardScript.discared && playTurn > cardScript.playedAtTurn)
                    {
                        card.GetComponent<Card>().inGameCardPoints = 1;
                    }
                }
            }
        }

        void Morale(Dictionary<BoardCell, List<GameObject>> boardState, int playTurn, BoardCell boardCell)
        {
            foreach (var card in boardState[boardCell])
                {
                    Card cardScript = card.GetComponent<Card>();
                    if (!cardScript.special && !cardScript.discared && playTurn > cardScript.playedAtTurn)
                    {
                        card.GetComponent<Card>().inGameCardPoints += 1;
                    }
                }
        }

        void AirSupport(Dictionary<BoardCell, List<GameObject>> boardState, int playTurn, BoardCell boardCell)
        {
            foreach (var card in boardState[boardCell])
                {
                    Card cardScript = card.GetComponent<Card>();
                    if (!cardScript.special && !cardScript.discared && playTurn > cardScript.playedAtTurn)
                    {
                        card.GetComponent<Card>().inGameCardPoints *= 2;
                    }
                }
        }

        void ClearWeather(Dictionary<BoardCell, List<GameObject>> boardState, int playTurn, CardColor cardColor)
        {
            if (cardColor == CardColor.Red)
        {
            foreach (var card in boardState[BoardCell.TopRed])
            {
                Card cardScript = card.GetComponent<Card>();
                if (!cardScript.special && !cardScript.discared && playTurn > cardScript.playedAtTurn)
                {
                    cardScript.inGameCardPoints = cardScript.cardPoints;
                }
            }
            foreach (var card in boardState[BoardCell.BottomRed])
            {
                Card cardScript = card.GetComponent<Card>();
                if (!cardScript.special && !cardScript.discared && playTurn > cardScript.playedAtTurn)
                {
                    cardScript.inGameCardPoints = cardScript.cardPoints;
                }
            }
        }
        else if (cardColor == CardColor.Blue)
            {
                foreach (var card in boardState[BoardCell.TopBlue])
                {
                    Card cardScript = card.GetComponent<Card>();
                    if (!cardScript.special && !cardScript.discared && playTurn > cardScript.playedAtTurn)
                    {
                        cardScript.inGameCardPoints = cardScript.cardPoints;
                    }
                }
                foreach (var card in boardState[BoardCell.BottomBlue])
                {
                    Card cardScript = card.GetComponent<Card>();
                    if (!cardScript.special && !cardScript.discared && playTurn > cardScript.playedAtTurn)
                    {
                        cardScript.inGameCardPoints = cardScript.cardPoints;
                    }
                }
            }
            else
            {
                foreach (var card in boardState[BoardCell.TopGreen])
                {
                    Card cardScript = card.GetComponent<Card>();
                    if (!cardScript.special && !cardScript.discared && playTurn > cardScript.playedAtTurn)
                    {
                        cardScript.inGameCardPoints = cardScript.cardPoints;
                    }
                }
                foreach (var card in boardState[BoardCell.BottomGreen])
                {
                    Card cardScript = card.GetComponent<Card>();
                    if (!cardScript.special && !cardScript.discared && playTurn > cardScript.playedAtTurn)
                    {
                        cardScript.inGameCardPoints = cardScript.cardPoints;
                    }
                }
            }
        }

        /// <summary>
        /// Undoes a move by removing a card from the board and updating the board state and total points.
        /// </summary>
        /// <param name="card">The card to be removed from the board.</param>
        /// <param name="tempHand">The temporary hand of cards.</param>
        /// <param name="boardState">The current state of the board.</param>
        /// <param name="totalPoints">The total points for each board side.</param>
        /// <param name="isMaximizingPlayer">A flag indicating if the move is made by the maximizing player.</param>
        public void UndoMove(GameObject card, List<GameObject> tempHand, Dictionary<BoardCell, List<GameObject>> boardState,
            Dictionary<BoardSide, int> totalPoints, bool isMaximizingPlayer)
        {
            if (isMaximizingPlayer)
            {
                tempHand.Insert(0, card);
                switch (card.GetComponent<Card>().cardColor)
                {
                    case CardColor.Red:
                        boardState[BoardCell.TopRed].Remove(card);
                        break;
                    case CardColor.Blue:
                        boardState[BoardCell.TopBlue].Remove(card);
                        break;
                    case CardColor.Green:
                        boardState[BoardCell.TopGreen].Remove(card);
                        break;
                }
            }
            else
            {
                switch (card.GetComponent<Card>().cardColor)
                {
                    case CardColor.Red:
                        boardState[BoardCell.BottomRed].Remove(card);
                        break;
                    case CardColor.Blue:
                        boardState[BoardCell.BottomBlue].Remove(card);
                        break;
                    case CardColor.Green:
                        boardState[BoardCell.BottomGreen].Remove(card);
                        break;
                }
            }

            PointsUpdate(boardState, totalPoints);
        }

    }