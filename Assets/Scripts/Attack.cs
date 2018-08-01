using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack : MonoBehaviour
{
    public int attackDamage = 10;
    public int comboMultiplier = 2;
    public bool playerWeapon;
    [SerializeField] private bool crag = false;
    [SerializeField] private GameObject parentObject;
    private Animator m_Anim;

    [SerializeField] private AudioSource m_Audio;

    private void Awake()
    {
        StartCoroutine(InitialiseWaitTimer());
    }

    IEnumerator InitialiseWaitTimer()
    {
        yield return new WaitForEndOfFrame();
        parentObject = this.transform.root.GetChild(0).gameObject;
        m_Anim = parentObject.GetComponent<Animator>();
    }

    private void OnTriggerEnter(Collider other)
    {
        //m_Audio.Play();
        if (playerWeapon)
        {
            if (other.gameObject.GetComponent<Health>() && other.gameObject.tag == "Enemy")
            {
                if (m_Anim.GetCurrentAnimatorStateInfo(0).IsName("Attack3 (KNOCKBACK)") ||
                    m_Anim.GetCurrentAnimatorStateInfo(0).IsName("Dash Attack"))
                {
                    float dir = 0;
                    if (other.transform.position.x > transform.parent.position.x)
                    {
                        dir = 1;
                    }
                    else
                    {
                        dir = -1;
                    }
                    if (crag)
                        other.gameObject.GetComponent<Health>().TakeDamage(attackDamage * comboMultiplier * 2, true, dir);
                    else
                        other.gameObject.GetComponent<Health>().TakeDamage(attackDamage * comboMultiplier, true, dir);
                }
                else if (crag && m_Anim.GetCurrentAnimatorStateInfo(0).IsName("Attack2"))
                {
                    other.gameObject.GetComponent<Health>().TakeDamage(attackDamage * comboMultiplier, false, 0);
                }
                else
                {
                    other.gameObject.GetComponent<Health>().TakeDamage(attackDamage, false, 0);
                    // Debug.Log(gameObject.transform.parent.name + " Hit the " + other.gameObject.name + " for " + attackDamage);
                }
            }

            if (other.tag == "Chicken")
            {
                other.transform.parent.gameObject.GetComponent<ChickenAI>().KickChicken();
            }
        }
        else
        {
            if (other.gameObject.GetComponent<PlayerHealth>() && other.gameObject.tag == "Player")
            {
                if (m_Anim.GetCurrentAnimatorStateInfo(0).IsName("Attack3 (KNOCKBACK)") ||
                    m_Anim.GetCurrentAnimatorStateInfo(0).IsName("Dash Attack"))
                {
                    float dir = 0;
                    if (other.transform.position.x > transform.parent.position.x)
                    {
                        dir = 1;
                    }
                    else
                    {
                        dir = -1;
                    }
                    other.gameObject.GetComponent<PlayerHealth>().TakeDamage(attackDamage * comboMultiplier, true, dir);
                    GetComponent<Collider>().enabled = false;
                }
                else
                {
                    other.gameObject.GetComponent<PlayerHealth>().TakeDamage(attackDamage, false , 0);
                    GetComponent<Collider>().enabled = false;
                    // Debug.Log(gameObject.transform.parent.name + " Hit the " + other.gameObject.name + " for " + attackDamage);
                }
            }
        }
    }
}
