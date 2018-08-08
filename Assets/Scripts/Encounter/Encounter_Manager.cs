using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class Encounter_Manager : MonoBehaviour {

    public static Encounter_Manager Instance = null;
    public static Camera_Follow camera_Instance;

    public Transform rightSpawnPoint;
    public Transform leftSpawnPoint;
    
    public List<Encounters> m_EncountersList = new List<Encounters>();

    [HideInInspector] public int encounterIndex;
    [HideInInspector] public bool beginEncounter;
    private bool flash;
    public Image arrowIndicator;
    private float flashTimer;
    private float flashArrow;
    public float flashDuration = 3f;
    public float flashFrequency = 0.3f;

    private AudioSource m_Audio;

    private void Awake()
    {
        Instance = this;
        camera_Instance = Camera.main.GetComponent<Camera_Follow>();
        rightSpawnPoint = camera_Instance.transform.GetChild(1);
        leftSpawnPoint = camera_Instance.transform.GetChild(0);

        m_Audio = GetComponent<AudioSource>();

        foreach (Transform child in this.transform)
        {
            child.GetComponent<MeshRenderer>().enabled = false;
        }

        arrowIndicator.enabled = false;
    }

    private void Update()
    {
        // If the player has triggered one of the child colliders start encounter.
        if (beginEncounter && m_EncountersList[encounterIndex] != null)
        {
            // If there are no enemies in the scene spawn the next wave of the encounter
            if (CheckForEnemies() == 0 && m_EncountersList[encounterIndex].waves.Length > 
                m_EncountersList[encounterIndex].waveNumber) // -1 from length?
            {
                DebugEncounters();
                // Begin wave by spawning enemies
                int z1 = 0;
                int z2 = 0;

                bool switchSides1 = false;
                foreach (GameObject enemy in 
                    m_EncountersList[encounterIndex].waves[m_EncountersList[encounterIndex].waveNumber].rightSideEnemies)
                {
                    GameObject newEnemy;
                    Vector3 newSpawnPoint =  rightSpawnPoint.position;

                    // Determine spawn location
                    if (switchSides1)
                    {
                        newSpawnPoint =
                            new Vector3(rightSpawnPoint.position.x,
                            rightSpawnPoint.position.y,
                            rightSpawnPoint.position.z + z1);
                        switchSides1 = !switchSides1;
                    }
                    else
                    {
                        newSpawnPoint =
                            new Vector3(rightSpawnPoint.position.x,
                            rightSpawnPoint.position.y,
                            rightSpawnPoint.position.z - z1);
                        switchSides1 = !switchSides1;
                    }

                    if (enemy.transform.GetChild(0).tag == "Chicken")
                    {
                        NavMeshHit hit;
                        if (NavMesh.SamplePosition(newSpawnPoint, out hit, 100.0f, NavMesh.AllAreas))
                        {
                            newSpawnPoint = hit.position;
                        }

                        newEnemy = Instantiate(enemy);
                        newEnemy.transform.parent = null;
                        newEnemy.GetComponent<NavMeshAgent>().Warp(newSpawnPoint);
                    }
                    else
                    {
                        newEnemy = Instantiate(enemy, newSpawnPoint, new Quaternion(0,0,0,0));
                        newEnemy.transform.parent = null;
                    }

                    z1 += 2;
                }

                bool switchSides2 = false;
                foreach (GameObject enemy in 
                    m_EncountersList[encounterIndex].waves[m_EncountersList[encounterIndex].waveNumber].leftSideEnemies)
                {
                    GameObject newEnemy;
                    Vector3 newSpawnPoint = leftSpawnPoint.position;

                    // Determine spawn location
                    if (switchSides2)
                    {
                        newSpawnPoint =
                            new Vector3(leftSpawnPoint.position.x,
                            leftSpawnPoint.position.y,
                            leftSpawnPoint.position.z + z2);
                        switchSides2 = !switchSides2;
                    }
                    else
                    {
                        newSpawnPoint =
                            new Vector3(leftSpawnPoint.position.x,
                            leftSpawnPoint.position.y,
                            leftSpawnPoint.position.z - z2);
                        switchSides2 = !switchSides2;
                    }

                    if (enemy.transform.GetChild(0).tag == "Chicken")
                    {
                        NavMeshHit hit;
                        if (NavMesh.SamplePosition(newSpawnPoint, out hit, 100.0f, NavMesh.AllAreas))
                        {
                            newSpawnPoint = hit.position;
                        }

                        newEnemy = Instantiate(enemy);
                        newEnemy.transform.parent = null;
                        newEnemy.GetComponent<NavMeshAgent>().Warp(newSpawnPoint);
                    }
                    else
                    {
                        newEnemy = Instantiate(enemy, newSpawnPoint, new Quaternion(0, 0, 0, 0));
                        newEnemy.transform.parent = null;
                    }
                    z2 += 2;
                }
                
                ++m_EncountersList[encounterIndex].waveNumber;
            }

            // If all waves have been spawned, end encounter.
            if (m_EncountersList[encounterIndex].waves.Length <= m_EncountersList[encounterIndex].waveNumber && 
                CheckForEnemies() == 0)
            {
                beginEncounter = false;
                camera_Instance.NewBoundary();
                m_Audio.Play();
                flash = true;
                flashTimer = 0;
            }
        }

        // Flash the arrow indicator after encounter ends
        if (flash)
        {
            flashTimer += Time.deltaTime;
            flashArrow += Time.deltaTime;
            if (flashTimer <= flashDuration)
            {
                if (flashArrow >= flashFrequency)
                {
                    arrowIndicator.enabled = !arrowIndicator.enabled;
                    flashArrow = 0;
                }
            }
            else
            {
                flash = false;
                arrowIndicator.enabled = false;
            }
        }
    }

    private int CheckForEnemies()
    {
        // Find all enemies in the scene and return the number of enemies.
        //TODO: Maybe change this so its triggered when an enemy is killed and not every frame. (for performance)
        GameObject[] enemiesInScene = GameObject.FindGameObjectsWithTag("Enemy");
        GameObject[] chickens = GameObject.FindGameObjectsWithTag("Chicken");

        return (enemiesInScene.Length + chickens.Length);
    }

    private void DebugEncounters()
    {
        // Debug function which tells you which encounter was triggered, how many waves it has and the contents of each wave.
        Debug.Log("Encounter triggered! Starting encounter: " + (encounterIndex + 1) + "\n");
        Debug.Log("Wave: " + (m_EncountersList[encounterIndex].waveNumber + 1) + " contains " +
            m_EncountersList[encounterIndex].waves[m_EncountersList[encounterIndex].waveNumber].rightSideEnemies.Length +
            " enemies on the right and " +
            m_EncountersList[encounterIndex].waves[m_EncountersList[encounterIndex].waveNumber].leftSideEnemies.Length +
            " enemies on the left.\n");
    }
}
