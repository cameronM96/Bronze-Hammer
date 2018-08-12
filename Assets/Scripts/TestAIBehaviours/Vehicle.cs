using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Vehicle : MonoBehaviour {

    //Vector3 location;
    private Rigidbody rb;
    private Vector3 velocity;
    private Vector3 acceleration;
    public float r = 1.5f;
    public float maxspeed = 4;
    public float maxforce = 0.1f;
    public float SlowDownDistance = 10;
    public float radius = 5;

    public bool arrival;
    //public bool StayWithInView;
    public bool teleporter;
    public float seekStrength = 0;
    public float fleeStrength = 0;
    public float seperateStrength = 0;
    public float cohesionStrength = 0;
    public float followStrength = 0;
    public float avoidStrength = 0f;         // Strength of avoid behaviour
    private FlowField flow;

    int mask = 1 << 11;            // https://docs.unity3d.com/Manual/Layers.html

    public float leftConstraint = 0.0f;
    public float rightConstraint = 0.0f;
    public float topConstraint = 0.0f;
    public float bottomConstraint = 0.0f;
    public float buffer = 1.0f; // set this so the spaceship disappears offscreen before re-appearing on other side
    public float distanceZ = 10.0f;
    public float distanceX = 10.0f;
    public float angleOfApproach = 30;

    private GameObject attackTarget;
    [SerializeField] private GameObject moveTarget;
    [HideInInspector] public bool rightSide;

	// Use this for initialization
	void Awake ()
    {
        rb = GetComponent<Rigidbody>();
        attackTarget = GameObject.FindGameObjectWithTag("MousePoint");
        flow = GameObject.FindGameObjectWithTag("FlowField").GetComponent<FlowField>();

        seekStrength = 0f;
        fleeStrength = 0f;
        seperateStrength = 0f;
        cohesionStrength = 0f;
        followStrength = 0f;
        avoidStrength = 0f;

        StartCoroutine(WaitTillEndOfFrame());
        //// using a specific distance
        //leftConstraint = Camera.main.ScreenToWorldPoint(new Vector3(0.0f, 0.0f, distanceZ)).x;
        //rightConstraint = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, 0.0f, distanceZ)).x;
        //topConstraint = Camera.main.ScreenToWorldPoint(new Vector3(distanceX, 0.0f, 0.0f)).z;
        //bottomConstraint = Camera.main.ScreenToWorldPoint(new Vector3(distanceX, 0.0f, Screen.height)).x;
    }

    IEnumerator WaitTillEndOfFrame ()
    {
        yield return new WaitForEndOfFrame();
        if (rightSide)
        {
            moveTarget = attackTarget.transform.GetChild(2).gameObject;
        }
        else
        {
            moveTarget = attackTarget.transform.GetChild(1).gameObject;
        }
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
        Vector3 cohesion = Cohesion(vehicles);
        Vector3 seek = Seek(moveTarget.transform.position);
        Vector3 flee = Flee(attackTarget.transform.position);
        Vector3 follow = Follow(flow);
        Vector3 avoid = Avoid(moveTarget,attackTarget);

        seperate *= seperateStrength;
        cohesion *= cohesionStrength;
        seek *= seekStrength;
        flee *= fleeStrength;
        follow *= followStrength;
        avoid *= avoidStrength;

        ApplyForce(seperate);
        ApplyForce(cohesion);
        ApplyForce(seek);
        ApplyForce(flee);
        ApplyForce(follow);
        ApplyForce(avoid);
    }

    void ApplyForce(Vector3 force)
    {
        acceleration += force;
    }

    // Seek target with arriving
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

        //Debug.Log("Seek: " + steer);
        return steer;                           // Using our physics model and applying the force to the
                                                // Object's acceleration.
    }

    // Flee from target
    private Vector3 Flee(Vector3 target)
    {
        Vector3 desired = transform.position - target;

        float d = desired.magnitude;        // Distance is the magnitude of the vector pointing towards target
        desired.Normalize();
        
        desired *= maxspeed;

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

    // Clump together with surrounding vehicles
    private Vector3 Cohesion(List<Vehicle> vehicles)
    {
        float desiredSeparation = r * 2;
        Vector3 sum = Vector3.zero;
        int count = 0;
        foreach (Vehicle v in vehicles)
        {
            float d = Vector3.Distance(transform.position, v.transform.position);
            if ((d > 0) && (d < desiredSeparation))
            {
                Vector3 diff = v.transform.position -  transform.position;
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

    private Vector3 Avoid(GameObject moveT, GameObject attackT)
    {
        Vector3 desired = Vector3.zero;
        float xDifference = attackT.transform.position.x - transform.position.x;                // Used to determine which side of the target the vehicle is on in the z axis.
        //Debug.Log(xDifference);
        float zDifference = attackT.transform.position.z - transform.position.z;                // Used to determine which side of the target the vehicle is on in the z axis.
        float zDistance = Mathf.Abs(attackT.transform.position.z - transform.position.z);       // Used to determine how far away from the target the vehicle is but only use the z axis.
        //Debug.Log(zDifference);
        Vector3 dir = transform.position - attackT.transform.position;
        Vector3 origin = attackT.transform.right;
        float rightSideAngle = Mathf.Abs(Vector3.Angle(origin,dir));        // Checks if vehicle is 45degrees from right vector of target.
        float leftSideAngle = Mathf.Abs(Vector3.Angle(-origin, dir));       // Checks if vehicle is 45degrees from left vector of target.

        if ((moveT.name == "LeftSide" && xDifference < 0 && rightSideAngle < angleOfApproach) ||
            (moveT.name == "RightSide" && xDifference > 0 && leftSideAngle < angleOfApproach))
        {
            if (zDifference > 0)
                desired = Vector3.forward;
            else
                desired = Vector3.back;

            desired.Normalize();

            float value = 0;
            if (zDistance != 0)
                value = SlowDownDistance / zDistance;
            else
                value = SlowDownDistance / 0.001f;

            float m = Map(value, 0, SlowDownDistance, 0, maxspeed);     // Map magnitude, slow down if close.
                                                                        // Debug.Log(m);
            desired *= m * -1;

        }

        //Debug.Log("Avoid desired: " + desired);

        if (desired != Vector3.zero)
        {
            Vector3 steer = desired - velocity;     // Reynold's formula for steering force.
            if (steer.magnitude > maxforce)
            {
                steer.Normalize();
                steer *= maxforce;
            }
            steer.y = 0;

            //Debug.Log("Avoid Steering: " + steer);
            return steer;                           // Using our physics model and applying the force to the
                                                    // Object's acceleration.
        }
        else
        {
            return Vector3.zero;
        }
    }

    private void Teleport()
    {
        if (transform.position.x < leftConstraint - buffer)     // Teleport Left to right;
        {
            rb.isKinematic = true;
            transform.position = new Vector3(rightConstraint, 0, transform.position.z * -1f);
            rb.isKinematic = false;
        }

        if (transform.position.x > rightConstraint + buffer)    // Teleport right to left;
        {
            rb.isKinematic = true;
            transform.position = new Vector3(leftConstraint, 0, transform.position.z * -1f);
            rb.isKinematic = false;
        }

        if (transform.position.z > topConstraint + buffer)      // Teleport Top to bottom;
        {
            rb.isKinematic = true;
            transform.position = new Vector3(transform.position.x * -1f, 0, bottomConstraint);
            rb.isKinematic = false;
        }

        if (transform.position.z < bottomConstraint - buffer)   // Teleport bottom to top
        {
            rb.isKinematic = true;
            transform.position = new Vector3(transform.position.x * -1f, 0, topConstraint);
            rb.isKinematic = false;
        }
    }

    static float Map (float value, float min, float max, float minSpeed, float MaxSpeed)
    {
        return (value - min) * (MaxSpeed - minSpeed) / (max - min) + minSpeed;
    }

    static float InverseMap(float value, float min, float max, float minSpeed, float MaxSpeed)
    {
        return minSpeed + (value - min) * (MaxSpeed - minSpeed) / (max - min);
    }
}
