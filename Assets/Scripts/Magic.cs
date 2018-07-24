﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Magic : MonoBehaviour {

    private GameObject[] enemies;
    [SerializeField] private float waitTimer;
    [SerializeField] private GameObject[] magicVisuals;
    private Camera gameCamera;
    private float sightDistance = 200f;
    private int layerMask = 0;
    GameObject magicEffect;

    private void Start()
    {
        gameCamera = Camera.main;
    }

    public void CastMagic (GameObject caster, float magicDamage, int magicLevel, int player)
    {
        enemies = GameObject.FindGameObjectsWithTag("Enemy");


        if (enemies != null)
        {
            foreach (GameObject enemy in enemies)
            {
                enemy.GetComponent<Health>().TakeDamage(Mathf.RoundToInt(magicDamage * magicLevel), false);
                enemy.GetComponent<MOMovementController>().freeze = true;
            }
        }

        switch (player)
        {
            case 0:
                EstocMagic(magicLevel, caster);
                break;
            case 1:
                LilithMagic(magicLevel, caster);
                break;
            case 2:
                CragMagic(magicLevel, caster);
                break;
            default:
                break;
        }

        StartCoroutine(FreezeEnemies(waitTimer));
    }

    IEnumerator FreezeEnemies(float waitTimer)
    {
        yield return new WaitForSeconds(waitTimer);
        enemies = GameObject.FindGameObjectsWithTag("Enemy");
        if (enemies != null)
        {
            foreach (GameObject enemy in enemies)
            {
                enemy.GetComponent<MOMovementController>().freeze = false;
            }
        }
    }

    private void EstocMagic(int magicLevel, GameObject caster)
    {
        switch (magicLevel)
        {
            case 1:
                Debug.Log("Level 1 Estoc Magic Cast! Magic Cast on Player");
                magicEffect = Instantiate(magicVisuals[magicLevel], caster.transform);
                magicEffect.transform.parent = null;
                break;
            case 2:
                Debug.Log("Level 2 Estoc Magic Cast! Magic Cast in centre of screen");
                magicEffect = Instantiate(magicVisuals[magicLevel], CameraToGround(caster));
                magicEffect.transform.parent = null;
                magicEffect.transform.position = CameraToGround(caster).position;
                break;
            case 3:
                Debug.Log("Level 3 Estoc Magic Cast! Magic Cast on Enemies");
                foreach (GameObject enemy in enemies)
                {
                    magicEffect = Instantiate(magicVisuals[magicLevel], enemy.transform);
                    magicEffect.transform.parent = null;
                }
                break;
            default:
                break;
        }
    }

    private void LilithMagic(int magicLevel, GameObject caster)
    {
        switch (magicLevel)
        {
            case 1:
                magicEffect = Instantiate(magicVisuals[magicLevel], caster.transform);
                magicEffect.transform.parent = null;
                break;
            case 2:
                magicEffect = Instantiate(magicVisuals[magicLevel], caster.transform);
                magicEffect.transform.parent = null;
                break;
            case 3:
                magicEffect = Instantiate(magicVisuals[magicLevel], caster.transform);
                magicEffect.transform.parent = null;
                break;
            case 4:
                magicEffect = Instantiate(magicVisuals[magicLevel], caster.transform);
                magicEffect.transform.parent = null;
                break;
            default:
                break;
        }
    }

    private void CragMagic(int magicLevel, GameObject caster)
    {
        switch (magicLevel)
        {
            case 1:
                magicEffect = Instantiate(magicVisuals[magicLevel], caster.transform);
                magicEffect.transform.parent = null;
                break;
            case 2:
                magicEffect = Instantiate(magicVisuals[magicLevel], caster.transform);
                magicEffect.transform.parent = null;
                break;
            default:
                break;
        }
    }

    private Transform CameraToGround (GameObject caster)
    {
        RaycastHit hit;
        Ray forwardRay = new Ray(gameCamera.transform.position, transform.forward);

        if (Physics.Raycast (forwardRay, out hit, sightDistance))
        {
            Debug.Log("Ray hit ground at: " + hit.transform.position);
            return hit.transform;
        }
        else
        {
            Debug.Log("Ray hit nothing, creating on player");
            return caster.transform;
        }
    }
}
