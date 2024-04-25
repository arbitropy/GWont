using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NumberCard : Card
{
    protected override void Awake()
    {
        base.Awake();
        string temp = gameObject.name;
        temp = temp.Replace("(Clone)", string.Empty);
        cardPoints = temp[temp.Length - 1] - 48;
        special = false;
        powerCardType = PowerCardType.General;
    }

    // Update is called once per frame
    void Update()
    {
    }
}