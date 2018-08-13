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

        if (other.tag == "Mount")
        {
            if (other.GetComponent<MountingController>().isCurrentlyMounted)
            {
                if (other.transform.parent.parent.tag == "Player")
                {
                    if (manaPot)
                    {
                        other.transform.parent.GetComponentInParent<PlayerHealth>().AddMana(manaValue);
                    }
                    else
                    {
                        // Add Health
                    }

                    Destroy(this.gameObject);
                }
            }
        }
    }

    // Wait a while before pot can be picked up
    IEnumerator Collidable ()
    {
        yield return new WaitForSeconds(0.1f);
        triggerCollider.enabled = true;
    }
}
