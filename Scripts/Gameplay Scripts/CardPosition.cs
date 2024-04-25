using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardPosition : MonoBehaviour
{
    private List<List<GameObject>> movingCardsLists = new List<List<GameObject>>();
    private List<List<Vector2>> movingPositionsLists = new List<List<Vector2>>();
    private List<List<Vector2>> currentPositionsLists = new List<List<Vector2>>();
    private List<bool> keepActiveList = new List<bool>();
    private List<float> elapsedList = new List<float>();
    private List<int> removalIndexes = new List<int>();
    private float duration = 1f;
    private static CardPosition instance;

    private void Awake()
    {
        // for accessing static values and functions
        instance = this;
    }

    private void Update()
    {
        Moving();
    }

    /// <summary>
    /// There are lists of lists of cards, current(starting) position, moving(finishing) position, each index of these three list signifies
    /// one set of cards in the lerp system.
    /// This method goes through each moving cards lists every update, and updates elapsed for each movingCards lists cards and
    /// updates position of each card of each list of moving cards for each frame. Then deletes the entries
    /// from three lists once elapsed time is not less than duration. 
    /// </summary>
    private void Moving()
    {
        // lets cards read this and be disabled when moving is happening
        // print(movingCardsLists.Count);
        if (movingCardsLists.Count != 0)
        {
            GlobalVariables.moving = true;
        }
        else GlobalVariables.moving = false;

        for (int i = 0; i < movingCardsLists.Count; i++)
        {
            foreach (var VARIABLE in instance.movingPositionsLists)
            {
                //Debug.Log(string.Join(",new ", VARIABLE));
            }
            elapsedList[i] += Time.deltaTime;
            // enter the list of cards 
            for (int j = 0; j < movingCardsLists[i].Count; j++)
            {
                Vector2 position = movingPositionsLists[i][j];
                if (elapsedList[i] < duration)
                {
                    // clamp01 is technically not needed as it is already being clamped by the elapsed/duration
                    float t = Mathf.Clamp01(elapsedList[i] / duration);
                    movingCardsLists[i][j].transform.position = Vector2.Lerp(currentPositionsLists[i][j], position, t);
                }
                else
                {
                    for (int k = 0; k < movingCardsLists[i].Count; k++)
                    {
                        // if timer elapsed, directly set at final position for consistency
                        movingCardsLists[i][k].transform.position = movingPositionsLists[i][k];
                        // if card is being removed from the board, deactivate it
                        if (!keepActiveList[i])
                        {
                            movingCardsLists[i][k].SetActive(false);
                        }
                    }
                    // finished, add to removal list to auto remove
                    removalIndexes.Add(i);
                    break;
                }
            }
        }
        if (removalIndexes.Count != 0)
        {
            removalIndexes.Reverse();
            foreach (var index in removalIndexes)
            {
                movingCardsLists.RemoveAt(index);
                movingPositionsLists.RemoveAt(index);
                currentPositionsLists.RemoveAt(index);
                keepActiveList.RemoveAt(index);
                elapsedList.RemoveAt(index);
            }

            removalIndexes.Clear();
        }
    }
    /// <summary>
    /// Adds the cards and positions to the LERP system. currently doesn't seem very reliable, needs rigorous testing
    /// </summary>
    /// <param name="cards"></param>
    /// <param name="positions"></param>
    public static void MoveCards(List<GameObject> cards, List<Vector2> positions, bool keepActive)
    {
        float elapsed = 0;
        if (instance != null && cards.Count > 0)
        {
            instance.elapsedList.Add(elapsed);
            instance.movingCardsLists.Add(cards);
            instance.movingPositionsLists.Add(positions);
            instance.keepActiveList.Add(keepActive);  // if card gets removed from board and needs to be deactivated
            List<Vector2> temp = new List<Vector2>();
            for (int i = 0; i < cards.Count; i++)
            {
                temp.Add(cards[i].transform.position);

            }
            instance.currentPositionsLists.Add(temp);

        }
    }

}
