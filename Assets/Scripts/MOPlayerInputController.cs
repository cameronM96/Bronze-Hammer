using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

public class MOPlayerInputController : MonoBehaviour
{

    public GameObject Player; // the player to be controlled

    private Transform gameCamera; // the transform of the main game camera
    private Vector3 gameCameraForward; //the forward vector of the main game camera
    private Vector3 moveDirection; // the direction the player will be moved in
    private float moveSpeed; //the speed the player will move
    private float jumpHeight; //the height of the players jump
    private float hMov;
    private float vMov;

    public bool sprinting;

    // Use this for initialization
    void Start()
    {
        //set the game camera
        gameCamera = Camera.main.transform;
        Debug.Log("game camera transform set for " + gameCamera.name);

        // set the player var to be the player
        Player = GameObject.FindGameObjectWithTag("Player");
        Debug.Log("Player found " + Player.name);

        //set the entitiy ID in the movemnt controller script, ID 1 = player
        gameObject.GetComponent<MOMovementController>().entityID = 1;

        //set movemnt values
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
            Player.GetComponent<MOMovementController>().Attack(sprinting);
        }

        //check for magic button
        if (Input.GetButtonDown("Magic Button"))
        {
            Debug.Log("magic key pressed");
            Player.GetComponent<MOMovementController>().Magic();
        }

        if (Input.GetKeyDown("left shift"))
        {
            sprinting = true;
            Debug.Log("Sprinting");
        }
        if (Input.GetKeyUp("left shift"))
        {
            sprinting = false;
            Debug.Log("not sprinting");
        }
    }

    //called once per physics update
    private void FixedUpdate()
    {
        //get the input for horizontal and vertical axis through unity controls
         hMov = CrossPlatformInputManager.GetAxis("Horizontal");
         vMov = CrossPlatformInputManager.GetAxis("Vertical");
        

        //calculate movement relative to the camera
        gameCameraForward = Vector3.Scale(gameCamera.forward, new Vector3(1, 0, 1)).normalized;
        moveDirection = vMov * gameCameraForward + hMov * gameCamera.right;
        //Debug.Log("player move direction is " + moveDirection);

        //call the method on the controller script sending the required vars
        if (sprinting)
        {
            Player.GetComponent<MOMovementController>().Move(moveDirection, moveSpeed*1.5f);
        }
        else if (!sprinting)
        {
            Player.GetComponent<MOMovementController>().Move(moveDirection, moveSpeed);
        }
    }
}
