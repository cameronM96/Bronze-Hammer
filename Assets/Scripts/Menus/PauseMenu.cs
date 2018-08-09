using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;
using UnityEngine.EventSystems;

public class PauseMenu : MonoBehaviour {

    public Transform PauseBackground;
    [SerializeField] private EventSystem eventSystem;
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
        PauseBackground.transform.GetChild(0).GetChild(0).gameObject.SetActive(true);
        PauseBackground.transform.GetChild(0).GetChild(1).gameObject.SetActive(false);
        PauseBackground.transform.GetChild(0).GetChild(2).gameObject.SetActive(false);
        eventSystem.SetSelectedGameObject(PauseBackground.transform.GetChild(0).GetChild(0).GetChild(0).gameObject);
        eventSystem.SetSelectedGameObject(PauseBackground.transform.GetChild(0).GetChild(0).GetChild(1).gameObject);
        Time.timeScale = 0f;
        paused = true;
    }

    public void Resume()
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
