using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerLives : MonoBehaviour {

    private static bool created = false;
    public int lives;

    // Use this for initialization
    private void Awake()
    {
        if (!created)
        {
            DontDestroyOnLoad(this.gameObject);
            created = true;
        }
    }

    // Lose Life
    public int LoseLife ()
    {
        lives -= 1;
        if (lives <= 0)
        {
            lives = 0;
        }
        return lives;
    }
}
