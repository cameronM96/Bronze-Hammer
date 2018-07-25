using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack : MonoBehaviour
{
    public int attackDamage = 10;
    public int comboMultiplier = 2;
    public bool playerWeapon;
    public bool knockback;

    [SerializeField] private AudioSource m_Audio;

	// Use this for initialization
	void Start ()
    {
        knockback = false;
	}

    private void OnTriggerEnter(Collider other)
    {
        //m_Audio.Play();
        if (playerWeapon)
        {
            if (other.gameObject.GetComponent<Health>() && other.gameObject.tag == "Enemy" && knockback)
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
                other.gameObject.GetComponent<MOMovementController>().KnockBack(dir);
                other.gameObject.GetComponent<Health>().TakeDamage(attackDamage * comboMultiplier , true);
                knockback = false;
            }
            else if (other.gameObject.GetComponent<Health>() && other.gameObject.tag == "Enemy")
            {
                other.gameObject.GetComponent<Health>().TakeDamage(attackDamage, false);
                Debug.Log(gameObject.transform.parent.name + " Hit the " + other.gameObject.name + " for " + attackDamage);
            }

            if (other.tag == "Chicken")
            {
                other.transform.parent.gameObject.GetComponent<ChickenAI>().KickChicken();
            }
        }
        else
        {

            if (other.gameObject.GetComponent<PlayerHealth>() && other.gameObject.tag == "Player" && knockback)
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
                other.gameObject.GetComponent<MOMovementController>().KnockBack(dir);
                other.gameObject.GetComponent<PlayerHealth>().TakeDamage(attackDamage * comboMultiplier, true);
                knockback = false;
            }
            else if (other.gameObject.GetComponent<PlayerHealth>() && other.gameObject.tag == "Player")
            {
                other.gameObject.GetComponent<PlayerHealth>().TakeDamage(attackDamage, false);
                Debug.Log(gameObject.transform.parent.name + " Hit the " + other.gameObject.name + " for " + attackDamage);
            }
        }
    }
}
