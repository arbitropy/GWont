using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    public TextMeshProUGUI cardInfoTextObjectComponent;
    public Transform cardInfoTextObject;
    private RectTransform rectTransform;
    private Vector3 defaultPosition;
    private Vector3 numberCardPosition;
    public TextMeshProUGUI topScoreTextObjectComponent;
    public TextMeshProUGUI bottomScoreTextObjectComponent;
    public Transform gameOverPanel;
    public TextMeshProUGUI gameOverTextObject;
    public TextMeshProUGUI roundTextObject;

    private void Start()
    {
        rectTransform = cardInfoTextObject.GetComponent<RectTransform>();
        defaultPosition = rectTransform.anchoredPosition;
        //print(defaultPosition);
        // numberCardPosition = defaultPosition + new Vector3(0, -150, 0);
        numberCardPosition = defaultPosition;
        //print(numberCardPosition);
    }

    public void MainMenuAfterGameOverButton(){
        SceneManager.LoadScene("Main Menu", LoadSceneMode.Single);
    }

    public void GameOver(BoardSide winner)
    {
        gameOverPanel.gameObject.SetActive(true);
        switch (winner)
        {
            case BoardSide.Top:
                gameOverTextObject.text = "Game Over, Top wins";
                break;
            case BoardSide.Bottom:
                gameOverTextObject.text = "Game Over, Bottom wins";
                break;
            case BoardSide.None:
                gameOverTextObject.text = "Game Over, it's a draw";
                break;
        }


    }

    public void ShowInfoText(PowerCardType powerCardType, CardColor cardColor)
    {
        rectTransform.anchoredPosition = defaultPosition;
        string cardInfoText = "";
        switch (powerCardType)
        {
            case PowerCardType.Assassin:
                cardInfoText =
                    "Remove the card with the highest points from the board, even if it's one of your own cards.";
                break;
            case PowerCardType.Decoy:
                cardInfoText =
                    "Handpick two cards from the battlefield, reclaiming them to your hand for future strategic maneuvers in the remaining rounds.";
                break;
            case PowerCardType.Medic:
                cardInfoText =
                    "Resurrect a random deceased card from the discard pile, granting it a chance to once again shape the tides of battle.";
                break;
            case PowerCardType.Weather:
                cardInfoText =
                    "Lower the points of all cards in the same colored cell to 1, including your own cells.";
                break;
            case PowerCardType.Morale:
                cardInfoText =
                    "Increase the points of all cards in your same colored cell by +1.";
                break;
            case PowerCardType.Spy:
                cardInfoText =
                    "Inflict a penalty of 3 points upon your opponent, while seizing the opportunity to refresh your hand by redrawing 2 random cards, ensuring a renewed arsenal for the forthcoming clashes.";
                break;
            case PowerCardType.AirSupport:
                cardInfoText =
                    "Double the points of all cards in your same colored cell.";
                break;
            case PowerCardType.ClearWeather:
                cardInfoText =
                    "Remove the weather effect from the same colored cell. Affects both sides.";
                break;
            case PowerCardType.General:
                rectTransform.anchoredPosition = numberCardPosition;
                cardInfoText = "A general power card.";
                break;
        }

        cardInfoTextObjectComponent.text = cardInfoText;
        if (cardColor == CardColor.Red)
        {
            cardInfoTextObjectComponent.faceColor = new Color(255f / 255f, 0f, 0f);
        }
        else if (cardColor == CardColor.Green)
        {
            cardInfoTextObjectComponent.faceColor = new Color(12f / 255f, 172f / 255f, 80f / 255f);
        }
        else if (cardColor == CardColor.Blue)
        {
            cardInfoTextObjectComponent.faceColor = new Color(78f / 255f, 0f, 255f / 255f);
        }

        cardInfoTextObject.gameObject.SetActive(true);
    }

    public void StopShowingInfo()
    {
        cardInfoTextObject.gameObject.SetActive(false);
    }

    public void UpdateRoundInUI()
    {
        roundTextObject.text = "Round " + GlobalVariables.round.ToString();
    }

    public void UpdatePointsInUI(BoardSide boardSide, int points)
    {
        if (boardSide == BoardSide.Top)
        {
            topScoreTextObjectComponent.text = points.ToString();
        }
        else
        {
            bottomScoreTextObjectComponent.text = points.ToString();
        }
    }
}