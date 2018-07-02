using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

public class MOPlayerInputController : MonoBehaviour
{

    public GameObject Player; // the player to be controlled
    public Camera gameCamera; // the games main camera

    private Vector3 gameCameraForward; // the forward direction of the game camera
    private Vector3 moveDirection; // the direction the player will be moved in
    private float moveSpeed; //the speed the player will move
    private float jumpHeight; //the height of the players jump

    // Use this for initialization
    void Start()
    {
        Player = GameObject.FindGameObjectWithTag("Player"); // set the player var to be the player
        gameObject.GetComponent<MOMovementController>().entityID = 1;
        moveSpeed = 5.0f;
        jumpHeight = 500.0f;
    }

    // Update is called once per frame
    void Update()
    {
        //check for the jump button
        if (Input.GetButtonDown("Jump Button"))
        {
            // make player jump
            Debug.Log("jump key pressed");
            Player.GetComponent<MOMovementController>().Jump(jumpHeight);
        }
        //check for attack button
        if (Input.GetButtonDown("Attack Button"))
        {
            //make player attack
            Debug.Log("Attack key pressed");
            Player.GetComponent<MOMovementController>().Attack();
        }

        //check for magic button
        if (Input.GetButtonDown("Magic Button"))
        {
            Debug.Log("magic key pressed");
            Player.GetComponent<MOMovementController>().Magic();
        }
    }

    private void FixedUpdate()
    {
        //get the input for horizontal and vertical axis through unity controls
        float hMov = CrossPlatformInputManager.GetAxis("Horizontal");
        float vMov = CrossPlatformInputManager.GetAxis("Vertical");

        //calculate movement relative to the player
        moveDirection = vMov * Vector3.forward + hMov * Vector3.right;

        //call the method on the controller script sending the required vars
        Player.GetComponent<MOMovementController>().Move(moveDirection, moveSpeed);
    }
}
