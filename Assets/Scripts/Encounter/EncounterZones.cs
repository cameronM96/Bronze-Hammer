using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EncounterZones : MonoBehaviour {

    private Encounter_Manager m_EncounterManager;
    public int encounterIndex = 0;
    private bool encounterTriggered = false;

    private void Awake()
    {
        m_EncounterManager = GetComponentInParent<Encounter_Manager>();
        encounterIndex = transform.GetSiblingIndex();
    }

    private void OnTriggerEnter(Collider other)
    {
        // Trigger encounter if it has not been triggered before and collider is a player.
        if (other.tag == "Player" && !encounterTriggered)
        {
            Debug.Log("Encounter: " + encounterIndex + " has been triggered");
            encounterTriggered = true;
            m_EncounterManager.beginEncounter = true;
            m_EncounterManager.encounterIndex = encounterIndex;
        }
    }
}
