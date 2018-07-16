using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{
    public int health;

    // Use this for initialization
    void Start()
    {
        health = 100;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void TakeDamage(int damageTaken)
    {
        health -= damageTaken;
        //update UI health
        if (health <= 0)
        {
            Debug.Log(gameObject.name + " has died");
            Destroy(gameObject);
            //go to game over screen or back to menu?
        }
        else
        {
            Debug.Log(gameObject.name + " took " + damageTaken + " damage, Leaving them at " + health + " health");
        }
    }
}