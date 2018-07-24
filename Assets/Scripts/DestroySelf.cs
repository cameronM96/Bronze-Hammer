using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroySelf : MonoBehaviour {

    public float waitTimer;

	// Use this for initialization
	void Awake ()
    {
        Destroy(this.gameObject, waitTimer);
	}
}
