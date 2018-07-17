using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack : MonoBehaviour {
    public int attackDamage;
    public bool playerWeapon;

	// Use this for initialization
	void Start () {
        attackDamage = 10;
	}

    private void OnTriggerEnter(Collider other)
    {
        if (playerWeapon)
        {
            if (other.gameObject.GetComponent<Health>() && other.gameObject.tag == "Enemy")
            {
                other.gameObject.GetComponent<Health>().TakeDamage(attackDamage);
                Debug.Log(gameObject.transform.parent.name + " Hit the " + other.gameObject.name + " for " + attackDamage);
            }
        }
        else
        {
            if (other.gameObject.GetComponent<PlayerHealth>() && other.gameObject.tag == "Player")
            {
                other.gameObject.GetComponent<PlayerHealth>().TakeDamage(attackDamage);
                Debug.Log(gameObject.transform.parent.name + " Hit the " + other.gameObject.name + " for " + attackDamage);
            }
        }
    }
}
