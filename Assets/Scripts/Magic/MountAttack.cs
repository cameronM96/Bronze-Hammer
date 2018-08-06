using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MountAttack : MonoBehaviour {

    public int attackDamage = 10;
    public bool playerAttack;

    private void OnTriggerEnter(Collider other)
    {
        //m_Audio.Play();
        if (playerAttack)
        {
            if (other.gameObject.GetComponent<Health>() && other.gameObject.tag == "Enemy")
            {
                // Determine which direction to send the target in when hit
                float dir = 0;
                if (other.transform.position.x > transform.position.x)
                {
                    dir = 1;
                }
                else
                {
                    dir = -1;
                }

                other.gameObject.GetComponent<Health>().TakeDamage(attackDamage, true, dir);
                // Debug.Log(gameObject.transform.parent.name + " Hit the " + other.gameObject.name + " for " + attackDamage);
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
                // Determine which direction to send the target in when hit
                float dir = 0;
                if (other.transform.position.x > transform.position.x)
                {
                    dir = 1;
                }
                else
                {
                    dir = -1;
                }

                other.gameObject.GetComponent<PlayerHealth>().TakeDamage(attackDamage, true, dir);
                // Debug.Log(gameObject.transform.parent.name + " Hit the " + other.gameObject.name + " for " + attackDamage);
            }
        }
    }
}
