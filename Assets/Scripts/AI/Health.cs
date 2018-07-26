using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{
    public int health;
    private Animator m_Anim;
    private AudioSource m_Audio;
    [SerializeField] private AudioClip[] m_AudioClips;

    // Use this for initialization
    void Awake()
    {
        health = 100;
        m_Anim = GetComponent<Animator>();
        m_Audio = GetComponent<AudioSource>();
    }

    public void TakeDamage(int damageTaken, bool knockBack , float dir)
    {
        if (!m_Anim.GetCurrentAnimatorStateInfo(0).IsName("Get Up"))
        {
            health -= damageTaken;
            //update UI health
            if (health <= 0)
            {
                //Debug.Log(gameObject.name + " has died");
                GetComponent<MOMovementController>().Death();
                //go to game over screen or back to menu?
            }
            else if (!knockBack)
            {
                //Debug.Log(gameObject.name + " took " + damageTaken + " damage, Leaving them at " + health + " health");
                m_Anim.SetBool("hurt", true);
                m_Audio.clip = m_AudioClips[0];
                m_Audio.Play();
            }
            else
            {
                GetComponent<MOMovementController>().KnockBack(dir);
            }
        }
    }
}