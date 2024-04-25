using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

public class BoardManagement : MonoBehaviour
{
    public static Dictionary<BoardCell, List<GameObject>> boardState;
    public static BoardManagement instance;
    private PointsSystem pointsSystem;
    private CardPosition cardPosition;
    private CardDistribution cardDistribution;
    public static Dictionary<BoardSide, List<GameObject>> cardsInHand;
    public static List<GameObject> discardPile;
    private Opponent opponent;
    private Player player;
    private Vector2 cardRemovalPosition = new Vector2(0f, 0f);

    private Color weatherColor = new Color(.5f, .5f, .5f);

    void Awake()
    {
        boardState = new Dictionary<BoardCell, List<GameObject>>();
        cardsInHand = new Dictionary<BoardSide, List<GameObject>>();
        discardPile = new List<GameObject>();

        instance = this;
        for (int i = 0; i < 6; i++)
        {
            boardState.Add((BoardCell)i, new List<GameObject>());
        }

        for (int i = 0; i < 2; i++)
        {
            cardsInHand.Add((BoardSide)i, new List<GameObject>());
        }

        for (int i = 0; i < 3; i++)
        {
            GlobalVariables.weather.Add(false);
        }

        pointsSystem = GetComponent<PointsSystem>();
        cardPosition = GetComponent<CardPosition>();
        cardDistribution = GetComponent<CardDistribution>();
        opponent = GetComponent<Opponent>();
        player = GetComponent<Player>();
    }

    void Update()
    {
        CheckIfRoundOver();
    }

    void CheckIfRoundOver()
    {
        if (cardsInHand[BoardSide.Top].Count == 0 ||
            cardsInHand[BoardSide.Bottom].Count == 0 || (GlobalVariables.playerSkip && GlobalVariables.playerTurn)) // currently only player skip is implemented
        {
            GlobalVariables.playerSkip = false;
            NewRound();
        }
    }

    /// <summary>
    /// starts new round, not sure how it is called yet
    /// </summary>
    void NewRound()
    {
        GlobalVariables.round++;
        GlobalVariables.turnCount = 0;
        if (GlobalVariables.round == 4)
        {
            pointsSystem.EndGame();
            boardState = null;
            // Destroy(gameObject);
        }
        else
        {
            cardDistribution.DistributeCardsNewRound();
            ClearBoardForNewRound();
            pointsSystem.NewRoundUpdate();
        }

    }

    /// <summary>
    /// adds all cards to discardPile and resets, moves to removal position using lerp system, and then deactivates 
    /// </summary>
    void ClearBoardForNewRound()
    {
        List<GameObject> allOnBoardCards = new List<GameObject>(); // will be used for mass removal LERP of cards
        List<Vector2> finalCardPositions = new List<Vector2>();
        for (int i = 0; i < 6; i++)
        {
            foreach (var card in boardState[(BoardCell)i])
            {
                // card.SetActive(false); needed for animation
                var cardScript = card.GetComponent<Card>();
                if (!cardScript.discared)
                {
                    discardPile.Add(card); // avoid duplication in discardPile
                }
                cardScript.ResetRound();
                allOnBoardCards.Add(card);
            }
            boardState[(BoardCell)i].Clear();
        }
        // the position will be the same for all card, but need to duplicate for existing lerp system
        foreach (var card in allOnBoardCards)
        {
            finalCardPositions.Add(cardRemovalPosition);
        }
        print(allOnBoardCards.Count);
        CardPosition.MoveCards(allOnBoardCards, finalCardPositions, false);
    }

