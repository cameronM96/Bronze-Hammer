using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack : MonoBehaviour {
    public int attackDamage;

	// Use this for initialization
	void Start () {
        attackDamage = 10;
	}

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponent<Health>())
        {
            other.gameObject.GetComponent<Health>().TakeDamage(attackDamage);
            Debug.Log(gameObject.transform.parent.name+" Hit the " + other.gameObject.name+" for "+attackDamage);
        }
    }
}
