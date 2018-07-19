using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(Encounter_Manager))]
public class Encounter_Manager_Inspector : MonoBehaviour {

    public Encounter_Manager instance;

	// Use this for initialization
	void Start ()
    {
        instance = GetComponent<Encounter_Manager>();
    }
	
	// Update is called once per frame
	void Update ()
    {
        int i = 1;
        foreach (Encounters encounter in instance.m_EncountersList)
        {
            encounter.name = "Encounter: " + i;
            i++;
            int j = 1;
            foreach (Waves wave in encounter.waves)
            {
                wave.name = "Wave: " + j;
                j++;
            }
        }
	}
}
