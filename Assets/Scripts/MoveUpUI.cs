using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveUpUI : MonoBehaviour {

    public float speed;
    public float deathTimer;

    private void Awake()
    {
        Destroy(this.gameObject, deathTimer);
    }

    // Update is called once per frame
    void Update ()
    {
        transform.Translate(Vector3.up * speed);
	}
}
