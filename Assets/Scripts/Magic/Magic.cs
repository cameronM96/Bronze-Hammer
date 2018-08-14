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
            // Deal damage to all enemies and freeze them for the duration of the spell
            foreach (GameObject enemy in enemies)
            {
                enemy.transform.parent.GetComponent<Rigidbody>().velocity = new Vector3(0, 0, 0);

                bool knockback = true;
                if (enemy.GetComponent<MOMovementController>().mounted)
                    knockback = false;

                enemy.GetComponent<Health>().TakeDamage(Mathf.RoundToInt(magicDamage * magicLevelMulitplier[magicLevel - 1]), knockback, 0);
                enemy.GetComponent<MOMovementController>().freeze = true;
            }

            // Deal damage to all bosses and freeze them for the duration of the spell
            foreach (GameObject enemy in boss)
            {
                enemy.transform.parent.GetComponent<Rigidbody>().velocity = new Vector3(0, 0, 0);

                bool knockback = true;
                if (enemy.GetComponent<MOMovementController>().mounted)
                    knockback = false;

                enemy.GetComponent<Health>().TakeDamage(Mathf.RoundToInt(magicDamage * magicLevelMulitplier[magicLevel - 1]), knockback, 0);
                enemy.GetComponent<MOMovementController>().freeze = true;
            }

            switch (player)
            {
                // Determine which character casted the magic
                case PlayerCharacters.Estoc:
                    EstocMagic(magicLevel, caster);
                    break;
                case PlayerCharacters.Lilith:
                    LilithMagic(magicLevel, caster);
                    break;
                case PlayerCharacters.Crag:
                    CragMagic(magicLevel, caster);
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

    // Estoc's spells
    private void EstocMagic(int magicLevel, GameObject caster)
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
                StartCoroutine(cameraShake.Shake(5f, .1f));
                break;
            case 2:
                // Crete Level 2 spell visuals (Centre multiple)
                magicEffect = Instantiate(magicVisuals[magicLevel - 1]);
                magicEffect.transform.parent = null;
                magicEffect.transform.position = CameraToGround(caster);
                StartCoroutine(cameraShake.Shake(5f, .2f));
                break;
            case 3:
                // Crete Level 3 spell visuals (Centre 1)
                magicEffect = Instantiate(magicVisuals[magicLevel - 1]);
                magicEffect.transform.parent = null;
                magicEffect.transform.position = CameraToGround(caster);
                StartCoroutine(cameraShake.Shake(5f, .4f));
                break;
            default:
                break;
        }
    }

    // Lilith's spells
    private void LilithMagic(int magicLevel, GameObject caster)
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
                StartCoroutine(cameraShake.Shake(5f, .1f));
                break;
            case 2:
                // Crete Level 1 spell visuals (on Player)
                magicEffect = Instantiate(magicVisuals[magicLevel - 1], caster.transform);
                magicEffect.transform.parent = null;
                StartCoroutine(cameraShake.Shake(5f, .2f));
                break;
            case 3:
                // Crete Level 2 spell visuals (Centre)
                magicEffect = Instantiate(magicVisuals[magicLevel - 1]);
                magicEffect.transform.parent = null;
                magicEffect.transform.position = CameraToGround(caster);
                StartCoroutine(cameraShake.Shake(5f, .4f));
                break;
            case 4:
                // Crete Level 2 spell visuals (Centre)
                magicEffect = Instantiate(magicVisuals[magicLevel - 1]);
                magicEffect.transform.parent = null;
                magicEffect.transform.position = CameraToGround(caster);
                StartCoroutine(cameraShake.Shake(5f, .6f));
                break;
            default:
                break;
        }
    }

    // Crag's spells
    private void CragMagic(int magicLevel, GameObject caster)
    {
        switch (magicLevel)
        {
            case 1:
                // Crete Level 1 spell visuals (on Player)
                magicEffect = Instantiate(magicVisuals[magicLevel - 1], caster.transform);
                magicEffect.transform.parent = null;
                StartCoroutine(cameraShake.Shake(5f, .2f));
                break;
            case 2:
                // Crete Level 1 spell visuals (on Player)
                magicEffect = Instantiate(magicVisuals[magicLevel - 1], caster.transform);
                magicEffect.transform.parent = null;
                StartCoroutine(cameraShake.Shake(5f, .5f));
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
