using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour {

    public int health;
    [SerializeField] private Slider healthBar;

    // Use this for initialization
    void Start()
    {
        health = 100;
        healthBar.maxValue = health;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void TakeDamage(int damageTaken)
    {
        health -= damageTaken;
        healthBar.value = health;
        //update UI health
        if (health <= 0)
        {
            //Debug.Log(gameObject.name + " has died");
            //Destroy(gameObject);
            //go to game over screen or back to menu?
        }
        else
        {
            //Debug.Log(gameObject.name + " took " + damageTaken + " damage, Leaving them at " + health + " health");
        }
    }
}
