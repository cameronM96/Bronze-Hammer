using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Camera_Follow : MonoBehaviour {

    public Transform target;
    public Transform leftBounds;
    public Transform rightBounds;
    public Transform[] boundaryList;

    public float smoothDampTime = 0.15f;
    private float smoothDampVelocity = 0f;
    private float currentX;
    private int currentBoundaryIndex = 0;
    [SerializeField] private float spawnBuffer = 2;

    private float camWidth, camHeight, levelMinX, levelMaxX;

	// Use this for initialization
	void Start ()
    {
        camHeight = this.gameObject.GetComponent<Camera>().orthographicSize * 2;
        camWidth = camHeight * this.gameObject.GetComponent<Camera>().aspect;

        float leftBoundWidth = 0;
        float rightBoundsWidth = 0;

        if (leftBounds.GetComponent<Renderer>() != null)
            leftBoundWidth = leftBounds.GetComponent<Renderer>().bounds.size.x / 2;

        if (rightBounds.GetComponent<Renderer>() != null)
            rightBoundsWidth = rightBounds.GetComponent<Renderer>().bounds.size.x / 2;

        leftBounds = GameObject.FindGameObjectWithTag("LeftBoundary").transform;
        boundaryList = new Transform[(GameObject.FindGameObjectWithTag("Encounter Manager").transform.childCount + 1)];

        int i = 0;
        foreach (Transform child in GameObject.FindGameObjectWithTag("Encounter Manager").transform)
        {
            boundaryList[i] = child;
            i++;
        }
        boundaryList[i] = GameObject.FindGameObjectWithTag("RightBoundary").transform; ;

        levelMinX = leftBounds.position.x + leftBoundWidth + (camWidth / 2);
        levelMaxX = (rightBounds.position.x + rightBoundsWidth) - (camWidth / 2);

        // Move Camera at start of game
        float targetX = Mathf.Max(levelMinX, Mathf.Min(levelMaxX, target.position.x));
        transform.position = new Vector3(targetX, transform.position.y, transform.position.z);

        //SetSpawnPoints();
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

    public void NewBoundary()
    {
        // Set new right boundary
        ++currentBoundaryIndex;
        if (boundaryList[currentBoundaryIndex] != null)
        {
            rightBounds = boundaryList[currentBoundaryIndex];
        }
        float rightBoundsWidth = 0;

        if (rightBounds.GetComponent<Renderer>() != null)
            rightBoundsWidth = rightBounds.GetComponent<Renderer>().bounds.size.x / 2;

        levelMaxX = (rightBounds.position.x + rightBoundsWidth) - (camWidth / 2);

        //Debug.Log("Changing Camera Max to: " + rightBounds);
    }

    private void SetSpawnPoints ()
    {
        bool switchbool = true;
        foreach (Transform spawner in transform)
        {
            if (switchbool)
            {
                spawner.position = new Vector3(-(camWidth / 2) - spawnBuffer, spawner.position.y, spawner.position.z);
            }
            else
            {
                spawner.position = new Vector3((camWidth / 2) + spawnBuffer, spawner.position.y, spawner.position.z);
            }
            switchbool = !switchbool;
        }
    }
}
