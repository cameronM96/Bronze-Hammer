using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MountAttack : MonoBehaviour {

    public int attackDamage = 10;
    public bool playerAttack;
    [SerializeField] private bool ranged;

    private void OnTriggerEnter(Collider other)
    {
        //m_Audio.Play();
        if (playerAttack)
        {
            if (other.gameObject.GetComponent<Health>() && other.gameObject.tag == "Enemy")
            {
                other.gameObject.GetComponent<Health>().TakeDamage(attackDamage, false, 0);
                // Debug.Log(gameObject.transform.parent.name + " Hit the " + other.gameObject.name + " for " + attackDamage);
                if (ranged)
                    Destroy(this.transform.parent.parent);
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
                other.gameObject.GetComponent<PlayerHealth>().TakeDamage(attackDamage, false, 0);
                // Debug.Log(gameObject.transform.parent.name + " Hit the " + other.gameObject.name + " for " + attackDamage);
                if (ranged)
                    Destroy(this.transform.parent.parent);
            }
        }
    }
}
