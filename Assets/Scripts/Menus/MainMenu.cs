using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;

public class MainMenu : MonoBehaviour {

    private int nextLevel;
    private ProceedToLevel proceedToLevel;

    private void Awake()
    {
        if (GameObject.FindGameObjectWithTag("PlayerLives"))
        {
            GameObject.FindGameObjectWithTag("PlayerLives").GetComponent<PlayerLives>().lives = 3;
        }

        proceedToLevel = GameObject.FindGameObjectWithTag("Proceed To Level").GetComponent<ProceedToLevel>();
    }

    public void PlayGame()
    {
        proceedToLevel.nextScene = 2;
        SceneManager.LoadScene(1);
    }

    public void Level1()
    {
        proceedToLevel.nextScene = 2;
        SceneManager.LoadScene(1);
    }

    public void Level2()
    {
        proceedToLevel.nextScene = 4;
        SceneManager.LoadScene(1);
    }

    public void Level3()
    {
        proceedToLevel.nextScene = 6;
        SceneManager.LoadScene(1);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}

