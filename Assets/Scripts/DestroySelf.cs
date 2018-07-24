using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroySelf : MonoBehaviour {

    [SerializeField] private float waitTimer;

	// Use this for initialization
	void Start ()
    {
        Destroy(this.gameObject, waitTimer);
	}
}
