using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AIBossController : MOMovementController
{
    public GameObject enemy; // the entity to be controlled
    public GameObject moveTarget;   // The Target to move towards
    public GameObject attackTarget; // The Target to attack
    public RuntimeAnimatorController form1Animations;//the contoller for the first form of the boss
    public RuntimeAnimatorController form2Animations;//the controller for the second form of the boss

    private MOMovementController m_character;
    // private NavMeshAgent agent;

    private float distanceToPlayer; //how far the boss is from the player

    private float teleportDistance = 10; //minimum distance for the boss to teleport away from the player
    private float teleportTime; //cooldown of the teleport ability
    //locations the boss can teleport to
    public GameObject teleportLocation1;
    public GameObject teleportLocation2;
    public GameObject teleportLocation3;
    public GameObject teleportLocation4;
    public GameObject teleportLocation5;

    private float attack1Cooldown;//cooldown of the basic lightning wave attack

    private float attack2Cooldown; //cooldown of the powerful magic attack

    private float attack3Cooldown; //cooldown of the knockback sword attack
    private float attack3Distance = 10; //the minimum distance the play muct be away to use attack 3

    private bool phase2; //bool of if the boss fight is in phase 1 or 2 

    private Transform gameCamera; // the transform of the main game camera
    private Vector3 gameCameraForward; //the forward vector of the main game camera
    private Vector3 moveDirection; // the direction the AI will be moved in relative to the camera.
    private Vector3 targetDir;  // Direction of the Target relative to the AI.

    [SerializeField] private float moveSpeed = 5.0f; //the speed the AI will move
    //[SerializeField] private float jumpHeight = 500.0f; //the height of the AI's jump
    private float hMov;
    private float vMov;
    //private bool jump;
    private bool attack;
    //private bool tooClose;

    public bool sprinting;

    // Use this for initialization
    protected override void Awake()
    {
        gameObject.GetComponent<MOMovementController>().entityID = 2;
        base.Awake();

        GetComponent<Animator>().runtimeAnimatorController = form1Animations;
        m_character = GetComponent<MOMovementController>();

        phase2 = false;

        // agent = GetComponentInParent<NavMeshAgent>();

        enemy = this.gameObject;

        //set move target as boss doesn't need to surround, will need to change with multiple players
        moveTarget = GameObject.FindGameObjectWithTag("Player");
        // This will need to change when there are multiple players
        attackTarget = GameObject.FindGameObjectWithTag("Player");

        //set the game camera
        gameCamera = Camera.main.transform;
    }

    // Update is called once per frame
    protected override void Update()
    {

        base.Update();

        if (teleportTime < 0.0f)
        {
            teleportTime -= Time.deltaTime;
        }
        if (attack1Cooldown < 0.0f)
        {
            attack1Cooldown -= Time.deltaTime;
        }
        if (attack2Cooldown < 0.0f)
        {
            attack2Cooldown -= Time.deltaTime;
        }

        //agent.destination = moveTarget.transform.position;

        // Determine which direction to walk
        if (moveTarget != null)
        {
            targetDir = (moveTarget.transform.position - enemy.transform.position).normalized;
            hMov = targetDir.x;
            vMov = targetDir.z;
        }
        else
        {
            hMov = 0;
            vMov = 0;
        }

        //calculate movement relative to the camera
        gameCameraForward = Vector3.Scale(gameCamera.forward, new Vector3(1, 0, 1)).normalized;
        moveDirection = vMov * gameCameraForward + hMov * gameCamera.right;

        // add steering for when there are other AI in the way

        // Attack if enemy is close enough and roughly the same z position AND facing the player
        if ((attackTarget.transform.position - enemy.transform.position).magnitude < 2 &&
            (attackTarget.transform.position.z - enemy.transform.position.z < 0.2 &&
            attackTarget.transform.position.z - enemy.transform.position.z > -0.2))
        {
            attack = true;
        }
        else
        {
            attack = false;
        }

        distanceToPlayer = Vector3.Distance(moveTarget.transform.position, gameObject.transform.position);

        // Add a check for jumping
    }

    private void FixedUpdate()
    {
        //call the method on the controller script sending the required vars
        if (attack && attack1Cooldown<=0.0f)
        {
            // Make sure AI is facing the right directions first
            this.transform.LookAt(attackTarget.transform);
            m_character.Move(Vector3.zero, moveSpeed);
            m_character.BossAttack(1);
            attack1Cooldown = 2.0f;
        }
        else if (attack && attack2Cooldown <=0.0f)
        {
            // Make sure AI is facing the right directions first
            this.transform.LookAt(attackTarget.transform);
            m_character.Move(Vector3.zero, moveSpeed);
            m_character.BossAttack(2);
            attack2Cooldown = 10.0f;
        }
        else if (attack && attack3Cooldown<=0.0f &&attack3Distance<=distanceToPlayer)
        {
            // Make sure AI is facing the right directions first
            this.transform.LookAt(attackTarget.transform);
            m_character.Move(Vector3.zero, moveSpeed);
            m_character.BossAttack(3);
            attack3Cooldown = 5.0f;
        }
        else
        {
            // Prevent movement if an AI is too close
            //if (!tooClose)
            m_character.Move(moveDirection, moveSpeed);
        }

        //use teleport ability
        //if (distanceToPlayer <= teleportDistance && teleportTime <= 0.0f)
        //{
        //    int teleportLocationSelector = Random.Range(1, 5);
        //    if (teleportLocationSelector == 1)
        //    {
        //        gameObject.transform.SetPositionAndRotation(teleportLocation1.transform.position, gameObject.transform.rotation);
        //    }
        //    else if (teleportLocationSelector == 2)
        //    {
        //        gameObject.transform.SetPositionAndRotation(teleportLocation2.transform.position, gameObject.transform.rotation);

        //    }
        //    else if (teleportLocationSelector == 3)
        //    {
        //        gameObject.transform.SetPositionAndRotation(teleportLocation3.transform.position, gameObject.transform.rotation);

        //    }
        //    else if (teleportLocationSelector == 4)
        //    {
        //        gameObject.transform.SetPositionAndRotation(teleportLocation4.transform.position, gameObject.transform.rotation);

        //    }
        //    else if (teleportLocationSelector == 5)
        //    {
        //        gameObject.transform.SetPositionAndRotation(teleportLocation5.transform.position, gameObject.transform.rotation);

        //    }
        //    teleportTime = 10.0f;
        //}
    }
}
