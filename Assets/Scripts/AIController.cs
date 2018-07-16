using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIController : MOMovementController
{
    public GameObject enemy; // the entity to be controlled
    public GameObject moveTarget;   // The Target to move towards
    public GameObject attackTarget; // The Target to attack

    private MOMovementController m_character;

    private Transform gameCamera; // the transform of the main game camera
    private Vector3 gameCameraForward; //the forward vector of the main game camera
    private Vector3 moveDirection; // the direction the AI will be moved in relative to the camera.
    private Vector3 targetDir;  // Direction of the Target relative to the AI.

    [SerializeField] private float moveSpeed = 5.0f; //the speed the AI will move
    [SerializeField] private float jumpHeight = 500.0f; //the height of the AI's jump
    private float hMov;
    private float vMov;
    private bool jump;
    private bool attack;

    public bool sprinting;

    // Use this for initialization
    protected override void Start ()
    {
        base.Start();

        m_character = GetComponent<MOMovementController>();

        enemy = this.gameObject;

        // This will need to change when there are multiple players
        attackTarget = GameObject.FindGameObjectWithTag("Player");

        //set the game camera
        gameCamera = Camera.main.transform;
    }

    // Update is called once per frame
    protected override void Update ()
    {
        base.Update();

        // Determine which direction to walk
        targetDir = (moveTarget.transform.position - enemy.transform.position).normalized;
        hMov = targetDir.x;
        vMov = targetDir.z;

        //calculate movement relative to the camera
        gameCameraForward = Vector3.Scale(gameCamera.forward, new Vector3(1, 0, 1)).normalized;
        moveDirection = vMov * gameCameraForward + hMov * gameCamera.right;

        // Attack if enemy is close enough and roughly the same z position
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

        // Add a check for jumping
    }

    private void FixedUpdate()
    {
        //call the method on the controller script sending the required vars
        if(attack)
        {
            // Make sure AI is facing the right directions first
            m_character.Move(Vector3.zero, moveSpeed);
            m_character.Attack(false);
        }
        else
        {
            //Debug.Log(moveDirection);
            m_character.Move(moveDirection, moveSpeed);
        }
        
        if (jump)
            m_character.Jump(jumpHeight);
    }
}
