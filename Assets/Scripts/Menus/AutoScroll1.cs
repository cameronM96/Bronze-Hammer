using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class AutoScroll1 : MonoBehaviour {

    public Scrollbar scrollBar;
    public float minScrollSpeed = 1;
    public float scrollSpeedMultiplier = 4;
    public float updateTimer = 1;
    public float transitionDelay = 5f;
    public float startDelay = 5f;
    public Animator fade;
    private float timer = 0;
    private bool stop = true;

	// Use this for initialization
	void Start () {
        scrollBar = GetComponent<Scrollbar>();
        StartCoroutine(WaitToStart(startDelay));
	}

    IEnumerator WaitToStart(float waitTimer)
    {
        yield return new WaitForSeconds(waitTimer);
        stop = false;
    }
	
	// Update is called once per frame
	void Update ()
    {
        if (!stop)
            timer += Time.deltaTime;

        if (timer >= updateTimer)
        {
            timer = 0;
            if (scrollBar.value > 0)
            {
                scrollBar.value -= ((0.01f* minScrollSpeed) + (-0.01f * (scrollSpeedMultiplier * Input.GetAxis("Vertical"))));
            }
            else
            {
                stop = true;
                fade.SetBool("Skip", true);
                StartCoroutine(ReturnToMenu(transitionDelay));
            }
        }
	}

    IEnumerator ReturnToMenu(float waitTimer)
    {
        yield return new WaitForSeconds(waitTimer);
        SceneManager.LoadScene(0);
    }
}
