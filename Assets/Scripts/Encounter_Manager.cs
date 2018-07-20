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

    private void Awake()
    {
        Instance = this;
        camera_Instance = Camera.main.GetComponent<Camera_Follow>();

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
                foreach (GameObject enemy in 
                    m_EncountersList[encounterIndex].waves[m_EncountersList[encounterIndex].waveNumber].rightSideEnemies)
                {
                    //TODO: Fan the enemies out a bit (like +/- 2 or 3 z each iteration)
                    // Instantiate(enemy, rightSpawnPoint);
                }

                foreach (GameObject enemy in 
                    m_EncountersList[encounterIndex].waves[m_EncountersList[encounterIndex].waveNumber].leftSideEnemies)
                {
                    // Instantiate(enemy, leftSpawnPoint);
                }
                
                ++m_EncountersList[encounterIndex].waveNumber;
            }

            // If all waves have been spawned end encounter.
            if (m_EncountersList[encounterIndex].waves.Length <= m_EncountersList[encounterIndex].waveNumber && 
                CheckForEnemies() == 0)
            {
                beginEncounter = false;
                camera_Instance.NewBoundary();
            }
        }
    }

    private int CheckForEnemies()
    {
        // Find all enemies in the scene and return the number of enemies.
        //TODO: Maybe change this so its triggered when an enemy is killed and not every frame. (for performance)
        GameObject[] enemiesInScene = GameObject.FindGameObjectsWithTag("Enemy");

        return enemiesInScene.Length;
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
