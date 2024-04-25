using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using TMPro;
using UnityEngine;

public class RulesPanel : MonoBehaviour
{
    public GameObject rulesPanel;
    public GameObject mainMenuPanel;
    public TextMeshProUGUI titleText;
    public TextMeshProUGUI rulesText;
    public TextMeshProUGUI buttonText;
    private string setupText = @"<mark>Objective:</mark>
Win the most rounds out of three to claim victory in the match.
<mark>Setup:</mark>
- Each player starts with 15 cards.
- There are three colors of cells for each player: Red, Green, and Blue.
- There are three colors of cards: Red, Green, and Blue.
- The cards are played into the corresponding colored cells (e.g., red cards in red cells).";
    private string gameplayText = @"<mark>Card Types:</mark>
1. Point Cards: Add points to the corresponding colored cell.
2. Power Cards: Have special effects on specific cells or all cells (right-click a card to see its effect).
<mark>Gameplay:</mark>
1. Each round, play cards strategically in your cells to accumulate points and use power cards to boost your point cards or hinder your opponent.
2. At the start of a new round, each player draws 3 additional cards.
3. Players can skip a round to save cards for later rounds, allowing their opponent to play solo until they also skip.
4. Each round, the player with the most combined points across their three cells wins.";

    private string tipsText = @"<mark>Winning the Game:</mark>
The player who wins the majority of the three rounds wins the game.
<mark>Tips:</mark>
Pay close attention to where your opponent is placing their points and utilize power cards to disrupt their strategy. Balance your card usage and decide wisely when to skip a round.
Good luck in claiming victory in GWONT!";

    private string setupTitleText = "How to Play: Setup";
    private string gameplayTitleText = "How to Play: Gameplay";
    private string tipsTitleText = "How to Play: Tips";
    string finalButtonText = "Main Menu";

    private int index = 1;

    private void Awake()
    {
        index = 1;
        titleText.text = setupTitleText;
        rulesText.text = setupText;
        buttonText.text = "Next";
    }

    public void ButtonWork()
    {
        index++;
        switch (index)
        {
            // case 1:
            //     titleText.text = setupTitleText;
            //     rulesText.text = setupText;
            //     index++;
            //     break;
            case 2:
                titleText.text = gameplayTitleText;
                rulesText.text = gameplayText;
                break;
            case 3:
                titleText.text = tipsTitleText;
                rulesText.text = tipsText;
                buttonText.text = finalButtonText;
                break;
            case 4:
                index = 1;
                titleText.text = setupTitleText;
                rulesText.text = setupText;
                buttonText.text = "Next";
                mainMenuPanel.SetActive(true);
                rulesPanel.SetActive(false);
                break;
        }
    }


}
