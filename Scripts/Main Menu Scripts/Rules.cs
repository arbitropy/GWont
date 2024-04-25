using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rules : MonoBehaviour
{
    public GameObject rulesPanel;
    public GameObject mainMenuPanel;
    void OnMouseOver()
    {
        if (Input.GetMouseButtonDown(0))
        {
            rulesPanel.SetActive(true);
            mainMenuPanel.SetActive(false);
        }
    }
}
