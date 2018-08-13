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
    private Health m_health;

    private float distanceToPlayer; //how far the boss is from the player

    private float teleportDistance = 10; //minimum distance for the boss to teleport away from the player
    private float teleportTime; //cooldown of the teleport ability
    //locations the boss can teleport to
    public GameObject teleportLocation1;
    public GameObject teleportLocation2;
    public GameObject teleportLocation3;
    public GameObject teleportLocation4;
    public GameObject teleportLocation5;

    private float attackTimeLimiter;//limits the time between boss attacks
    private int attackPicker;//randomly picks the attack the boss will perform

    [HideInInspector] public float meleeAttackDistance;

    private bool phase2; //bool of if the boss fight is in phase 1 or 2 

    private Transform gameCamera; // the transform of the main game camera
    private Vector3 gameCameraForward; //the forward vector of the main game camera
    private Vector3 moveDirection; // the direction the AI will be moved in relative to the camera.
    private Vector3 targetDir;  // Direction of the Target relative to the AI.

    [SerializeField] private float moveSpeed = 5.0f; //the speed the AI will move
    //[SerializeField] private float jumpHeight = 25.0f; //the height of the AI's jump
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

        //GetComponent<Animator>().runtimeAnimatorController = form1Animations;
        m_character = GetComponent<MOMovementController>();
        Debug.Log("m_character = " + m_character);
        m_health = GetComponent<Health>();

        phase2 = false;
        attackTimeLimiter = 0.0f;

        // agent = GetComponentInParent<NavMeshAgent>();

        enemy = this.gameObject;

        meleeAttackDistance = 6;

        //set move target as boss doesn't need to surround, will need to change with multiple players
        moveTarget = GameObject.FindGameObjectWithTag("Player");
        // This will need to change when there are multiple players
        attackTarget = GameObject.FindGameObjectWithTag("Player");
        StartCoroutine(WaitTillEndOfFrame());

        //set the game camera
        gameCamera = Camera.main.transform;
    }

    IEnumerator WaitTillEndOfFrame ()
    {
        yield return new WaitForEndOfFrame();
        //set move target as boss doesn't need to surround, will need to change with multiple players
        moveTarget = GameObject.FindGameObjectWithTag("Player");
        // This will need to change when there are multiple players
        attackTarget = GameObject.FindGameObjectWithTag("Player");
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();

        if (teleportTime >= 0.0f)
        {
            teleportTime -= Time.deltaTime;
        }

        if (attackTimeLimiter >= 0.0f)
        {
            attackTimeLimiter -= Time.deltaTime;
           // Debug.Log("attack time limiter = " + attackTimeLimiter);
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

        distanceToPlayer = Vector3.Distance(moveTarget.transform.position, gameObject.transform.position);

        // Add a check for jumping
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();

        if (attack)
        {
            if (attackTimeLimiter <= 0)
            {
                Debug.Log("Calling attack player");
                attackPicker = Random.Range(1, 4);
                Debug.Log("Randomised to " + attackPicker);
                AttackPlayer(attackPicker, phase2);
            }
        }
        else
        {
            // Prevent movement if an AI is too close
            //if (!tooClose)
            moveDirection *= moveSpeed;
            m_character.Move(moveDirection);
        }

        if (m_health.bossHealth <= 375&&phase2 == false)
        {
            phase2 = true;
        }
        //use teleport ability
        if (1 == 2)//distanceToPlayer <= teleportDistance && teleportTime <= 0.0f)
        {
            int teleportLocationSelector = Random.Range(1, 5);
            if (teleportLocationSelector == 1)
            {
                gameObject.transform.SetPositionAndRotation(teleportLocation1.transform.position, gameObject.transform.rotation);
            }
            else if (teleportLocationSelector == 2)
            {
                gameObject.transform.SetPositionAndRotation(teleportLocation2.transform.position, gameObject.transform.rotation);

            }
            else if (teleportLocationSelector == 3)
            {
                gameObject.transform.SetPositionAndRotation(teleportLocation3.transform.position, gameObject.transform.rotation);

            }
            else if (teleportLocationSelector == 4)
            {
                gameObject.transform.SetPositionAndRotation(teleportLocation4.transform.position, gameObject.transform.rotation);

            }
            else if (teleportLocationSelector == 5)
            {
                gameObject.transform.SetPositionAndRotation(teleportLocation5.transform.position, gameObject.transform.rotation);

            }
            teleportTime = 10.0f;
        }
    }

    private void AttackPlayer(int attackNumber, bool isPhaseTwo)
    {
        Debug.Log("Attack player called");

        if (!isPhaseTwo)
        {
            attackTimeLimiter = 4.0f; //sets a 4 second delay for the boss before he can attack again (adjust as needed)

            //call the method on the controller script sending the required vars
            if (attackNumber == 1)
            {
                Debug.Log("Called attack 1");
                // Make sure AI is facing the right directions first
                this.transform.LookAt(attackTarget.transform);
                m_character.Move(Vector3.zero);
                m_character.BossAttack(1);

            }
            else if (attackNumber == 2)
            {
                Debug.Log("Called attack 2");
                // Make sure AI is facing the right directions first
                this.transform.LookAt(attackTarget.transform);
                m_character.Move(Vector3.zero);
                m_character.BossAttack(2);

            }
            else if (attackNumber == 3)
            {
                Debug.Log("Called attack 3");
                // Make sure AI is facing the right directions first
                this.transform.LookAt(attackTarget.transform);
                m_character.Move(Vector3.zero);
                m_character.BossAttack(3);
            }
        }
        else if (isPhaseTwo)
        {
            attackTimeLimiter = 3.0f; //sets a 4 second delay for the boss before he can attack again (adjust as needed)

            // Make sure AI is facing the right directions first
            this.transform.LookAt(attackTarget.transform);
            m_character.Move(Vector3.zero);
            m_character.BossAttack(3);
        }
    }
}