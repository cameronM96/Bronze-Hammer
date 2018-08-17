using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Health : MonoBehaviour
{
    public float health;
    private float maxHealth;
    public int bossHealth;
    private Animator m_Anim;
    private AudioSource m_Audio;
    [SerializeField] private AudioClip[] m_AudioClips;
    private bool dead = false;
    private MOMovementController m_characterController;
    [SerializeField] private GameObject damageIndicator;
    private GameObject gameUI;
    private GameObject bossUI;
    private Image bossHealthUI;

    // Use this for initialization
    void Awake()
    {
        m_characterController = GetComponent<MOMovementController>();
        if (gameObject.tag == "Boss")
        {
            maxHealth = bossHealth;
            gameUI = GameObject.FindGameObjectWithTag("GameUI");
            foreach (Transform t in gameUI.transform)
            {
                if (t.tag == "Boss UI")
                {
                    bossUI = t.gameObject;
                }
            }
            if (bossUI != null)
            {
                bossUI.SetActive(true);
                bossHealthUI = bossUI.transform.GetChild(1).GetChild(1).GetComponentInChildren<Image>();
                bossHealthUI.fillAmount = 1;
            }
        }

        m_Anim = GetComponent<Animator>();
        m_Audio = GetComponent<AudioSource>();
    }

    public void TakeDamage(int damageTaken, bool knockBack, float xdir, float zdir)
    {
        if (gameObject.tag == "Boss")
        {
            bossHealth -= damageTaken;
            GameObject indicator = Instantiate(damageIndicator);
            indicator.transform.position = this.transform.position;
            indicator.transform.GetChild(0).GetChild(0).GetComponent<Text>().text = "" + damageTaken;
            indicator.transform.GetChild(0).GetChild(0).GetComponent<Text>().color = Color.yellow;
            bossHealthUI.fillAmount = bossHealth / maxHealth;

            //update UI health
            if (bossHealth <= 0 && !dead)
            {
                //Debug.Log(gameObject.name + " has died");
                dead = true;
                GetComponent<MOMovementController>().Death();
                //go to game over screen or back to menu?
            }
            else if (health > 0)
            {
                //Debug.Log(gameObject.name + " took " + damageTaken + " damage, Leaving them at " + health + " health");
                m_Anim.SetBool("hurt", true);
                GetComponent<MOMovementController>().hurtAnim = true;
                m_Audio.clip = m_AudioClips[0];
                m_Audio.Play();
            }
        }
        else
        {
            if (!m_characterController.knockedDownAnim)
            {
                health -= damageTaken;
                GameObject indicator = Instantiate(damageIndicator);
                indicator.transform.position = this.transform.position;
                indicator.transform.GetChild(0).GetChild(0).GetComponent<Text>().text = "" + damageTaken;
                //update UI health
                if (health <= 0 && !dead)
                {
                    //Debug.Log(gameObject.name + " has died");
                    dead = true;
                    m_characterController.Death();
                    //go to game over screen or back to menu?
                }
                else if (!knockBack && health > 0)
                {
                    //Debug.Log(gameObject.name + " took " + damageTaken + " damage, Leaving them at " + health + " health");
                    m_Anim.SetBool("hurt", true);
                    GetComponent<MOMovementController>().hurtAnim = true;
                    m_Audio.clip = m_AudioClips[0];
                    m_Audio.Play();
                }
                else if (knockBack && health > 0)
                {
                    //Debug.Log("Knocking back target");
                    m_characterController.knockedDownAnim = true;
                    m_characterController.knockback = true;
                    m_characterController.knockbackxDir = xdir;
                    m_characterController.knockbackzDir = zdir;
                }
            }
        }
    }
}