using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{
    public int health;
    public int bossHealth;
    private Animator m_Anim;
    private AudioSource m_Audio;
    [SerializeField] private AudioClip[] m_AudioClips;
    private bool dead = false;

    // Use this for initialization
    void Awake()
    {
        if (gameObject.tag == "Boss")
        {
            bossHealth = 250;
        }
        else
        {
            health = 100;
        }
        m_Anim = GetComponent<Animator>();
        m_Audio = GetComponent<AudioSource>();
    }

    public void TakeDamage(int damageTaken, bool knockBack, float dir)
    {
        if (gameObject.tag == "Boss")
        {
            bossHealth -= damageTaken;
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
                m_Audio.clip = m_AudioClips[0];
                m_Audio.Play();
            }
        }
        else
        {
            if (!m_Anim.GetCurrentAnimatorStateInfo(0).IsName("Get Up"))
            {
                health -= damageTaken;
                //update UI health
                if (health <= 0 && !dead)
                {
                    //Debug.Log(gameObject.name + " has died");
                    dead = true;
                    GetComponent<MOMovementController>().Death();
                    //go to game over screen or back to menu?
                }
                else if (!knockBack && health > 0)
                {
                    //Debug.Log(gameObject.name + " took " + damageTaken + " damage, Leaving them at " + health + " health");
                    m_Anim.SetBool("hurt", true);
                    m_Audio.clip = m_AudioClips[0];
                    m_Audio.Play();
                }
                else if (knockBack && health > 0)
                {
                    GetComponent<MOMovementController>().KnockBack(dir);
                }
            }
        }

    }
}