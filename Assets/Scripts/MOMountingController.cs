using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MOMountingController : MonoBehaviour
{
    private GameObject mountedCharacter;

    public bool isCurrentlyMounted;

    // Use this for initialization
    void Start()
    {
        isCurrentlyMounted = false;
        //gameObject.transform.parent = null;
    }

    private void Update()
    {

    }

    private void OnCollisionEnter(Collision collision)
    {
        //check so that you can have 2 mounts ride each other. 
        if (collision.gameObject.tag != "Mount")
        {
            //check not already mounted by something
            if (!isCurrentlyMounted)
            {
                Debug.Log(gameObject.name+" Being mounted by " + collision.collider);
                //set mounted character to the object collided with
                mountedCharacter = collision.collider.gameObject;

                //set collided with game object as the mounts parent
                gameObject.transform.parent = mountedCharacter.transform;

                //adjust the bool
                isCurrentlyMounted = true;

                //adjust mounts position to appear under character as if riding
                gameObject.transform.position = new Vector3(mountedCharacter.transform.position.x, 0.5f, mountedCharacter.transform.position.z);
                gameObject.transform.Rotate(0, mountedCharacter.transform.rotation.w, 0);
            }
        }
    }
}