    /// <summary>
    /// Adds cards to hand existing, ending position is calculated here, starting position is wherever the card already is so technically should work anytime. 
    /// </summary>
    /// <param name="boardSide"></param>
    /// <param name="cards"></param>
    public void AddCardInHand(BoardSide boardSide, List<GameObject> cards)
    {
        foreach (var card in cards)
        {
            cardsInHand[boardSide].Add(card);
            if (boardSide == BoardSide.Top)
            {
                card.GetComponent<Card>().SetOpponent();
            }
        }
        //print(cardsInHand[boardSide].Count);
        List<Vector2> positions = BoardBounds.GetPositionInHand(cardsInHand[boardSide].Count, boardSide);
        CardPosition.MoveCards(cardsInHand[boardSide], positions, true);
    }

    /// <summary>
    /// Adds card to board, calculates the position of the card in the board, removes the card from hand
    /// </summary>
    /// <param name="boardCell"></param>
    /// <param name="card"></param>
    public static void AddCardToBoard(BoardCell boardCell, GameObject card)
    {
        
        boardState[boardCell].Add(card);

        Card cardScript = card.GetComponent<Card>();
        cardScript.onBoard = true;

        // this is to support proper simulating in AI
        GlobalVariables.turnCount++;
        cardScript.playedAtTurn = GlobalVariables.turnCount;

        cardsInHand[cardScript.boardSide].Remove(card);
        List<Vector2> tempHandCardPositionList = BoardBounds.GetPositionInHand(cardsInHand[cardScript.boardSide].Count, cardScript.boardSide);
        CardPosition.MoveCards(cardsInHand[cardScript.boardSide], tempHandCardPositionList, true);

        List<Vector2> tempPositionList = BoardBounds.GetPositionInBoard(boardCell, boardState[boardCell].Count);
        CardPosition.MoveCards(boardState[boardCell], tempPositionList, true);
        if (cardScript.special)
        {
            switch (cardScript.powerCardType)
            {
                case PowerCardType.Assassin:
                    instance.Assassin((cardScript.boardSide == BoardSide.Top) ? BoardSide.Bottom : BoardSide.Top);
                    break;
                case PowerCardType.Decoy:
                    instance.Decoy();
                    break;
                case PowerCardType.Medic:
                    instance.Medic(cardScript.boardSide);
                    break;
                case PowerCardType.Weather:
                    instance.Weather(cardScript.cardColor);
                    break;
                case PowerCardType.Morale:
                    instance.Morale(boardCell);
                    break;
                case PowerCardType.Spy:
                    instance.Spy();
                    break;
                case PowerCardType.AirSupport:
                    instance.AirSupport(boardCell);
                    break;
                case PowerCardType.ClearWeather:
                    instance.ClearWeather();
                    break;
            }
        }
        else
        {
            instance.AddPoints(boardCell, cardScript.cardPoints);
        }
    }

