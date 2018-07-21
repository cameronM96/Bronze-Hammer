using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EncounterZones : MonoBehaviour {

    private Encounter_Manager m_EncounterManager;
    public int encounterIndex = 0;
    private bool encounterTriggered = false;
    public Text stage;

    private void Awake()
    {
        m_EncounterManager = GetComponentInParent<Encounter_Manager>();
        encounterIndex = transform.GetSiblingIndex();
        stage = GameObject.FindGameObjectWithTag("Stage").GetComponent<Text>();
    }

    private void OnTriggerEnter(Collider other)
    {
        // Trigger encounter if it has not been triggered before and collider is a player.
        if (other.tag == "Player" && !encounterTriggered)
        {
            //Debug.Log("Encounter: " + encounterIndex + " has been triggered");
            encounterTriggered = true;
            m_EncounterManager.beginEncounter = true;
            m_EncounterManager.encounterIndex = encounterIndex;
            stage.text = "[Stage " + (encounterIndex + 1) + "]";
        }
    }
}
