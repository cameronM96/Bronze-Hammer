using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AIController : MOMovementController
{
    public GameObject enemy;                // the entity to be controlled
    public GameObject moveTarget;           // The Target to move towards
    public GameObject attackTarget;         // The Target to attack

    private MOMovementController m_character;
    // private NavMeshAgent agent;

    // AI Steering Behaviour variables
    public float moveSpeed = 7.0f;          //the speed the AI will move
    Vector3 velocity;
    Vector3 acceleration;
    public float r = 5f;                  // Detection range
    public float maxforce = 0.1f;           // Acceleration rate
    public float SlowDownDistance = 10;     // Start slowing down when getting close to destination;
    int mask = 1 << 11 | 1 << 10;      // https://docs.unity3d.com/Manual/Layers.html, What to detect (AI and player only)
    public float radius = 5;

    public bool arrival;                    // Enable arrival behaviour
    public float seekStrength = 1;          // Strength of seek behaviour
    public float seperateStrength = 1.2f;      // Strength of seperate behaviour
    public float avoidStrength = 1.5f;         // Strength of avoid behaviour
    //[SerializeField] private float jumpHeight = 10.0f; //the height of the AI's jump
    private float hMov;
    private float vMov;
    //private bool jump;
    private bool attack;
    //private bool tooClose;
    public float angleOfApproach = 60;
    public bool SmoothAvoid = true;

    public bool sprinting;
    public float attackDistance = 4;
    [HideInInspector] public float meleeAttackDistance;
    private bool stop;

    // Use this for initialization
    protected override void Awake ()
    {
        base.Awake();

        meleeAttackDistance = attackDistance;

        radius = 5;
        r = 7f;
        seekStrength = 1f;
        seperateStrength = 1.5f;
        avoidStrength = 0f;
        SmoothAvoid = false;
        angleOfApproach = 50;

        m_character = GetComponent<MOMovementController>();

        enemy = this.gameObject;

        // This will need to change when there are multiple players
        attackTarget = GameObject.FindGameObjectWithTag("Player");

        moveTarget = attackTarget;
    }

    // Update is called once per frame
    protected override void Update ()
    {
        base.Update();

        //agent.destination = moveTarget.transform.position;

        ApplyBehaviours(SearchArroundAI());

        velocity += acceleration;

        // Do not exceed max speed
        if (velocity.magnitude > moveSpeed)
        {
            velocity.Normalize();
            velocity *= moveSpeed;
        }
        acceleration = Vector3.zero;
        
        // add steering for when there are other AI in the way
        float dist1 = Vector3.Distance(attackTarget.transform.position, enemy.transform.position);
        float dist2 = Vector3.Distance(moveTarget.transform.position, enemy.transform.position);
        //Debug.Log(dist);
        // Attack if enemy is close enough and roughly the same z position AND facing the player
        if (dist1 < meleeAttackDistance)
        {
            //Debug.Log("Attacking");
            attack = true;
        }
        else
        {
            attack = false;
        }

        // Add a check for jumping
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();
        //call the method on the controller script sending the required vars
        if(attack && !m_character.dead)
        {
            // Make sure AI is facing the right directions first
            if (!m_character.attackingAnim)
            {
                Vector3 lookTarget = new Vector3(
                    attackTarget.transform.position.x,
                    this.transform.position.y,
                    attackTarget.transform.position.z);
                this.transform.parent.LookAt(lookTarget);
            }
            m_character.Move(Vector3.zero);
            m_character.Attack(false);
        }
        else
        {
            if (!stop)
            {
                // Prevent movement if an AI is too close
                //if (!tooClose)
                m_character.Move(velocity);
            }
        }
    }

    private void ApplyBehaviours(List<AIController> aiList)
    {
        Vector3 seperate = Separate(aiList);
        Vector3 seek = Seek(moveTarget.transform.position);
        //Vector3 follow = Follow(flow);
        Vector3 avoid = Avoid(moveTarget, attackTarget);

        seperate *= seperateStrength;
        seek *= seekStrength;
        //follow *= followStrength;
        avoid *= avoidStrength;

        ApplyForce(seperate);
        ApplyForce(seek);
        //ApplyForce(follow);
        ApplyForce(avoid);
    }

    // Seek target with arriving
    private Vector3 Seek(Vector3 target)
    {
        Vector3 newTarget = target;
        // Adjust target if they are inside the "Avoid" zone
        if (SmoothAvoid)
        {
            float xDifference = attackTarget.transform.position.x - transform.parent.position.x;                // Used to determine which side of the target the vehicle is on in the z axis.
                                                                                                                //Debug.Log(xDifference);
            float zDifference = attackTarget.transform.position.z - transform.parent.position.z;                // Used to determine which side of the target the vehicle is on in the z axis.
            //float zDistance = Mathf.Abs(attackTarget.transform.position.z - transform.parent.position.z);       // Used to determine how far away from the target the vehicle is but only use the z axis.
                                                                                                                //Debug.Log(zDifference);
            Vector3 dir = transform.parent.position - attackTarget.transform.parent.position;
            Vector3 origin = attackTarget.transform.parent.right;
            float rightSideAngle = Mathf.Abs(Vector3.Angle(origin, dir));        // Checks if vehicle is 45degrees from right vector of target.
            float leftSideAngle = Mathf.Abs(Vector3.Angle(-origin, dir));       // Checks if vehicle is 45degrees from left vector of target.

            if ((moveTarget.name == "LeftSide" && xDifference < 0 && rightSideAngle < angleOfApproach) ||
                (moveTarget.name == "RightSide" && xDifference > 0 && leftSideAngle < angleOfApproach))
            {
                if (zDifference > 0)
                    newTarget -= new Vector3(0, 0, (r * 1) + 1f);
                else
                    newTarget += new Vector3(0, 0, (r * 1) + 1f);
            }
        }

        Vector3 desired = newTarget - transform.parent.position;

        float d = desired.magnitude;        // Distance is the magnitude of the vector pointing towards target
        desired.Normalize();

        if (arrival)
        {
            // Arriving check (slow down when you get close)
            if (d < SlowDownDistance)
            {
                float m = Map(d, 0, SlowDownDistance, 0, moveSpeed);     // Map magnitude, slow down if close.
                desired *= m;
            }
            else
            {
                desired *= moveSpeed;
            }
        }
        else
        {
            desired *= moveSpeed;
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

    // Adds all the steering behaviours together
    void ApplyForce(Vector3 force)
    {
        acceleration += force;
    }

    // Move away from targets in field of view
    private Vector3 Separate(List<AIController> aiList)
    {
        float desiredSeparation = r * 2;
        Vector3 sum = Vector3.zero;
        int count = 0;
        foreach (AIController ai in aiList)
        {
            float d = Vector3.Distance(transform.parent.position, ai.transform.position);
            if ((d > 0) && (d < desiredSeparation))
            {
                Vector3 diff = transform.parent.position - ai.transform.position;
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
            sum *= moveSpeed;

            Vector3 steer = sum - velocity;

            if (steer.magnitude > maxforce)
            {
                steer.Normalize();
                steer *= moveSpeed;
            }
            steer.y = 0;
            return steer;
        }
        else
        {
            return new Vector3(0, 0, 0);
        }
    }

    private List<AIController> SearchArroundAI()
    {
        // Search around the character and find all AI and players
        Collider[] hitColliders = Physics.OverlapSphere(transform.GetChild(0).position, radius, mask);
        List<AIController> aiList = new List<AIController>();
        int i = 0;
        while (i < hitColliders.Length)
        {
            if (hitColliders[i].tag == "Enemy" &&
                hitColliders[i].transform != transform)
            {
                // Check if the AI or player is inside it's field of view (90deg)
                float angle = AngleBetween(this.transform.forward, hitColliders[i].transform.position);
                float distance = Mathf.Abs(Vector3.Distance(transform.position, hitColliders[i].transform.position));
                //Debug.Log(angle);
                if (angle < 45 && angle > -45 || distance  < 2f)
                    aiList.Add(hitColliders[i].transform.GetComponent<AIController>());
            }
            i++;
        }

        return aiList;
    }

    private Vector3 Avoid(GameObject moveT, GameObject attackT)
    {
        Vector3 desired = Vector3.zero;
        float xDifference = attackT.transform.position.x - transform.parent.position.x;                // Used to determine which side of the target the vehicle is on in the z axis.
        //Debug.Log(xDifference);
        float zDifference = attackT.transform.position.z - transform.parent.position.z;                // Used to determine which side of the target the vehicle is on in the z axis.
        float zDistance = Mathf.Abs(attackT.transform.position.z - transform.parent.position.z);       // Used to determine how far away from the target the vehicle is but only use the z axis.
        //Debug.Log(zDifference);
        Vector3 dir = transform.parent.position - attackT.transform.parent.position;
        Vector3 origin = attackT.transform.parent.right;
        float rightSideAngle = Mathf.Abs(Vector3.Angle(origin, dir));        // Checks if vehicle is 45degrees from right vector of target.
        float leftSideAngle = Mathf.Abs(Vector3.Angle(-origin, dir));       // Checks if vehicle is 45degrees from left vector of target.

        if (((moveT.name == "LeftSide" && xDifference < 0 && rightSideAngle < angleOfApproach) ||
            (moveT.name == "RightSide" && xDifference > 0 && leftSideAngle < angleOfApproach)) &&
            zDistance < r * 2)
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

            float m = Map(value, 0, SlowDownDistance, 0, moveSpeed/2);     // Map magnitude, slow down if close.
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

    static float Map(float value, float min, float max, float minSpeed, float MaxSpeed)
    {
        return (value - min) * (MaxSpeed - minSpeed) / (max - min) + minSpeed;
    }

    static float InverseMap(float value, float min, float max, float minSpeed, float MaxSpeed)
    {
        return (value - max) * (minSpeed - MaxSpeed) / (min - max) + MaxSpeed;
    }

    // Angle between 2 vectors in degrees
    static public float AngleBetween(Vector3 v1, Vector3 v2)
    {
        float dot = Vector3.Dot(v1, v2);
        float theta = Mathf.Acos(dot / (v1.magnitude * v2.magnitude));
        float degrees = theta * Mathf.Rad2Deg;
        return degrees;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Player")
            stop = true;
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.tag == "Player")
            stop = false;
    }
}
