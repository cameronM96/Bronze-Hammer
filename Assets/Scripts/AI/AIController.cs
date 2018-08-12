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
    public float r = 1.5f;                  // Detection range
    public float maxforce = 0.1f;           // Acceleration rate
    public float SlowDownDistance = 10;     // Start slowing down when getting close to destination;
    int mask = 1 << 11 | 1 << 10;      // https://docs.unity3d.com/Manual/Layers.html, What to detect (AI and player only)
    public float radius = 5;

    public bool arrival;                    // Enable arrival behaviour
    public float seekStrength = 1;          // Stength of seek behaviour
    public float seperateStrength = 2;      // Stength of seperate behaviour
    //[SerializeField] private float jumpHeight = 10.0f; //the height of the AI's jump
    private float hMov;
    private float vMov;
    //private bool jump;
    private bool attack;
    //private bool tooClose;

    public bool sprinting;
    public float attackDistance = 3;
    [HideInInspector] public float meleeAttackDistance;
    private bool stop;

    // Use this for initialization
    protected override void Awake ()
    {
        base.Awake();

        meleeAttackDistance = attackDistance;

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
        if (dist1 < meleeAttackDistance && dist2 < 0.2f)
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

        seperate *= seperateStrength;
        seek *= seekStrength;
        //follow *= followStrength;

        ApplyForce(seperate);
        ApplyForce(seek);
        //ApplyForce(follow);
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
            float d = Vector3.Distance(transform.position, ai.transform.position);
            if ((d > 0) && (d < desiredSeparation))
            {
                Vector3 diff = transform.position - ai.transform.position;
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
                //Debug.Log(angle);
                if (angle < 45 && angle > -45)
                    aiList.Add(hitColliders[i].transform.GetComponent<AIController>());
            }
            i++;
        }

        return aiList;
    }

    static float Map(float value, float min, float max, float minSpeed, float MaxSpeed)
    {
        return (value - min) * (MaxSpeed - minSpeed) / (max - min) + minSpeed;
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
