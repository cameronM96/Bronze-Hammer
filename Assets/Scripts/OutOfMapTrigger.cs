using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OutOfMapTrigger : MonoBehaviour {

    private int damage = 10000;

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Enemy")
        {
            other.gameObject.GetComponent<Health>().TakeDamage(damage, false, 0);
        }

        if (other.tag == "Player")
        {
            other.gameObject.GetComponent<PlayerHealth>().TakeDamage(damage, false, 0);
        }
    }
}
