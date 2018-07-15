using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MOMountingController : MonoBehaviour {
    private GameObject mount;
    private GameObject mountedCharacter;
    private int mountedEntityID = 3;

	// Use this for initialization
	void Start ()
    {
        mount = gameObject;
	}
	
	// Update is called once per frame
	void Update ()
    {
		
	}

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("Being mounted by " + collision.collider);
        mountedCharacter = collision.collider.gameObject;
        mountedCharacter.GetComponent<MOMovementController>().enabled = false;
        mount.GetComponent<MOMovementController>().enabled = true;
    }
}
