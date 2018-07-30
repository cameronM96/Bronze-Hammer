using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Encounter_Manager : MonoBehaviour {

    public static Encounter_Manager Instance = null;
    public static Camera_Follow camera_Instance;

    public Transform rightSpawnPoint;
    public Transform leftSpawnPoint;
    
    public List<Encounters> m_EncountersList = new List<Encounters>();

    [HideInInspector] public int encounterIndex;
    [HideInInspector] public bool beginEncounter;

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
                    Transform newSpawnPoint =  rightSpawnPoint;

                    // Determine spawn location
                    if (switchSides1)
                    {
                        newSpawnPoint.position =
                            new Vector3(rightSpawnPoint.position.x,
                            rightSpawnPoint.position.y,
                            rightSpawnPoint.position.z + z1);
                        switchSides1 = !switchSides1;
                    }
                    else
                    {
                        newSpawnPoint.position =
                            new Vector3(rightSpawnPoint.position.x,
                            rightSpawnPoint.position.y,
                            rightSpawnPoint.position.z - z1);
                        switchSides1 = !switchSides1;
                    }

                    if (enemy.transform.GetChild(0).tag == "Chicken")
                    {
                        newEnemy = Instantiate(enemy);
                        newEnemy.transform.parent = null;
                        newEnemy.GetComponent<UnityEngine.AI.NavMeshAgent>().Warp(rightSpawnPoint.position);
                    }
                    else
                    {
                        newEnemy = Instantiate(enemy, rightSpawnPoint);
                        newEnemy.transform.parent = null;
                    }

                    z1 += 2;
                }

                bool switchSides2 = false;
                foreach (GameObject enemy in 
                    m_EncountersList[encounterIndex].waves[m_EncountersList[encounterIndex].waveNumber].leftSideEnemies)
                {
                    GameObject newEnemy;
                    Transform newSpawnPoint = leftSpawnPoint;

                    // Determine spawn location
                    if (switchSides2)
                    {
                        newSpawnPoint.position =
                            new Vector3(leftSpawnPoint.position.x,
                            leftSpawnPoint.position.y,
                            leftSpawnPoint.position.z + z2);
                        switchSides2 = !switchSides2;
                    }
                    else
                    {
                        newSpawnPoint.position =
                            new Vector3(leftSpawnPoint.position.x,
                            leftSpawnPoint.position.y,
                            leftSpawnPoint.position.z - z2);
                        switchSides2 = !switchSides2;
                    }

                    if (enemy.transform.GetChild(0).tag == "Chicken")
                    {
                        newEnemy = Instantiate(enemy);
                        newEnemy.transform.parent = null;
                        newEnemy.GetComponent<UnityEngine.AI.NavMeshAgent>().Warp(rightSpawnPoint.position);
                    }
                    else
                    {
                        newEnemy = Instantiate(enemy, rightSpawnPoint);
                        newEnemy.transform.parent = null;
                    }
                    z2 += 2;
                }
                
                ++m_EncountersList[encounterIndex].waveNumber;
            }

            // If all waves have been spawned end encounter.
            if (m_EncountersList[encounterIndex].waves.Length <= m_EncountersList[encounterIndex].waveNumber && 
                CheckForEnemies() == 0)
            {
                beginEncounter = false;
                camera_Instance.NewBoundary();
                m_Audio.Play();
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
