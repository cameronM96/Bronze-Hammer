using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Potion : MonoBehaviour {

    [SerializeField] private bool manaPot = true;
    [SerializeField] private int manaValue = 25;
    [SerializeField] private int healthValue;
    [SerializeField] private Collider triggerCollider;

    private void Awake()
    {
        StartCoroutine(Collidable());
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            if (manaPot)
            {
                other.GetComponent<PlayerHealth>().AddMana(manaValue);
            }
            else
            {
                // Add Health
            }

            Destroy(this.gameObject);
        }
    }

    // Wait a while before pot can be picked up
    IEnumerator Collidable ()
    {
        yield return new WaitForSeconds(0.2f);
        triggerCollider.enabled = true;
    }
}
