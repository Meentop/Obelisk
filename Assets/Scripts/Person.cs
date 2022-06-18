using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Person : MonoBehaviour
{
    public string fullName;

    public Transform hairSpawnPoint, beardSpawnPoint, bodySpawnPoint;

    public int industrialEfficiency = 100, combatEfficiency = 100, efficiencyModifier = 0;

    public bool isHungry = false, active = false;

    public void EducateCombat()
    {
        if(combatEfficiency < 150)
        {
            combatEfficiency++;
            industrialEfficiency--;
        }
    }

    public void EducateIndustrial()
    {
        if (industrialEfficiency < 150)
        {
            combatEfficiency--;
            industrialEfficiency++;
        }
    }
}
