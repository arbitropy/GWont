using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;

public class BoardBounds : MonoBehaviour
{
    /// <summary>
    /// Position of each cell, list contains two vectors, first position second size.
    /// </summary>
    public static Dictionary<BoardCell, List<Vector2>> cellPositions;

    public static float halfGap = 0, halfCard;
    private Vector2 inHandCardPosition = new Vector2(0, 3.9f);
    [SerializeField] private GameObject tempCard;
    // Start is called before the first frame update
    void Awake()
    {
        cellPositions = new Dictionary<BoardCell, List<Vector2>>();
        //Updates board cell positions at the start
        for (int i = 0; i < 6; i++)
        {
            List<Vector2> tempList = new List<Vector2>();
            Transform temp = transform.GetChild(i);
            tempList.Add(temp.position);
            tempList.Add(temp.GetComponent<BoxCollider2D>().size);
            cellPositions.Add((BoardCell)i, tempList);
        }

        //Gets card sizes once
        tempCard = Instantiate(tempCard);
        halfCard = tempCard.GetComponent<SpriteRenderer>().size.y / 3.5f;
        DestroyImmediate(tempCard, true);
    }

    /// <summary>
    /// Gets cell info and number of cards, and returns a list of positions for the cards to put in
    /// </summary>
    /// <param name="cell"></param>
    /// <param name="cards"></param>
    public static List<Vector2> GetPositionInBoard(BoardCell cell, int cards)
    {
        List<Vector2> results = new List<Vector2>();
        //print(cards + " cards");
        for (int i = -(cards - 1); i <= (cards - 1); i += 2)
        {
            Vector2 temp = Vector2.zero;
            temp.x = cellPositions[cell][0].x + i * (halfCard + halfGap);
            temp.y = cellPositions[cell][0].y;
            results.Add(temp);
        }
        //Debug.Log(string.Join(", ", results));
        return results;
    }

    /// <summary>
    /// Reuses GetPositionInBoard to get position for cards in hand
    /// </summary>
    /// <param name="cards"></param>
    /// <param name="boardSide"></param>
    /// <returns></returns>
    public static List<Vector2> GetPositionInHand(int cards, BoardSide boardSide)
    {
        List<Vector2> results = GetPositionInBoard(BoardCell.BottomBlue, cards);
        int offset = 1;
        if (boardSide != BoardSide.Top)
        {
            offset = -1;
        }
        for (int i = 0; i < results.Count; i++)
        {
            // x position was given by positioninboard, y is fixed by player side * 3.9f
            results[i] = new Vector2(results[i].x, (float)offset * 3.9f);
        }
        //Debug.Log(string.Join(", ", results));
        return results;
    }
}