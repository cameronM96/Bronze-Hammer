using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AdjustAIBehaviour : MonoBehaviour {

    public Slider seekStrength;
    public Slider fleeStrength;
    public Slider seperateStrength;
    public Slider cohesionStrength;
    public Slider followStrength;

    private GameObject[] vehicles;

    public GameObject flowField;

    public float updateTimer = 1;
    private float timer = 0;
        

	// Use this for initialization
	void Start ()
    {
        UpdateVehiclesBehaviour(FindVehicles());
	}
	
	// Update is called once per frame
	void Update ()
    {
        timer += Time.deltaTime;

        if (timer >= updateTimer)
            UpdateVehiclesBehaviour(FindVehicles());
	}

    private GameObject[] FindVehicles ()
    {
        return GameObject.FindGameObjectsWithTag("Vehicle");
    }

    void UpdateVehiclesBehaviour (GameObject[] vehicles)
    {
        foreach(GameObject vehicle in vehicles)
        {
            Vehicle v = vehicle.transform.parent.GetComponent<Vehicle>();
            v.seekStrength = seekStrength.value;
            v.fleeStrength = fleeStrength.value;
            v.seperateStrength = seperateStrength.value;
            v.cohesionStrength = cohesionStrength.value;
            v.followStrength = followStrength.value;
        }

        if (followStrength.value == 0)
        {
            foreach (Transform arrow in flowField.transform)
            {
                arrow.gameObject.SetActive(false);
            }
        }
        else
        {
            foreach (Transform arrow in flowField.transform)
            {
                arrow.gameObject.SetActive(true);
            }
        }
    }
}
