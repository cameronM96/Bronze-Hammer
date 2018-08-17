using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Magic : MonoBehaviour {

    private GameObject[] enemies;
    private GameObject[] boss;
    [SerializeField] private float waitTimer;
    [SerializeField] private float[] magicLevelMulitplier;
    [SerializeField] private GameObject[] magicVisuals;
    private GameObject magicEffect;
    private CameraShake cameraShake;

    private void Awake()
    {
        cameraShake = Camera.main.GetComponent<CameraShake>();
    }

    public void CastMagic (GameObject caster, float magicDamage, int magicLevel, PlayerCharacters player)
    {
        // Find all enemies
        enemies = GameObject.FindGameObjectsWithTag("Enemy");
        boss = GameObject.FindGameObjectsWithTag("Boss");
        
        if (enemies != null || boss != null)
        {
            switch (player)
            {
                // Determine which character casted the magic
                case PlayerCharacters.Estoc:
                    EstocMagic(magicLevel, caster, magicDamage);
                    break;
                case PlayerCharacters.Lilith:
                    LilithMagic(magicLevel, caster, magicDamage);
                    break;
                case PlayerCharacters.Crag:
                    CragMagic(magicLevel, caster, magicDamage);
                    break;
                default:
                    break;
            }

            StartCoroutine(FreezeEnemies(waitTimer));
        }
        else
        {
            // No enemies Found
        }
    }

    // Freeze enemy timer 
    IEnumerator FreezeEnemies(float waitTimer)
    {
        yield return new WaitForSeconds(waitTimer);

        enemies = GameObject.FindGameObjectsWithTag("Enemy");

        if (enemies != null)
        {
            foreach (GameObject enemy in enemies)
            {
                // Unfreeze enemies
                enemy.GetComponent<MOMovementController>().freeze = false;
            }
        }
    }

    private void DealDamage (float magicDamage, int magicLevel, float xdir, float zdir)
    {
        // Deal damage to all enemies and freeze them for the duration of the spell
        foreach (GameObject enemy in enemies)
        {
            enemy.GetComponent<Health>().TakeDamage(Mathf.RoundToInt(magicDamage * magicLevelMulitplier[magicLevel - 1]), true, xdir, zdir);
            enemy.GetComponent<MOMovementController>().freeze = true;
        }

        // Deal damage to all bosses and freeze them for the duration of the spell
        foreach (GameObject enemy in boss)
        {
            enemy.GetComponent<Health>().TakeDamage(Mathf.RoundToInt(magicDamage * magicLevelMulitplier[magicLevel - 1]), true, 0, 0);
            enemy.GetComponent<MOMovementController>().freeze = true;
        }
    }

    private void EstocLevel3Tornado (GameObject tornado)
    {
        foreach (GameObject enemy in enemies)
        {
            enemy.GetComponentInParent<Rigidbody>().isKinematic = true;
            enemy.GetComponent<Collider>().enabled = false;
            float closestDist = 99999f;
            Transform target = null;
            foreach (Transform child in tornado.transform.GetChild(1))
            {
                float dist = Vector3.Distance(enemy.transform.position, child.position);
                if (dist < closestDist)
                {
                    closestDist = dist;
                    target = child;
                }
            }
            enemy.transform.parent.parent = target;
        }
    }

    private void CragLevel2Damage (GameObject enemy, float magicDamage, int magicLevel, float xdir, float zdir)
    {
        enemy.GetComponent<Health>().TakeDamage(Mathf.RoundToInt(magicDamage * magicLevelMulitplier[magicLevel - 1]), true, xdir, zdir);
        enemy.GetComponent<MOMovementController>().freeze = true;
    }

    // Estoc's spells
    private void EstocMagic(int magicLevel, GameObject caster, float magicDamage)
    {
        switch (magicLevel)
        {
            case 1:
                // Crete Level 1 spell visuals (all enemies)
                foreach (GameObject enemy in enemies)
                {
                    magicEffect = Instantiate(magicVisuals[magicLevel - 1], enemy.transform);
                    magicEffect.transform.parent = null;
                    enemy.transform.parent.GetComponent<Rigidbody>().velocity = Vector3.up * 30;
                }
                // Crete Level 1 spell visuals (all bosses)
                foreach (GameObject enemy in boss)
                {
                    magicEffect = Instantiate(magicVisuals[magicLevel - 1], enemy.transform);
                    magicEffect.transform.parent = null;
                    enemy.transform.parent.GetComponent<Rigidbody>().velocity = Vector3.up * 30;
                }
                //magicEffect = Instantiate(magicVisuals[magicLevel - 1], caster.transform);
                //magicEffect.transform.parent = null;
                DealDamage(magicDamage, magicLevel, 0, 0);
                StartCoroutine(cameraShake.Shake(5f, .1f));
                break;
            case 2:
                // Crete Level 2 spell visuals (Centre multiple)
                magicEffect = Instantiate(magicVisuals[magicLevel - 1]);
                magicEffect.transform.parent = null;
                magicEffect.transform.position = CameraToGround(caster);
                StartCoroutine(cameraShake.Shake(5f, .2f));
                DealDamage(magicDamage, magicLevel, 0, 0);
                break;
            case 3:
                // Crete Level 3 spell visuals (Centre 1)
                magicEffect = Instantiate(magicVisuals[magicLevel - 1]);
                magicEffect.transform.parent = null;
                magicEffect.transform.position = CameraToGround(caster);
                StartCoroutine(cameraShake.Shake(5f, .4f));
                DealDamage(magicDamage, magicLevel, 0, 0);
                EstocLevel3Tornado(magicEffect);
                break;
            default:
                break;
        }
    }

    // Lilith's spells
    private void LilithMagic(int magicLevel, GameObject caster, float magicDamage)
    {
        switch (magicLevel)
        {
            case 1:
                // Crete Level 1 spell visuals (all enemies)
                foreach (GameObject enemy in enemies)
                {
                    magicEffect = Instantiate(magicVisuals[magicLevel - 1], enemy.transform);
                    magicEffect.transform.parent = null;
                }
                foreach (GameObject enemy in boss)
                {
                    magicEffect = Instantiate(magicVisuals[magicLevel - 1], enemy.transform);
                    magicEffect.transform.parent = null;
                    enemy.transform.parent.GetComponent<Rigidbody>().velocity = Vector3.up * 30;
                }
                DealDamage(magicDamage, magicLevel, 0, 0);
                StartCoroutine(cameraShake.Shake(5f, .1f));
                break;
            case 2:
                // Crete Level 1 spell visuals (on Player)
                magicEffect = Instantiate(magicVisuals[magicLevel - 1], caster.transform);
                magicEffect.transform.parent = null;
                StartCoroutine(cameraShake.Shake(5f, .2f));
                DealDamage(magicDamage, magicLevel, 0, 0);
                break;
            case 3:
                // Crete Level 2 spell visuals (Centre)
                magicEffect = Instantiate(magicVisuals[magicLevel - 1]);
                magicEffect.transform.parent = null;
                magicEffect.transform.position = CameraToGround(caster);
                StartCoroutine(cameraShake.Shake(5f, .4f));
                DealDamage(magicDamage, magicLevel, 0, 0);
                break;
            case 4:
                // Crete Level 2 spell visuals (Centre)
                magicEffect = Instantiate(magicVisuals[magicLevel - 1]);
                magicEffect.transform.parent = null;
                magicEffect.transform.position = CameraToGround(caster);
                StartCoroutine(cameraShake.Shake(5f, .6f));
                DealDamage(magicDamage, magicLevel, 0, 0);
                break;
            default:
                break;
        }
    }

    // Crag's spells
    private void CragMagic(int magicLevel, GameObject caster, float magicDamage)
    {
        switch (magicLevel)
        {
            case 1:
                // Crete Level 1 spell visuals (on Player)
                magicEffect = Instantiate(magicVisuals[magicLevel - 1]);
                magicEffect.transform.parent = null;
                magicEffect.transform.position = caster.transform.position;
                magicEffect.transform.localScale *= 2;
                StartCoroutine(cameraShake.Shake(5f, .2f));
                DealDamage(magicDamage, magicLevel, 0, 0);
                break;
            case 2:
                // Crete Level 1 spell visuals (on Player)
                magicEffect = Instantiate(magicVisuals[magicLevel - 1]);
                magicEffect.transform.parent = null;
                magicEffect.transform.position = caster.transform.position;
                magicEffect.transform.localScale *= 2;
                StartCoroutine(cameraShake.Shake(5f, .5f));
                foreach (GameObject enemy in enemies)
                {
                    float dir = 0;
                    if (enemy.transform.parent.position.x > transform.parent.position.x)
                    {
                        dir = 1;
                    }
                    else
                    {
                        dir = -1;
                    }
                    CragLevel2Damage(enemy, magicDamage, magicLevel, dir, 0);
                }
                foreach (GameObject enemy in boss)
                {
                    CragLevel2Damage(enemy, magicDamage, magicLevel, 0, 0);
                }
                break;
            default:
                break;
        }
    }

    private Vector3 CameraToGround (GameObject caster)
    {
        // Raycast from camera down to the ground (Finds the centre of the screen)
        RaycastHit hit;
        //Ray forwardRay = new Ray(gameCamera.transform.position, transform.forward);

        if (Physics.Raycast (Camera.main.transform.position, Camera.main.transform.forward, out hit, 200f))
        {
            return hit.transform.position;
        }
        else
        {
            return caster.transform.position;
        }
    }
}
