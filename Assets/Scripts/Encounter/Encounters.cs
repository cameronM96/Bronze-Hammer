using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class Encounters {

    [HideInInspector] public string name = "Encounter: ";
    public GameObject encounterPoint;   // Number of Encounters there will be per level.
    public Waves[] waves;               // Number of waves there will be per encounter.

    [HideInInspector] public int waveNumber = 0;
    
    public Encounters(GameObject newEncounter, Waves[] newWaves)
    {
        encounterPoint = newEncounter;
        waves = newWaves;
    }
}
