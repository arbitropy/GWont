using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Object = System.Object;
using Random = UnityEngine.Random;

public class CardDistribution : MonoBehaviour
{
    private List<GameObject> numberCards = new List<GameObject>();
    private List<GameObject> specialCards = new List<GameObject>();
    private int totalNumberCards, totalSpecialCards, startingCardsAmount = 15, newRoundCardsAmount = 3;
    private BoardManagement boardManagement;

    
    private void Start()
    {
        LoadAllCards();
        boardManagement = gameObject.GetComponent<BoardManagement>();
        DistributeCardsInit();
    }

    /// <summary>
    /// Distributes the initial cards to the players in round 1.
    /// </summary>
    void DistributeCardsInit()
    {
        List<GameObject> cardsTop = new List<GameObject>();
        List<GameObject> cardsBottom = new List<GameObject>();
        for (int i = 0; i < startingCardsAmount; i++)
        {
            cardsTop.Add(GenerateCard(BoardSide.Top));
            cardsBottom.Add(GenerateCard(BoardSide.Bottom));
        }
        boardManagement.AddCardInHand(BoardSide.Top, cardsTop);
        //print(temp.transform.position + " " + temp.name);
        boardManagement.AddCardInHand(BoardSide.Bottom, cardsBottom);
        //print(temp2.transform.position + " " + temp2.name);
    }

    /// <summary>
    /// Distributes the new round cards to the players.
    /// </summary>
    public void DistributeCardsNewRound()
    {
        List<GameObject> cardsTop = new List<GameObject>();
        List<GameObject> cardsBottom = new List<GameObject>();
        for (int i = 0; i < newRoundCardsAmount; i++)
        {
            cardsTop.Add(GenerateCard(BoardSide.Top));
            cardsBottom.Add(GenerateCard(BoardSide.Bottom));
        }
        boardManagement.AddCardInHand(BoardSide.Top, cardsTop);
        boardManagement.AddCardInHand(BoardSide.Bottom, cardsBottom);
    }

    KeyValuePair<bool, int> GetRandomIndex()
    {
        int cardType = Random.Range(0, 3);
        KeyValuePair<bool, int> result;
        if (cardType == 0)
        {
            result = new KeyValuePair<bool, int>(true, Random.Range(0, totalSpecialCards));

        }
        else
        {
            result = new KeyValuePair<bool, int>(false, Random.Range(0, totalNumberCards));
        }

        return result;
    }

    /// <summary>
    /// Generates a card based on the specified board side.
    /// </summary>
    /// <param name="boardSide">The board side to generate the card for.</param>
    /// <returns>The generated card as a GameObject.</returns>
    GameObject GenerateCard(BoardSide boardSide)
    {
        GameObject temp;
        KeyValuePair<bool, int> tempRandomResult;
        tempRandomResult = GetRandomIndex();
        if (boardSide == BoardSide.Top)
        {
            if (tempRandomResult.Key)
            {
                temp = Instantiate(specialCards[tempRandomResult.Value], GlobalVariables.instantiationPosition, Quaternion.identity);
                temp.GetComponent<Card>().boardSide = BoardSide.Top;
            }
            else
            {
                temp = Instantiate(numberCards[tempRandomResult.Value], GlobalVariables.instantiationPosition, Quaternion.identity);
                temp.GetComponent<Card>().boardSide = BoardSide.Top;
            }
        }
        else
        {
            if (tempRandomResult.Key)
            {
                temp = Instantiate(specialCards[tempRandomResult.Value], -GlobalVariables.instantiationPosition, Quaternion.identity);
                temp.GetComponent<Card>().boardSide = BoardSide.Bottom;
            }
            else
            {
                temp = Instantiate(numberCards[tempRandomResult.Value], -GlobalVariables.instantiationPosition, Quaternion.identity);
                temp.GetComponent<Card>().boardSide = BoardSide.Bottom;
            }
        }

        return temp;
    }

    void LoadAllCards()
    {
        Object[] numberCardPrefabs = Resources.LoadAll("Prefabs/NumberPrefabs");
        Object[] specialCardPrefabs = Resources.LoadAll("Prefabs/SpecialPrefabs");
        foreach (var VARIABLE in numberCardPrefabs)
        {
            numberCards.Add((GameObject)VARIABLE);
        }
        foreach (var VARIABLE in specialCardPrefabs)
        {
            specialCards.Add((GameObject)VARIABLE);
        }

        totalNumberCards = numberCards.Count;
        totalSpecialCards = specialCards.Count;
    }

    public void GenerateCardsForSide(BoardSide boardSide, int numberOfCards)
    {
        List<GameObject> temp = new List<GameObject>();
        for (int i = 0; i < numberOfCards; i++)
        {
            temp.Add(GenerateCard(boardSide));
        }
        boardManagement.AddCardInHand(boardSide, temp);
    }

    public void GiveCardsForSide(BoardSide boardSide)
    {

    }
}