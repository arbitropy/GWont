using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class SkipButton : MonoBehaviour
{
    private void OnMouseEnter()
    {
        transform.localScale = new Vector3(1.1f, 1.1f, 1.1f);
    }

    private void OnMouseExit()
    {
        transform.localScale = new Vector3(1f, 1f, 1f);
    }
    void OnMouseOver()
    {
        if (Input.GetMouseButtonDown(0) && !GlobalVariables.moving && GlobalVariables.playerTurn)
        {
            GlobalVariables.playerSkip = true;
        }
    }
}
