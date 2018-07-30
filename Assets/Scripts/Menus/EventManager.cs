using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EventManager : MonoBehaviour {

    public delegate void PauseGameDelegate();
    public static PauseGameDelegate pauseGame;

    public static void PauseGame()
    {
        if (pauseGame != null)
            pauseGame();
    }
}
