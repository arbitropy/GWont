using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpecialCard : Card
{
    protected override void Awake()
    {
        base.Awake();
        string temp = gameObject.name;
        if (temp.Contains("Air"))
        {
            powerCardType = PowerCardType.AirSupport;
        }
        else if (temp.Contains("Assassin"))
        {
            powerCardType = PowerCardType.Assassin;
        }
        else if (temp.Contains("Clear"))
        {
            powerCardType = PowerCardType.ClearWeather;
        }
        else if (temp.Contains("Decoy"))
        {
            powerCardType = PowerCardType.Decoy;
        }
        else if (temp.Contains("Medic"))
        {
            powerCardType = PowerCardType.Medic;
        }
        else if (temp.Contains("Morale"))
        {
            powerCardType = PowerCardType.Morale;
        }
        else if (temp.Contains("Spy"))
        {
            powerCardType = PowerCardType.Spy;
        }
        else if (temp.Contains("Weather"))
        {
            powerCardType = PowerCardType.Weather;
        }

        special = true;
    }
    // Update is called once per frame
    void Update()
    {
    }
}