using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MOMovementController : MonoBehaviour
{
    private GameObject scriptEntity;
    public int entityID = 0;

    // Use this for initialization
    void Start()
    {
        //check what entity the script is attached to used for debugging and testing reasons
        if (entityID == 1)
        {
            Debug.Log("Contoller working for player with name " + gameObject.name);
            scriptEntity = GameObject.FindGameObjectWithTag("Player");
        }
        else if (entityID == 2)
        {
            Debug.Log("Controller working for enemy with name " + gameObject.name);
        }
        else
        {
            Debug.Log("Controller not working for " + gameObject.name);
        }
    }

    // method is called when needed from an input script
    public void Move(Vector3 mov, float speed)
    {
        //move the gameobject based on the vars from the input script
        scriptEntity.transform.parent.Translate(mov * speed * Time.deltaTime);
        if (mov.x<0 & mov.z ==0) //left
        {
            scriptEntity.transform.rotation = Quaternion.Euler(gameObject.transform.rotation.x,180*Time.deltaTime,gameObject.transform.rotation.z);
        }
        else if (mov.x> 0 & mov.z == 0)//right
        {
            scriptEntity.transform.rotation = Quaternion.Euler(gameObject.transform.rotation.x, 0 * Time.deltaTime, gameObject.transform.rotation.z);
        }
        else if (mov.z > 0 & mov.x == 0)//up
        {
            scriptEntity.transform.rotation = Quaternion.Euler(gameObject.transform.rotation.x, 270 * Time.deltaTime, gameObject.transform.rotation.z);
        }
        else if (mov.z< 0 & mov.x == 0)//down
        {
            scriptEntity.transform.rotation = Quaternion.Euler(gameObject.transform.rotation.x, 90 * Time.deltaTime, gameObject.transform.rotation.z);
        }
        else if (mov.x <0 & mov.z>0)//up and left
        {
            scriptEntity.transform.rotation = Quaternion.Euler(gameObject.transform.rotation.x, 225 * Time.deltaTime, gameObject.transform.rotation.z);
        }
        else if (mov.x > 0 & mov.z > 0)//up and right
        {
            scriptEntity.transform.rotation = Quaternion.Euler(gameObject.transform.rotation.x, 315 * Time.deltaTime, gameObject.transform.rotation.z);
        }
        else if (mov.x < 0 & mov.z < 0)//down and left
        {
            scriptEntity.transform.rotation = Quaternion.Euler(gameObject.transform.rotation.x, 135 * Time.deltaTime, gameObject.transform.rotation.z);
        }
        else if (mov.x > 0 & mov.z < 0)//down and right
        {
            scriptEntity.transform.rotation = Quaternion.Euler(gameObject.transform.rotation.x, 45 * Time.deltaTime, gameObject.transform.rotation.z);
        }
    }

    //called from input controller 
    public void Jump(float height)
    {
        Debug.Log("game object jumping");
        scriptEntity.GetComponent<Rigidbody>().AddForce(0, height, 0);
        //jump animation and stuff here?
    }

    //called from input controller
    public void Attack ()
    {
        Debug.Log("game object attacking");
        //attack animation and stuff here?
    }

    //called from player's input controller only
    public void Magic()
    {
        Debug.Log("game object using magic");
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