    /// <summary>
    /// finds maximum number card and destroys max card, from either opponent
    /// </summary>
    void Assassin(BoardSide boardSideOpponent)
    {
        int max = 0, maxIndex = -1;
        for (int i = 0; i < 6; i++)
        {
            foreach (var card in boardState[(BoardCell)i])
            {
                int points = card.GetComponent<Card>().cardPoints;
                if (points > max && !card.GetComponent<Card>().discared)
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
                    if (card.GetComponent<Card>().cardPoints == max && !card.GetComponent<Card>().discared)
                    {
                        RemoveCardFromBoard((BoardCell)i, card);
                    }
                }
            }
        }
    }

    /// <summary>
    /// Lets the player take two cards back from board, not implemented
    /// </summary>
    void Decoy()
    {

    }

    /// <summary>
    /// Doesn't work, better implement later
    /// </summary>
    void Medic(BoardSide boardSide)
    {
        if (discardPile.Count > 0)
        {
            int index = Random.Range(0, discardPile.Count);
            GameObject reviveCard = discardPile[index];
            discardPile.RemoveAt(index);
            reviveCard.GetComponent<Card>().boardSide = boardSide;
            reviveCard.transform.position = (boardSide == BoardSide.Top ? 1 : -1) * GlobalVariables.instantiationPosition;
            reviveCard.SetActive(true);
            cardsInHand[boardSide].Add(reviveCard);
            List<Vector2> tempHandCardPositionList = BoardBounds.GetPositionInHand(cardsInHand[boardSide].Count, boardSide);
            CardPosition.MoveCards(cardsInHand[boardSide], tempHandCardPositionList, true);
        }
    }

    /// <summary>
    /// Takes card color, traverses through the two same colored cell, dims card sprite renderer, sets new points 
    /// doesn't work with new cards added after adding weather card to the board
    /// </summary>
    void Weather(CardColor cardColor)
    {
        GlobalVariables.weather[(int)cardColor] = true;
        int topPoints = 0, bottomPoints = 0;
        if (cardColor == CardColor.Red)
        {
            foreach (var card in boardState[BoardCell.TopRed])
            {
                if (!card.GetComponent<Card>().special && !card.GetComponent<Card>().discared) // special cards don't have any points, so 1 point for each number card, discarded cards already don't have any effect
                {
                    topPoints++;
                }
                card.GetComponent<SpriteRenderer>().color = weatherColor;
            }
            pointsSystem.CellPointsSet(BoardCell.TopRed, topPoints);
            foreach (var card in boardState[BoardCell.BottomRed])
            {
                if (!card.GetComponent<Card>().special)
                {
                    bottomPoints++;
                }
                card.GetComponent<SpriteRenderer>().color = weatherColor;
            }
            pointsSystem.CellPointsSet(BoardCell.BottomRed, bottomPoints);
        }
        else if (cardColor == CardColor.Blue)
        {
            foreach (var card in boardState[BoardCell.TopBlue])
            {
                if (card.GetComponent<Card>().special)
                {
                    topPoints++;
                }
                card.GetComponent<SpriteRenderer>().color = weatherColor;
            }
            pointsSystem.CellPointsSet(BoardCell.TopBlue, topPoints);
            foreach (var card in boardState[BoardCell.BottomBlue])
            {
                if (!card.GetComponent<Card>().special)
                {
                    bottomPoints++;
                }
                card.GetComponent<SpriteRenderer>().color = weatherColor;
            }
            pointsSystem.CellPointsSet(BoardCell.BottomBlue, bottomPoints);
        }
        else
        {
            foreach (var card in boardState[BoardCell.TopGreen])
            {
                topPoints++;
                card.GetComponent<SpriteRenderer>().color = weatherColor;
            }
            pointsSystem.CellPointsSet(BoardCell.TopGreen, topPoints);
            foreach (var card in boardState[BoardCell.BottomGreen])
            {
                bottomPoints++;
                card.GetComponent<SpriteRenderer>().color = weatherColor;
            }
            pointsSystem.CellPointsSet(BoardCell.BottomGreen, bottomPoints);
        }
    }

    /// <summary>
    /// adds one point of each number card in the cell
    /// </summary>
    /// <param name="boardCell"></param>
    void Morale(BoardCell boardCell)
    {
        int pointsToAdd = 0;
        foreach (var card in boardState[boardCell])
        {
            if (!card.GetComponent<Card>().special)
            {
                pointsToAdd++;
            }
        }
        pointsSystem.CellPointsUpdate(boardCell, pointsToAdd);
    }

    void MoraleRemove(BoardCell boardCell)
    {
        int pointsToAdd = 0;
        foreach (var card in boardState[boardCell])
        {
            if (!card.GetComponent<Card>().special)
            {
                pointsToAdd++;
            }
        }
        pointsSystem.CellPointsUpdate(boardCell, pointsToAdd);
    }

    void Spy()
    {

    }

    /// <summary>
    /// Doubles the cell points
    /// </summary>
    /// <param name="boardCell"></param>
    void AirSupport(BoardCell boardCell)
    {
        pointsSystem.CellPointDouble(boardCell, true);
    }

    void AirSupportRemove(BoardCell boardCell)
    {
        pointsSystem.CellPointDouble(boardCell, true);
    }

    /// <summary>
    /// Reverse of weather() but for all colors.
    /// </summary>
    void ClearWeather()
    {
        CardColor running = CardColor.Blue;
        bool found = false;
        for (int i = 0; i < 3; i++)
        {
            if (GlobalVariables.weather[i])
            {
                running = (CardColor)i;
                found = true;
            }
        }
        if (!found) return;

        int topPoints = 0, bottomPoints = 0;
        if (running == CardColor.Red)
        {
            foreach (var card in boardState[BoardCell.TopRed])
            {
                if (!card.GetComponent<Card>().special)
                {
                    topPoints += card.GetComponent<Card>().cardPoints;
                }
                card.GetComponent<SpriteRenderer>().color = Color.white;
            }
            pointsSystem.CellPointsSet(BoardCell.TopRed, topPoints);
            foreach (var card in boardState[BoardCell.BottomRed])
            {
                if (!card.GetComponent<Card>().special)
                {
                    bottomPoints += card.GetComponent<Card>().cardPoints;
                }
                card.GetComponent<SpriteRenderer>().color = Color.white;
            }
            pointsSystem.CellPointsSet(BoardCell.BottomRed, bottomPoints);
        }
        else if (running == CardColor.Blue)
        {
            foreach (var card in boardState[BoardCell.TopBlue])
            {
                if (card.GetComponent<Card>().special)
                {
                    topPoints += card.GetComponent<Card>().cardPoints;
                }
                card.GetComponent<SpriteRenderer>().color = Color.white;
            }
            pointsSystem.CellPointsSet(BoardCell.TopBlue, topPoints);
            foreach (var card in boardState[BoardCell.BottomBlue])
            {
                if (!card.GetComponent<Card>().special)
                {
                    bottomPoints += card.GetComponent<Card>().cardPoints;
                }
                card.GetComponent<SpriteRenderer>().color = Color.white;
            }
            pointsSystem.CellPointsSet(BoardCell.BottomBlue, bottomPoints);
        }
        else
        {
            foreach (var card in boardState[BoardCell.TopGreen])
            {
                topPoints++;
                card.GetComponent<SpriteRenderer>().color = Color.white;
            }
            pointsSystem.CellPointsSet(BoardCell.TopGreen, topPoints);
            foreach (var card in boardState[BoardCell.BottomGreen])
            {
                bottomPoints++;
                card.GetComponent<SpriteRenderer>().color = Color.white;
            }
            pointsSystem.CellPointsSet(BoardCell.BottomGreen, bottomPoints);
        }
    }

    void AddPoints(BoardCell boardCell, int points)
    {
        pointsSystem.CellPointsUpdate(boardCell, points);
    }
    void RemovePoints(BoardCell boardCell, int points)
    {
        pointsSystem.CellPointsUpdate(boardCell, -points);
    }

    public static void RemoveCardFromBoard(BoardCell boardCell, GameObject card)
    {
        Card cardScript = card.GetComponent<Card>();
        if (cardScript.special)
        {
            switch (cardScript.powerCardType)
            {
                case PowerCardType.Medic:
                    instance.Medic(BoardSide.Top);
                    break;
                case PowerCardType.Morale:
                    instance.Morale(boardCell);
                    break;
                case PowerCardType.Spy:
                    instance.Spy();
                    break;
                case PowerCardType.AirSupport:
                    instance.AirSupport(boardCell);
                    break;
                case PowerCardType.ClearWeather:
                    instance.ClearWeather();
                    break;
            }
        }
        else
        {
            instance.RemovePoints(boardCell, cardScript.cardPoints);
        }
        // boardState[boardCell].Remove(card); // doesn't remove the card from boardstate for not adding removal
        discardPile.Add(card);
        card.GetComponent<SpriteRenderer>().color = Color.black; // sets the color to black for discardPile
        card.GetComponent<Card>().discared = true;
        // card.SetActive(false);
    }
}