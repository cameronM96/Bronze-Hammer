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
    public bool attack3 = false;
    public bool attack2 = false;

    [SerializeField] private AudioSource m_Audio;

    private void Awake()
    {
        if (parentObject == null || parentObject != this.transform.root.GetChild(0).gameObject)
            StartCoroutine(InitialiseWaitTimer());
    }

    IEnumerator InitialiseWaitTimer()
    {
        yield return new WaitForEndOfFrame();
        parentObject = this.transform.root.GetChild(0).gameObject;
    }

    private void OnTriggerEnter(Collider other)
    {
        //m_Audio.Play();
        if (playerWeapon)
        {
            if (other.gameObject.GetComponent<Health>() && (other.gameObject.tag == "Enemy"|| other.gameObject.tag == "Boss"))
            {
                if (attack3)
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

                    //Debug.Log("Dir = " + dir);
                    if (crag)
                    {
                        other.gameObject.GetComponent<Health>().TakeDamage(attackDamage * comboMultiplier * 2, true, dir);
                    }
                    else
                    {
                        other.gameObject.GetComponent<Health>().TakeDamage(attackDamage * comboMultiplier, true, dir);
                    }
                    //Debug.Log("Target should have been knocked back");
                }
                else if (crag && attack2)
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
                if (!other.gameObject.GetComponent<MOMovementController>().charging)
                {
                    if (attack3)
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
                        other.gameObject.GetComponent<PlayerHealth>().TakeDamage(attackDamage * comboMultiplier, true, dir);
                        GetComponent<Collider>().enabled = false;
                        Debug.Log("Target should have been knocked back");
                    }
                    else
                    {
                        other.gameObject.GetComponent<PlayerHealth>().TakeDamage(attackDamage, false, 0);
                        GetComponent<Collider>().enabled = false;
                        // Debug.Log(gameObject.transform.parent.name + " Hit the " + other.gameObject.name + " for " + attackDamage);
                    }
                }
            }
        }
    }
}
