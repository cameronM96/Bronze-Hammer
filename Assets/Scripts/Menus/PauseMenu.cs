using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class PauseMenu : MonoBehaviour {

    public Transform PauseBackground;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            if (PauseBackground.gameObject.activeInHierarchy == false)
                {
                PauseBackground.gameObject.SetActive(true);
                }
            else
            {
                PauseBackground.gameObject.SetActive(false);
            }
        }
    }

	public void Quit ()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 0);
    }
}
