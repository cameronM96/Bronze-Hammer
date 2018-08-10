using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseToScene : MonoBehaviour {

    public GameObject vehicle;
    private Vector3 mousePosition;

	// Update is called once per frame
	void Update ()
    {
        int mask = 1 << 11;            // https://docs.unity3d.com/Manual/Layers.html
        mask = ~mask;

        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out hit, Mathf.Infinity, mask))
        {
            mousePosition = hit.point;
        }

        this.transform.position = mousePosition;

        if (Input.GetMouseButton(0))
            SpawnVehicle();
	}

    void SpawnVehicle ()
    {
        GameObject newVehicle = Instantiate(vehicle, this.transform);
        newVehicle.transform.parent = null;
        newVehicle.transform.localScale = new Vector3(1, 1, 1);
    }
}
