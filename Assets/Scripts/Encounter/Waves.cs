using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class Waves {

    [HideInInspector] public string name = "Wave: ";
    public GameObject[] rightSideEnemies;
    public GameObject[] leftSideEnemies;
}
