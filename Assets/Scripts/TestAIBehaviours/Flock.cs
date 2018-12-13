using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flock : MonoBehaviour {

    List<Vehicle> boids;

    // Use this for initialization
    void Start () {
        boids = new List<Vehicle>();
    }
	
	// Update is called once per frame
	void Update () {

        if (boids.Count > 0)
        {
            foreach (Vehicle b in boids)
            {
                b.Flock(boids);
            }
        }
	}

    public void AddBoid(Vehicle b)
    {
        boids.Add(b);
    }
}
