using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TransitionSceneController : MonoBehaviour {

    public bool textFinished = false;
    private SelectedPlayer selectedPlayer;
    [SerializeField] private GameObject estoc;
    [SerializeField] private GameObject lilith;
    [SerializeField] private GameObject crag;
    [SerializeField] private Animator fadeScreen;
    private GameObject activeText;

    // Use this for initialization
    void Start()
    {
        selectedPlayer = GameObject.FindGameObjectWithTag("Character Selector").GetComponent<SelectedPlayer>();

        estoc = transform.GetChild(1).gameObject;
        lilith = transform.GetChild(2).gameObject;
        crag = transform.GetChild(3).gameObject;
        fadeScreen = transform.GetChild(4).GetComponent<Animator>();

        if (selectedPlayer != null)
        {
            if (selectedPlayer.estoc)
                EstocText();

            if (selectedPlayer.lilith)
                LilithText();

            if (selectedPlayer.crag)
                CragText();
        }
        else
        {
            EstocText();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (textFinished)
        {
            if (Input.anyKeyDown)
            {
                fadeScreen.SetBool("Skip", true);
                StartCoroutine(LoadScene());
            }
        }
        else
        {
            if(Input.anyKey)
            {
                activeText.GetComponent<TypingLetters>().delay = 0.01f;
            }
            else
            {
                activeText.GetComponent<TypingLetters>().delay = 0.05f;
            }

            textFinished = activeText.GetComponent<TypingLetters>().finished;
        }
    }

    IEnumerator LoadScene()
    {
        yield return new WaitForSeconds(3f);

        if (SceneManager.GetActiveScene().buildIndex == 8)
        {
            SceneManager.LoadScene(0);
        }
        else
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }
    }

    void EstocText()
    {
        estoc.SetActive(true);
        lilith.SetActive(false);
        crag.SetActive(false);
        activeText = estoc;
    }

    void LilithText()
    {
        estoc.SetActive(false);
        lilith.SetActive(true);
        crag.SetActive(false);
        activeText = lilith;
    }

    void CragText()
    {
        estoc.SetActive(false);
        lilith.SetActive(false);
        crag.SetActive(true);
        activeText = crag;
    }
}
