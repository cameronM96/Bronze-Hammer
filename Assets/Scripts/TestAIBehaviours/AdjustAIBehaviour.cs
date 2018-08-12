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
    public Slider avoidStrength;
    public Button resetButton;
    public bool viewArrows;
    public bool moveArrows;
    public bool randomizeBehaviour;

    private GameObject[] vehicles;

    public GameObject flowField;

    public float updateTimer = 1;
    private float timer = 0;
        

	// Use this for initialization
	void Start ()
    {
        StartCoroutine(WaitTillEndOfFrame());
	}
	
    IEnumerator WaitTillEndOfFrame()
    {
        yield return new WaitForEndOfFrame();
        UpdateVehiclesBehaviour(FindVehicles());
    }

	// Update is called once per frame
	void Update ()
    {
        timer += Time.deltaTime;

        if (timer >= updateTimer)
        {
            if (randomizeBehaviour)
                RandomizeBehaviour();

            UpdateVehiclesBehaviour(FindVehicles());
            flowField.GetComponent<FlowField>().updateOn = moveArrows;
        }
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
            v.avoidStrength = avoidStrength.value;
        }

        if (followStrength.value == 0)
        {
            if (flowField.transform.GetChild(0).gameObject.activeSelf)
            {
                foreach (Transform arrow in flowField.transform)
                {
                    arrow.gameObject.SetActive(false);
                }
            }
        }
        else if (viewArrows)
        {
            if (!flowField.transform.GetChild(0).gameObject.activeSelf)
            {
                foreach (Transform arrow in flowField.transform)
                {
                    arrow.gameObject.SetActive(true);
                }
            }
        }
        else
        {
            if (flowField.transform.GetChild(0).gameObject.activeSelf)
            {
                foreach (Transform arrow in flowField.transform)
                {
                    arrow.gameObject.SetActive(false);
                }
            }
        }
    }

    void RandomizeBehaviour()
    {
        seekStrength.value += Random.Range(-0.1f, 0.1f);
        fleeStrength.value += Random.Range(-0.1f, 0.1f);
        seperateStrength.value += Random.Range(-0.1f, 0.1f);
        cohesionStrength.value += Random.Range(-0.1f, 0.1f);
        followStrength.value += Random.Range(-0.1f, 0.1f);
        avoidStrength.value += Random.Range(-0.1f, 0.1f);
    }

    public void RemoveVehicles()
    {
        GameObject[] vehicles = FindVehicles();
        foreach(GameObject vehicle in vehicles)
        {
            Destroy(vehicle.transform.parent.gameObject);
        }
    }
}
