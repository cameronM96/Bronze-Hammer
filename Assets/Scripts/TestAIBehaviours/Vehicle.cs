using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Vehicle : MonoBehaviour {

    //Vector3 location;
    Rigidbody rb;
    Vector3 velocity;
    Vector3 acceleration;
    public float r = 1.5f;
    public float maxspeed = 4;
    public float maxforce = 0.1f;
    public float SlowDownDistance = 10;
    private List<Vehicle> vehicles;
    public float radius = 5;

    int mask = 1 << 11;            // https://docs.unity3d.com/Manual/Layers.html

    GameObject target;

	// Use this for initialization
	void Awake ()
    {
        vehicles = new List<Vehicle>();
        rb = GetComponent<Rigidbody>();
        target = GameObject.FindGameObjectWithTag("MousePoint");
    }

    private void Update()
    {
        Seek(target.transform.position);
        
        SearchArroundVehicle();

        if (vehicles.Count != 0)
            Separate(vehicles);

        velocity += acceleration;

        // Do not exceed max speed
        if (velocity.magnitude > maxspeed)
        {
            velocity.Normalize();
            velocity *= maxspeed;
        }
        acceleration = Vector3.zero;
    }

    private void FixedUpdate()
    {
        // apply force to rigidbody
        rb.velocity = velocity;
        
        // Lookin in direction of velocity
        if (velocity != Vector3.zero)
            transform.GetChild(0).rotation = Quaternion.LookRotation(velocity, Vector3.up);
    }

    // Seek with arriving
    private void Seek(Vector3 target)
    {
        Vector3 desired = target - transform.position;

        float d = desired.magnitude;        // Distance is the magnitude of the vector pointing towards target
        desired.Normalize();

        // Arriving check (slow down when you get close)
        if (d < SlowDownDistance)
        {
            float m = Map(d, 0, SlowDownDistance, 0, maxspeed);     // Map magnitude, slow down if close.
            desired *= m;
        }
        else
        {
            desired *= maxspeed;
        }

        Vector3 steer = desired - velocity;  // Reynold's formula for steering force.
        if (steer.magnitude > maxforce)
        {
            steer.Normalize();
            steer *= maxforce;
        }
        steer.y = 0;

        ApplyForce(steer);                      // Using our physics model and applying the force to the
                                                // Object's acceleration.
    }

    // Find all the vehciles around this vehicle
    void SearchArroundVehicle ()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.GetChild(0).position, radius, mask);
        vehicles = new List<Vehicle>();
        int i = 0;
        while (i < hitColliders.Length)
        {
            if (hitColliders[i].tag == "Vehicle" && 
                hitColliders[i].transform.parent != transform)
                vehicles.Add(hitColliders[i].transform.parent.GetComponent<Vehicle>());
            i++;
        }
    }

    // Move away from surrounding vehicles
    void Separate (List<Vehicle> vehicles)
    {
        float desiredSeparation = r * 2;
        Vector3 sum = Vector3.zero;
        int count = 0;
        foreach (Vehicle v in vehicles)
        {
            float d = Vector3.Distance(transform.position, v.transform.position);
            if ((d > 0) && (d < desiredSeparation))
            {
                Vector3 diff = transform.position - v.transform.position;
                diff.Normalize();
                diff /= d;
                sum += diff;
                count++;
            }
        }
        if (count > 0)
        {
            sum /= count;
            sum.Normalize();
            sum *= maxspeed;

            Vector3 steer = sum - velocity;

            if (steer.magnitude > maxforce)
            {
                steer.Normalize();
                steer *= maxforce;
            }
            ApplyForce(steer);
        }
    }

    void ApplyForce(Vector3 force)
    {
        acceleration += force;
    }

    static float Map (float value, float min, float max, float minSpeed, float MaxSpeed)
    {
        return (value - min) * (MaxSpeed - minSpeed) / (max - min) + minSpeed;
    }
}
