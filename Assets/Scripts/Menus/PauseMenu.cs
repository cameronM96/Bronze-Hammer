using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class PauseMenu : MonoBehaviour {

    public Transform PauseBackground;
    bool paused = false;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (paused)
            {

                Resume();
            }
            else
            {
                Pause();
            }
        }
    }

    void Pause()
    {
        PauseBackground.gameObject.SetActive(true);
        Time.timeScale = 0f;
        paused = true;
    }

    void Resume()
    {
        PauseBackground.gameObject.SetActive(false);
        Time.timeScale = 1f;
        paused = false;
    }

	public void Quit ()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(0);
    }
}
