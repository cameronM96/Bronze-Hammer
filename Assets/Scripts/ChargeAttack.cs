using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChargeAttack : MonoBehaviour {

    public int attackDamage = 15;
    public bool playerWeapon;
    [SerializeField] private GameObject parentObject;
    private IEnumerator endCharge;

    private void Awake()
    {
        StartCoroutine(InitialiseWaitTimer());
        endCharge = EndCharge();
    }

    IEnumerator InitialiseWaitTimer()
    {
        yield return new WaitForEndOfFrame();
        parentObject = this.transform.root.GetChild(0).gameObject;
    }

    IEnumerator EndCharge()
    {
        yield return new WaitForSeconds(0.02f);
        parentObject.GetComponent<MOMovementController>().charging = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (playerWeapon)
        {
            if (other.gameObject.GetComponent<Health>() && (other.gameObject.tag == "Enemy" || other.gameObject.tag == "Boss"))
            {
                bool knockBack = true;
                float dir = 0;
                if (other.transform.position.x > parentObject.transform.position.x)
                {
                    dir = 1;
                }
                else
                {
                    dir = -1;
                }

                if (other.gameObject.tag == "Boss")
                    knockBack = false;

                other.gameObject.GetComponent<Health>().TakeDamage(attackDamage, knockBack, dir, 0);
                StartCoroutine(endCharge);
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
                float dir = 0;
                if (other.transform.position.x > parentObject.transform.position.x)
                {
                    dir = 1;
                }
                else
                {
                    dir = -1;
                }
                other.gameObject.GetComponent<PlayerHealth>().TakeDamage(attackDamage, true, dir);
                StartCoroutine(endCharge);
            }
        }
    }
}
