using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayerHealth : MonoBehaviour {

    [SerializeField] private string characterName;
    [SerializeField] private Text characterNameUI;
    public float health;
    [SerializeField] private Image healthBar;
    [SerializeField] private Text healthText;
    [SerializeField] private float maxHealth;
    public int mana;
    private int totalMaxMana = 0;
    public Image[] manaBars;
    [SerializeField] private Text manaText;
    public int[] manaPerLevel;
    public Text level;
    public int currentMagicLevel;
    public Text livesUI;
    public GameObject deathScreen;
    public GameObject gameOverScreen;
    [SerializeField] private GameObject livesPrefab;
    public PlayerLives playerLives;
    public bool addMana = false;
    private bool dead = false;

    private Animator m_Anim;

    private AudioSource m_Audio;
    [SerializeField] private AudioClip[] m_AudioClips;

    // Use this for initialization
    void Awake()
    {
        characterNameUI.text = characterName;
        // Default Health
        maxHealth = health;
        healthBar.fillAmount = 1;
        healthText.text = "" + health + "/" + maxHealth;

        // Find Components
        m_Anim = GetComponent<Animator>();
        m_Audio = GetComponent<AudioSource>();
        if (!GameObject.FindGameObjectWithTag("PlayerLives"))
        {
            GameObject livesInstance = Instantiate(livesPrefab);
            livesInstance.transform.parent = null;
            livesInstance.transform.position = new Vector3(0, 0, 0);
            playerLives = GameObject.FindGameObjectWithTag("PlayerLives").GetComponent<PlayerLives>();
        }
        else
        {
            playerLives = GameObject.FindGameObjectWithTag("PlayerLives").GetComponent<PlayerLives>();
        }
        livesUI.text = ("" + playerLives.lives);

        // Default mana
        foreach (int levelmax in manaPerLevel)
        {
            totalMaxMana += levelmax;
        }

        manaText.text = "" + mana + "/" + manaPerLevel[currentMagicLevel];
        level.text = "" + currentMagicLevel + "/" + manaPerLevel.Length;
    }

    // Update is called once per frame
    void Update()
    {
        if (addMana)
        {
            AddMana(25);
            addMana = false;
        }
    }

    public void TakeDamage(int damageTaken, bool knockedDown, float dir)
    {
        if (health > 0 && !m_Anim.GetCurrentAnimatorStateInfo(0).IsName("Get Up"))
        {
            health -= damageTaken;
            if (health < 0)
                health = 0;

            healthBar.fillAmount = (health / maxHealth);
            healthText.text = "" + health + "/" + maxHealth;
            //update UI health
            if (health <= 0 && dead)
            {
                dead = true;
                playerLives.LoseLife();
                livesUI.text = ("" + playerLives.lives);
                GetComponent<MOMovementController>().Death();

                if (playerLives.lives <= 0)
                {
                    // Game Over
                    gameOverScreen.SetActive(true);
                    StartCoroutine(GameOver());
                    //go to game over screen
                }
                else
                {
                    // Restart Level
                    //Debug.Log(gameObject.name + " has died");
                    deathScreen.SetActive(true);
                    StartCoroutine(RestartLevel());
                    //go to game over screen
                }
            }
            else if (!knockedDown && health > 0)
            {
                m_Anim.SetBool("hurt", true);
                m_Audio.clip = m_AudioClips[0];
                m_Audio.Play();
                //Debug.Log(gameObject.name + " took " + damageTaken + " damage, Leaving them at " + health + " health");
            }
            else if (knockedDown && health > 0)
            {
                GetComponent<MOMovementController>().KnockBack(dir);
            }
        }
    }

    public void AddMana(int manaValue)
    {
        mana += manaValue;
        if (mana > totalMaxMana)
            mana = totalMaxMana;

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
                if (mana != totalMaxMana)
                {
                    manaText.text = "" + 0 + "/" + manaPerLevel[magiclevel];
                }
                else
                {
                    manaText.text = "" + currentMana + "/" + manaPerLevel[magiclevel -1];
                }
            }
            else if (manaleft != 0)
            {
                // All mana has been accounted for
                currentMana = manaleft;
                manaleft = 0;
                manaText.text = "" + currentMana + "/" + manaPerLevel[magiclevel];
            }
            else
            {
                // All mana has been accounted for
                currentMana = manaleft;
                manaleft = 0;
            }

            // Update this manabar
            manabar.fillAmount = currentMana / maxMana;
        }

        // Update Magic level
        level.text = "" + magiclevel + "/" + manaPerLevel.Length;
        currentMagicLevel = magiclevel;
    }

    public void UseMana()
    {
        mana = 0;

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
        currentMagicLevel = magiclevel;
        manaText.text = "" + mana + "/" + manaPerLevel[magiclevel];
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
