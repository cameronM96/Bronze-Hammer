using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MOMovementController : MonoBehaviour
{
    private GameObject scriptEntity;
    private float entityTurnSpeed = 10;

    public int entityID = 0;
    public Vector3 entityRotation;

    // Use this for initialization
    void Start()
    {
        //check what entity the script is attached to used for debugging and testing reasons and possible mounting controls
        if (entityID == 1) // ID 1 = PLAYER
        {
            Debug.Log("Contoller working for player with name " + gameObject.name);
            scriptEntity = GameObject.FindGameObjectWithTag("Player");
        }
        else if (entityID == 2) // ID 2 = ENEMY
        {
            Debug.Log("Controller working for enemy with name " + gameObject.name);
        }
        else // if no ID matched return this error with what entity it is not working for
        {
            Debug.Log("Controller not working for " + gameObject.name);
        }
    }

    private void Update()
    {
        //scriptEntity.transform.rotation = Quaternion.Euler(entityRotation*entityTurnSpeed*Time.deltaTime);
        scriptEntity.transform.rotation = Quaternion.Lerp(scriptEntity.transform.rotation, Quaternion.Euler(entityRotation), entityTurnSpeed * Time.deltaTime);
    }

    // method is called when needed from an input script
    public void Move(Vector3 mov, float speed)
    {
        //move the gameobject based on the vars from the input script
        scriptEntity.transform.parent.Translate(mov * speed * Time.deltaTime);

        if (mov.x < 0 & mov.z == 0) //left
        {
            entityRotation.Set(0, 270, 0);
        }
        else if (mov.x > 0 & mov.z == 0)//right
        {
            entityRotation.Set(0, 90, 0);
        }
        else if (mov.z > 0 & mov.x == 0)//up
        {
            entityRotation.Set(0, 0, 0);
        }
        else if (mov.z < 0 & mov.x == 0)//down
        {
            entityRotation.Set(0, 180, 0);
        }
        else if (mov.x < 0 & mov.z > 0)//up and left
        {
            entityRotation.Set(0, 315, 0);
        }
        else if (mov.x > 0 & mov.z > 0)//up and right
        {
            entityRotation.Set(0, 45, 0);
        }
        else if (mov.x < 0 & mov.z < 0)//down and left
        {
            entityRotation.Set(0, 225, 0);
        }
        else if (mov.x > 0 & mov.z < 0)//down and right
        {
            entityRotation.Set(0, 135, 0);
        }
    }

    //called from input controller 
    public void Jump(float height)
    {
        Debug.Log(scriptEntity.name + " jumping");
        scriptEntity.GetComponent<Rigidbody>().AddForce(0, height, 0);
        //jump animation and stuff here?
    }

    //called from input controller
    public void Attack()
    {
        Debug.Log(scriptEntity.name + " attacking");
        //attack animation and stuff here?
    }

    //called from player's input controller only
    public void Magic()
    {
        Debug.Log(scriptEntity.name + " using magic");
        //check for different players 
        /*
        if (gameObject.name == "")
        {
        //use the magic for player 1
        }
        else if (gameObject.name == "")
        {
        //use the magic for player 2
        }
        else if (gameObject.name == "")
        {
        use the magic for player 3
        }
        */
    }
}
