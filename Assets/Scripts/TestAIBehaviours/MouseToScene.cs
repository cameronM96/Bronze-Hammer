using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MouseToScene : MonoBehaviour {

    public GameObject vehicle;
    private Vector3 mousePosition;
    private bool rightside = false;
    public bool onButtonDown;

	// Update is called once per frame
	void Update ()
    {
        int mask = 1 << 11;            // https://docs.unity3d.com/Manual/Layers.html
        mask = ~mask;
        bool hitFloor = false;

        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out hit, Mathf.Infinity, mask))
        {
            mousePosition = hit.point;
            if (!EventSystem.current.IsPointerOverGameObject())
                hitFloor = true;
        }

        this.transform.position = mousePosition;

        if (onButtonDown)
        {
            if (Input.GetMouseButtonDown(0) && hitFloor)
            {
                rightside = !rightside;
                SpawnVehicle(rightside);
            }
        }
        else
        {
            if (Input.GetMouseButton(0) && hitFloor)
            {
                rightside = !rightside;
                SpawnVehicle(rightside);
            }
        }
	}

    void SpawnVehicle (bool rightSide)
    {
        GameObject newVehicle = Instantiate(vehicle, this.transform);
        newVehicle.transform.parent = null;
        newVehicle.transform.localScale = new Vector3(1, 1, 1);
        Debug.Log(rightSide);
        newVehicle.GetComponent<Vehicle>().rightSide = rightSide;
    }
}
