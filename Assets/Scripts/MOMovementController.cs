using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MOMovementController : MonoBehaviour
{
    private GameObject scriptEntity;
    private GameObject attackTrigger;
    private float entityTurnSpeed = 10.0f;
    private float timerA = 0.0f;
    private float timerC = 0.0f;
    private float timerJ = 0.0f;
    private int attackCounter;

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
            attackTrigger = GameObject.Find("Player Attack Trigger");

        }
        else if (entityID == 2) // ID 2 = ENEMY
        {
            Debug.Log("Controller working for enemy with name " + gameObject.name);
        }
        else // if no ID matched return this error with what entity it is not working for
        {
            Debug.Log("Controller not working for " + gameObject.name);
        }
        attackTrigger.SetActive(false);
        attackCounter = 0;
    }

    private void Update()
    {
        //scriptEntity.transform.rotation = Quaternion.Euler(entityRotation*entityTurnSpeed*Time.deltaTime);
        scriptEntity.transform.rotation = Quaternion.Lerp(scriptEntity.transform.rotation, Quaternion.Euler(entityRotation), entityTurnSpeed * Time.deltaTime);
        if (timerA > 0)
        {
            timerA -= Time.deltaTime;
        }
        else if (timerA <= 0 & attackTrigger.activeSelf == true)
        {
            timerA = 0;
            attackTrigger.SetActive(false);
            Debug.Log("attack trigger for " + scriptEntity + " is active = " + attackTrigger.activeSelf);
        }

        if (timerC>0)
        {
            timerC -= Time.deltaTime;
        }
        if (timerJ > 0)
        {
            timerJ -= Time.deltaTime;
            //  Debug.Log("timerJ = " + timerJ);
        }
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
        if (timerJ <= 0.0f)
        {
            Debug.Log(scriptEntity.name + " jumping");
            scriptEntity.GetComponent<Rigidbody>().AddForce(0, height, 0);
            timerJ = 2.0f;
        }
        //jump animation and stuff here?
    }

    //called from input controller
    public void Attack(bool sprinting)
    {
        Debug.Log(scriptEntity.name + " attacking");
        if (timerA == 0)
        {
            if (timerJ>0)
            {
                Debug.Log("jump attack used");
                attackCounter = 0;
                attackTrigger.SetActive(true);
                Debug.Log("attack trigger for " + scriptEntity + " is active = " + attackTrigger.activeSelf);
                timerA = 1.0f;
                //put jump attack here
            }
            else if (attackCounter >= 3)
            {
                Debug.Log("3 attack combo used");
                attackCounter = 0;
                attackTrigger.SetActive(true);
                Debug.Log("attack trigger for " + scriptEntity + " is active = " + attackTrigger.activeSelf);
                timerA = 0.25f;
                //put combo here
            }
            else if (sprinting)
            {
                Debug.Log("charge attack used");
                attackTrigger.SetActive(true);
                Debug.Log("attack trigger for " + scriptEntity + " is active = " + attackTrigger.activeSelf);   
            }
            else
            {
                attackTrigger.SetActive(true);
                Debug.Log("attack trigger for " + scriptEntity + " is active = " + attackTrigger.activeSelf);
                timerA = 0.25f;
                //attack animation and stuff here?
                attackCounter += 1;
                if (timerC==0)
                {
                    timerC = 1;
                }
            }

        }
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
