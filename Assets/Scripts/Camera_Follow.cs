using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Camera_Follow : MonoBehaviour {

    public Transform target;
    public Transform leftBounds;
    public Transform rightBounds;

    public float smoothDampTime = 0.15f;
    private float smoothDampVelocity = 0f;
    private float currentX;

    private float camWidth, camHeight, levelMinX, levelMaxX;

	// Use this for initialization
	void Start ()
    {
        target = GameObject.FindGameObjectWithTag("Player").transform;

        camHeight = this.gameObject.GetComponent<Camera>().orthographicSize * 2;
        camWidth = camHeight * this.gameObject.GetComponent<Camera>().aspect;

        //float leftBoundWidth = leftBounds.GetComponent<SpriteRenderer>().bounds.size.x / 2;
        //float rightBoundsWidth = rightBounds.GetComponent<SpriteRenderer>().bounds.size.x / 2;

        levelMinX = leftBounds.position.x + (camWidth / 2);
        levelMaxX = rightBounds.position.x - (camWidth / 2);
    }
	
	// Update is called once per frame
	void Update ()
    {
        currentX = transform.position.x;

        if (target)
        {
            float targetX = Mathf.Max(levelMinX, Mathf.Min(levelMaxX, target.position.x));

            float x = Mathf.SmoothDamp(transform.position.x, targetX, ref smoothDampVelocity, smoothDampTime);

            if (currentX < x)
                transform.position = new Vector3(x, transform.position.y, transform.position.z);
        }
	}
}
