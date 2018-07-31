using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProceedToLevel : MonoBehaviour {

    public int nextScene = 2;
    private static bool created = false;

    void Start()
    {
        if (!created)
        {
            DontDestroyOnLoad(this.gameObject);
            created = true;
        }
    }
}
