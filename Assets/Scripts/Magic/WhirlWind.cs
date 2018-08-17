using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WhirlWind : MonoBehaviour {


    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Enemy")
        {
            other.transform.parent.GetComponent<Rigidbody>().velocity = Vector3.up * 30;
        }
    }
}
