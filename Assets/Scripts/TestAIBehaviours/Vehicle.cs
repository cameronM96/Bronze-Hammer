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
    public float radius = 5;

    public bool arrival;
    //public bool StayWithInView;
    public bool teleporter;
    public float seekStrength = 1;
    public float seperateStrength = 1;
    public float followStrength = 1;
    private FlowField flow;

    int mask = 1 << 11;            // https://docs.unity3d.com/Manual/Layers.html

    public float leftConstraint = 0.0f;
    public float rightConstraint = 0.0f;
    public float topConstraint = 0.0f;
    public float bottomConstraint = 0.0f;
    public float buffer = 1.0f; // set this so the spaceship disappears offscreen before re-appearing on other side
    public float distanceZ = 10.0f;
    public float distanceX = 10.0f;

    GameObject target;

	// Use this for initialization
	void Awake ()
    {
        rb = GetComponent<Rigidbody>();
        target = GameObject.FindGameObjectWithTag("MousePoint");
        flow = GameObject.FindGameObjectWithTag("FlowField").GetComponent<FlowField>();

        //// using a specific distance
        //leftConstraint = Camera.main.ScreenToWorldPoint(new Vector3(0.0f, 0.0f, distanceZ)).x;
        //rightConstraint = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, 0.0f, distanceZ)).x;
        //topConstraint = Camera.main.ScreenToWorldPoint(new Vector3(distanceX, 0.0f, 0.0f)).z;
        //bottomConstraint = Camera.main.ScreenToWorldPoint(new Vector3(distanceX, 0.0f, Screen.height)).x;
    }

    private void Update()
    {
        ApplyBehaviours(SearchArroundVehicle());

        velocity += acceleration;

        // Do not exceed max speed
        if (velocity.magnitude > maxspeed)
        {
            velocity.Normalize();
            velocity *= maxspeed;
        }
        acceleration = Vector3.zero;

        if (teleporter)
            Teleport();
    }

    private void FixedUpdate()
    {
        // apply force to rigidbody
        rb.velocity = velocity;
        
        // Lookin in direction of velocity
        if (velocity != Vector3.zero)
            transform.GetChild(0).rotation = Quaternion.LookRotation(velocity, Vector3.up);
    }

    private void ApplyBehaviours (List<Vehicle> vehicles)
    {
        Vector3 seperate = Separate(vehicles);
        Vector3 seek = Seek(target.transform.position);
        Vector3 follow = Follow(flow);

        seperate *= seperateStrength;
        seek *= seekStrength;
        follow *= followStrength;

        ApplyForce(seperate);
        ApplyForce(seek);
        ApplyForce(follow);
    }

    // Seek with arriving
    private Vector3 Seek(Vector3 target)
    {
        Vector3 desired = target - transform.position;

        float d = desired.magnitude;        // Distance is the magnitude of the vector pointing towards target
        desired.Normalize();

        if (arrival)
        {
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

        return steer;                           // Using our physics model and applying the force to the
                                                // Object's acceleration.
    }

    // Find all the vehciles around this vehicle
    private List<Vehicle> SearchArroundVehicle ()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.GetChild(0).position, radius, mask);
        List<Vehicle> vehicles = new List<Vehicle>();
        int i = 0;
        while (i < hitColliders.Length)
        {
            if (hitColliders[i].tag == "Vehicle" && 
                hitColliders[i].transform.parent != transform)
                vehicles.Add(hitColliders[i].transform.parent.GetComponent<Vehicle>());
            i++;
        }

        return vehicles;
    }

    // Move away from surrounding vehicles
    private Vector3 Separate (List<Vehicle> vehicles)
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
            steer.y = 0;
            return steer;
        }
        else
        {
            return new Vector3(0, 0, 0);
        }
    }

    private Vector3 Follow(FlowField flow)
    {
        Vector3 desired = flow.LookUp(this.transform.position); // Find steer vector at this location
        desired *= maxspeed;

        Vector3 steer = desired - velocity;  // Reynold's formula for steering force.
        if (steer.magnitude > maxforce)
        {
            steer.Normalize();
            steer *= maxforce;
        }
        steer.y = 0;
        return steer;
    }

    void ApplyForce(Vector3 force)
    {
        acceleration += force;
    }

    private void Teleport()
    {
        if (transform.position.x < leftConstraint - buffer)     // Teleport Left to right;
        {
            rb.isKinematic = true;
            transform.position = new Vector3(rightConstraint, 0, transform.position.z);
            rb.isKinematic = false;
        }

        if (transform.position.x > rightConstraint + buffer)    // Teleport right to left;
        {
            rb.isKinematic = true;
            transform.position = new Vector3(leftConstraint, 0, transform.position.z);
            rb.isKinematic = false;
        }

        if (transform.position.z > topConstraint + buffer)      // Teleport Top to bottom;
        {
            rb.isKinematic = true;
            transform.position = new Vector3(transform.position.x, 0, bottomConstraint);
            rb.isKinematic = false;
        }

        if (transform.position.z < bottomConstraint - buffer)   // Teleport bottom to top
        {
            rb.isKinematic = true;
            transform.position = new Vector3(transform.position.x, 0, topConstraint);
            rb.isKinematic = false;
        }
    }

    static float Map (float value, float min, float max, float minSpeed, float MaxSpeed)
    {
        return (value - min) * (MaxSpeed - minSpeed) / (max - min) + minSpeed;
    }
}
