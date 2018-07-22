﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour {

    public int health;
    [SerializeField] private Image healthBar;
    private int maxHealth;
    public int mana;
    private int maxMana = 0;
    [SerializeField] private Image[] manaBars;
    [SerializeField] private int[] manaPerLevel;
    [SerializeField] private Text level;
    public bool addMana = false;

    // Use this for initialization
    void Awake()
    {
        health = 100;
        maxHealth = health;
        healthBar.fillAmount = 1;

        // Get Max mana
        foreach (int levelmax in manaPerLevel)
        {
            maxMana += levelmax;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (addMana)
        {
            AddMana(10);
            addMana = false;
        }
    }

    public void TakeDamage(int damageTaken, bool knockedDown)
    {
        health -= damageTaken;
        healthBar.fillAmount = health / maxHealth;
        //update UI health
        if (health <= 0)
        {
            //Debug.Log(gameObject.name + " has died");
            GetComponent<MOMovementController>().Death();
            //go to game over screen or back to menu?
        }
        else
        {
            //Debug.Log(gameObject.name + " took " + damageTaken + " damage, Leaving them at " + health + " health");
        }
    }

    public void AddMana(int manaValue)
    {
        mana += manaValue;
        if (mana > maxMana)
            mana = maxMana;

        float manaleft = mana;
        int magiclevel = 0;

        // Set each bar relative to the manaPerLevel
        foreach (Image manabar in manaBars)
        {
            float currentMana = 0;
            float maxMana = manaPerLevel[magiclevel];

            if (manaleft >= maxMana)
            {
                // If there is still mana left over, increase magic level and reset manaLeft.
                currentMana = maxMana;
                manaleft -= maxMana;
                ++magiclevel;
            }
            else
            {
                // All mana has been acounted for
                currentMana = manaleft;
                manaleft = 0;
            }

            // Update this manabar
            manabar.fillAmount = currentMana / maxMana;
        }

        // Update Magic level
        level.text = "" + magiclevel;
    }
}
