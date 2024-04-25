using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.TextCore.Text;
using UnityEngine.XR;

public class Card : MonoBehaviour
{
    public CardColor cardColor;
    public int cardPoints;
    public int inGameCardPoints; // currently only used in AI simulation to simulation weather and other point changing
    public PowerCardType powerCardType;
    public BoardSide boardSide;
    public Vector3 boardPosition; // used just as temp variable while showing card
    public UIController uiController;
    public Sprite covertSprite;
    public Sprite overtSprite;
    private SpriteRenderer spriteRenderer;
    private BoxCollider2D boxCollider2D;

    // state bools
    public bool selfShowingInfo;
    public bool opponent;
    public bool special;
    public bool onBoard;
    public bool highlighted;
    public bool discared;
    public int playedAtTurn;


    protected virtual void Awake()
    {
        uiController = FindObjectOfType<UIController>();
        string temp = gameObject.name;
        if (temp.Contains("Red"))
        {
            cardColor = CardColor.Red;
        }
        else if (temp.Contains("Green"))
        {
            cardColor = CardColor.Green;
        }
        else
        {
            cardColor = CardColor.Blue;
        }

        StartCoroutine(EnableMouseEnterAfterDelay(1f));
        covertSprite = Resources.Load<Sprite>("Sprites/Cards/Unknown");
        spriteRenderer = GetComponent<SpriteRenderer>();
        overtSprite = spriteRenderer.sprite;

    }

    void Start(){
        inGameCardPoints = cardPoints;
    }
    public void ResetRound()
    {
        onBoard = false;
        highlighted = false;
        selfShowingInfo = false;
        opponent = false;
        discared = true;
        spriteRenderer.sprite = overtSprite;

        inGameCardPoints = cardPoints; // not being used
    }

    public void SetOpponent()
    {
        opponent = true;
        spriteRenderer.sprite = covertSprite;
    }

    private void OnMouseOver()
    {
        // control only if player card
        if (!opponent)
        {
            if (!(GlobalVariables.showingInfo && !selfShowingInfo) || !GlobalVariables.showingInfo)
            {
                if (Input.GetMouseButtonDown(0))
                {
                    if (!onBoard && !selfShowingInfo && !opponent && GlobalVariables.playerTurn)
                    {

                        ToBoard();
                        GlobalVariables.playerTurn = false;
                    }
                }
                else if (Input.GetMouseButtonDown(1))
                {
                    if (GlobalVariables.showingInfo && selfShowingInfo)
                    {
                        UnshowInfo();
                    }
                    else if (!GlobalVariables.showingInfo)
                    {
                        ShowInfo();
                    }
                }
            }
        }
        else
        {
            if (onBoard)
            {
                if (Input.GetMouseButtonDown(1))
                {
                    if (GlobalVariables.showingInfo && selfShowingInfo)
                    {
                        UnshowInfo();
                    }
                    else if (!GlobalVariables.showingInfo)
                    {
                        ShowInfo();
                    }
                }
            }
        }

    }

    void UnshowInfo()
    {
        GlobalVariables.showingInfo = false;
        selfShowingInfo = false;
        Transform transformCache = transform;
        transformCache.position = boardPosition;
        transformCache.localScale = Vector3.one;

        GetComponent<SpriteRenderer>().sortingLayerName = "Cards";

        uiController.StopShowingInfo();
    }

    void ShowInfo()
    {
        GlobalVariables.showingInfo = true;
        selfShowingInfo = true;
        print("showing info");

        Transform transformCache = transform;
        boardPosition = transformCache.position;
        transformCache.position = Vector3.zero;
        transformCache.localScale = 8.5f * transformCache.localScale;

        GetComponent<SpriteRenderer>().sortingLayerName = "Info";

        uiController.ShowInfoText(powerCardType, cardColor);
    }

    public void ToBoard()
    {
        spriteRenderer.sprite = overtSprite;
        if (boardSide == BoardSide.Top)
        {
            if (cardColor == CardColor.Red)
            {
                BoardManagement.AddCardToBoard(BoardCell.TopRed, this.gameObject);
            }
            else if (cardColor == CardColor.Blue)
            {
                BoardManagement.AddCardToBoard(BoardCell.TopBlue, this.gameObject);
            }

            if (cardColor == CardColor.Green)
            {
                BoardManagement.AddCardToBoard(BoardCell.TopGreen, this.gameObject);
            }
        }
        else
        {
            if (cardColor == CardColor.Red)
            {
                BoardManagement.AddCardToBoard(BoardCell.BottomRed, this.gameObject);
            }
            else if (cardColor == CardColor.Blue)
            {
                BoardManagement.AddCardToBoard(BoardCell.BottomBlue, this.gameObject);
            }

            if (cardColor == CardColor.Green)
            {
                BoardManagement.AddCardToBoard(BoardCell.BottomGreen, this.gameObject);
            }
        }
    }


    private void OnMouseEnter()
    {
        print(onBoard);
        print(GlobalVariables.showingInfo);
        print(opponent);
        print(GlobalVariables.moving);
        if (!onBoard && !GlobalVariables.showingInfo && !opponent && !GlobalVariables.moving)
        {
            highlighted = true;
            // print("high " + cardColor);
            HandCardHighlightUp();
        }
    }

    /// <summary>
    /// highlights card below cursor, smooth transition isn't used (yet)
    /// </summary>
    void HandCardHighlightUp()
    {
        var transform1 = transform;
        Vector2 originalPosition = transform1.position;
        originalPosition.y += .2f;
        transform1.position = originalPosition;
        // increase box collider size with movement, and move box collider below to set the offset
        Vector2 tempSize = boxCollider2D.size;
        tempSize.y += .2f;
        boxCollider2D.size = tempSize;
        Vector2 tempOffset = boxCollider2D.offset;
        tempOffset.y -= .1f;
        boxCollider2D.offset = tempOffset;
    }

    void HandCardHighlightDown()
    {
        var transform1 = transform;
        Vector2 originalPosition = transform1.position;
        originalPosition.y -= .2f;
        transform1.position = originalPosition;
        // fix collider size and offset
        Vector2 tempSize = boxCollider2D.size;
        tempSize.y -= .2f;
        boxCollider2D.size = tempSize;
        Vector2 tempOffset = boxCollider2D.offset;
        tempOffset.y += .1f;
        boxCollider2D.offset = tempOffset;
    }

    private void OnMouseExit()
    {
        if (!onBoard && !GlobalVariables.showingInfo && !opponent && !GlobalVariables.moving && highlighted)
        {
            HandCardHighlightDown();
            highlighted = false;
            // print("low " + cardColor);
        }
    }

    private IEnumerator EnableMouseEnterAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        boxCollider2D = gameObject.AddComponent<BoxCollider2D>();
        // matching collider with visible cards size
        boxCollider2D.size = new Vector2(0.56f, 0.82f);

    }
}