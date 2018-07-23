using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayerHealth : MonoBehaviour {

    public float health;
    [SerializeField] private Image healthBar;
    [SerializeField] private float maxHealth;
    public int mana;
    private int maxMana = 0;
    public Image[] manaBars;
    public int[] manaPerLevel;
    public Text level;
    public Text livesUI;
    public GameObject deathScreen;
    public GameObject gameOverScreen;
    [SerializeField] private GameObject livesPrefab;
    public PlayerLives playerLives;
    public bool addMana = false;

    private Animator m_Anim;

    private AudioSource m_Audio;
    [SerializeField] private AudioClip[] m_AudioClips;

    // Use this for initialization
    void Awake()
    {
        health = 100;
        maxHealth = health;
        healthBar.fillAmount = 1;
        m_Anim = GetComponent<Animator>();
        m_Audio = GetComponent<AudioSource>();
        if (!GameObject.FindGameObjectWithTag("PlayerLives"))
        {
            GameObject livesInstance = Instantiate(livesPrefab);
            livesInstance.transform.parent = null;
            livesInstance.transform.position = new Vector3(0, 0, 0);
            playerLives = GameObject.FindGameObjectWithTag("PlayerLives").GetComponent<PlayerLives>();
            Debug.Log("Creating Lives prefab");
        }
        else
        {
            playerLives = GameObject.FindGameObjectWithTag("PlayerLives").GetComponent<PlayerLives>();
            Debug.Log("Lives Prefab found");
        }
        livesUI.text = ("" + playerLives.lives);

        // Get Max mana
        foreach (int levelmax in manaPerLevel)
        {
            maxMana += levelmax;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (addMana)
        {
            AddMana(10);
            addMana = false;
        }
    }

    public void TakeDamage(int damageTaken, bool knockedDown)
    {
        if (health > 0)
        {
            health -= damageTaken;
            if (health < 0)
                health = 0;

            healthBar.fillAmount = (health / maxHealth);
            //update UI health
            if (health <= 0)
            {
                if (playerLives.lives <= 0)
                {
                    // Game Over
                    playerLives.lives -= 1;
                    livesUI.text = ("" + playerLives.LoseLife());
                    GetComponent<MOMovementController>().Death();
                    gameOverScreen.SetActive(true);
                    StartCoroutine(GameOver());
                    //go to game over screen
                }
                else
                {
                    // Restart Level
                    //Debug.Log(gameObject.name + " has died");
                    playerLives.lives -= 1;
                    livesUI.text = ("" + playerLives.LoseLife());
                    GetComponent<MOMovementController>().Death();
                    deathScreen.SetActive(true);
                    StartCoroutine(RestartLevel());
                    //go to game over screen
                }
            }
            else if (!knockedDown)
            {
                m_Anim.SetBool("hurt", true);
                m_Audio.clip = m_AudioClips[0];
                m_Audio.Play();
                //Debug.Log(gameObject.name + " took " + damageTaken + " damage, Leaving them at " + health + " health");
            }
        }
    }

    public void AddMana(int manaValue)
    {
        mana += manaValue;
        if (mana > maxMana)
            mana = maxMana;

        float manaleft = mana;
        int magiclevel = 0;

        // Set each bar relative to the manaPerLevel
        foreach (Image manabar in manaBars)
        {
            float currentMana = 0;
            float maxMana = manaPerLevel[magiclevel];

            if (manaleft >= maxMana)
            {
                // If there is still mana left over, increase magic level and reset manaLeft.
                currentMana = maxMana;
                manaleft -= maxMana;
                ++magiclevel;
            }
            else
            {
                // All mana has been acounted for
                currentMana = manaleft;
                manaleft = 0;
            }

            // Update this manabar
            manabar.fillAmount = currentMana / maxMana;
        }

        // Update Magic level
        level.text = "" + magiclevel;
    }

    IEnumerator RestartLevel ()
    {
        yield return new WaitForSeconds(3);
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    IEnumerator GameOver ()
    {
        yield return new WaitForSeconds(3);
        SceneManager.LoadScene("Main Menu Scene");
    }
}
