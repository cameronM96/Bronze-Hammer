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
    public float mana;
    [SerializeField] private float totalMaxMana = 0;
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
    [SerializeField] private GameObject damageIndicator;
    [SerializeField] private Image hitIndicator;

    private Animator m_Anim;

    private AudioSource m_Audio;
    [SerializeField] private AudioClip[] m_AudioClips;
    private MOMovementController m_characterController;
    private CameraShake cameraShake;

    // Use this for initialization
    void Awake()
    {
        m_characterController = GetComponent<MOMovementController>();
        characterNameUI.text = characterName;

        cameraShake = Camera.main.GetComponent<CameraShake>();
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

        AddMana(manaPerLevel[0]);
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
        if (health > 0 && !m_characterController.knockedDownAnim)
        {
            health -= damageTaken;
            GameObject indicator = Instantiate(damageIndicator);
            indicator.transform.position = this.transform.position;
            indicator.transform.GetChild(0).GetChild(0).GetComponent<Text>().text = "" + damageTaken;
            indicator.transform.GetChild(0).GetChild(0).GetComponent<Text>().color = Color.red;
            StartCoroutine(FlashScreen(new Color(200, 0, 0)));
            StartCoroutine(cameraShake.Shake(0.4f,0.1f));

            if (health < 0)
                health = 0;

            healthBar.fillAmount = (health / maxHealth);
            healthText.text = "" + health + "/" + maxHealth;
            //update UI health
            if (health <= 0 && !dead)
            {
                dead = true;
                playerLives.LoseLife();
                livesUI.text = ("" + playerLives.lives);
                m_characterController.Death();

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
                m_characterController.knockedDownAnim = true;
                m_characterController.knockback = true;
                m_characterController.knockbackDir = dir;
            }
        }
    }

    IEnumerator FlashScreen(Color color)
    {
        hitIndicator.enabled = true;
        color.a = 0.31f;
        hitIndicator.color = color;
        yield return new WaitForSeconds(0.4f);
        hitIndicator.enabled = false;
    }

    public void AddMana(int manaValue)
    {
        mana += manaValue;
        if (mana > totalMaxMana)
            mana = totalMaxMana;

        float manaleft = mana;
        int magiclevel = 0;

        GameObject indicator = Instantiate(damageIndicator);
        indicator.transform.position = this.transform.position;
        indicator.transform.GetChild(0).GetChild(0).GetComponent<Text>().text = "" + manaValue;
        indicator.transform.GetChild(0).GetChild(0).GetComponent<Text>().color = Color.blue;
        StartCoroutine(FlashScreen(Color.blue));

        // Sound
        m_Audio.clip = m_AudioClips[1];
        m_Audio.Play();

        float manaSoFar = 0;
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
                    
                }

                manaSoFar += currentMana;

                // Update this manabar
                manabar.fillAmount = (manaSoFar / totalMaxMana);
            }
            else if (manaleft != 0)
            {
                // All mana has been accounted for
                currentMana = manaleft;
                manaleft = 0;

                manaSoFar += currentMana;

                // Update this manabar
                manabar.fillAmount = (manaSoFar / totalMaxMana);
            }
            else
            {
                // All mana has been accounted for
                currentMana = manaleft;
                manaleft = 0;
            }

        }

        // Update Magic level
        manaText.text = "" + mana + "/" + totalMaxMana;
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
