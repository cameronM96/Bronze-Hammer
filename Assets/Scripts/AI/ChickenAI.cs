using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ChickenAI : MonoBehaviour {

    private Vector3 wanderCentre;
    [SerializeField] private GameObject manaPot;
    [SerializeField] private float pauseTimer = 3;
    public float retreatTimer = 20f;
    private float time = 0;
    public float timesHit = 0;
    [SerializeField] private float maxTimesHit = 3;
    private float maxX = 0;
    private float minX = 0;
    private float maxZ = 0;
    private float minZ = 0;
    private NavMeshAgent agent;
    private Rigidbody m_Rigidbody;
    private int mask = 1 << 9;            // https://docs.unity3d.com/Manual/Layers.html
    private bool waiting = false;
    private bool retreating = false;
    
	// Use this for initialization
	void Awake ()
    {
        time = 0;
        agent = GetComponent<NavMeshAgent>();
        wanderCentre = CameraToGround();
        GetBoundaries(wanderCentre);
        StartCoroutine(InitialiseWaitTimer());
        m_Rigidbody = GetComponent<Rigidbody>();
	}

    IEnumerator InitialiseWaitTimer()
    {
        yield return new WaitForEndOfFrame();
        NewGoal(wanderCentre);
    }

    // Update is called once per frame
    void Update ()
    {
        time += Time.deltaTime;

        if (timesHit < maxTimesHit && time < retreatTimer)
        {
            // If destination was reached, wait for a while then find new destination.
            if (agent.pathStatus == NavMeshPathStatus.PathComplete)
            {
                float distance = Vector3.Distance(this.transform.position,agent.destination);

                if (distance < 1 && distance > - 1)
                {
                    if (!waiting)
                    {
                        StartCoroutine(PauseTimer(pauseTimer));
                    }
                }
            }
        }
        else
        {
            // Run away if time existed for too long or hit a bunch.
            if (!retreating)
            {
                wanderCentre = CameraToGround();
                GetBoundaries(wanderCentre);
                Retreat(wanderCentre);
            }
        }
    }

    // Get the boundaries for where chicken can move to (In cameraview and game area)
    public void GetBoundaries (Vector3 origin)
    {
        RaycastHit hit;

        // X values
        if (Physics.Raycast(origin, Vector3.right, out hit, 20f, mask))
        {
            maxX = hit.transform.position.x - 2;
        }
        else
        {
            maxX = origin.x - 15f;
        }

        // MixX
        if (Physics.Raycast(origin, Vector3.left, out hit, 20f, mask))
        {
            minX = hit.transform.position.x + 2;
        }
        else
        {
            minX = origin.x + 15f;
        }

        // Z values
        maxZ = Camera.main.transform.GetChild(2).transform.position.z;
        minZ = Camera.main.transform.GetChild(3).transform.position.z;
    }

    // Find new place to walk too (Wander)
    public void NewGoal (Vector3 origin)
    {
        float x = Random.Range(minX, maxX);
        float y = origin.y;
        float z = Random.Range(minZ, maxZ); ;
        Vector3 newGoal = new Vector3(x, y, z);

        NavMeshHit hit;
        if (NavMesh.SamplePosition(newGoal, out hit, 100.0f, NavMesh.AllAreas))
        {
            newGoal = hit.position;
        }

        Debug.Log(newGoal);
        agent.SetDestination(newGoal);
    }

    // Run away (run off screen)
    private void Retreat (Vector3 origin)
    {
        Vector3 retreatPoint = Camera.main.transform.GetChild(4).transform.position;

        NavMeshHit hit;
        if (NavMesh.SamplePosition(retreatPoint, out hit, 1.0f, NavMesh.AllAreas))
        {
            retreatPoint = hit.position;
        }

        agent.SetDestination(retreatPoint);
        agent.speed *= 2;
        retreating = true;
        Destroy(this.gameObject, 10);
    }

    // Wait for a while then find new destination
    IEnumerator PauseTimer (float waitTimer)
    {
        waiting = true;
        agent.isStopped = true;
        yield return new WaitForSeconds(waitTimer);
        wanderCentre = CameraToGround();
        GetBoundaries(wanderCentre);
        NewGoal(wanderCentre);
        waiting = false;
        agent.isStopped = false;
    }

    // Finds the centre point of the screen
    public Vector3 CameraToGround()
    {
        // Raycast from camera down to the ground (Finds the centre of the screen)
        RaycastHit hit;
        //Ray forwardRay = new Ray(gameCamera.transform.position, transform.forward);

        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, Mathf.Infinity, 1))
        {
            return hit.transform.position;
        }
        else
        {
            return this.transform.position;
        }
    }

    // Drop pots when hit
    public void KickChicken()
    {
        m_Rigidbody.AddForce(new Vector3(m_Rigidbody.velocity.x, 300, m_Rigidbody.velocity.z));
        if (timesHit < maxTimesHit)
        {
            GameObject potion = Instantiate(manaPot);
            potion.GetComponent<Rigidbody>().isKinematic = true;
            potion.transform.position = new Vector3(this.transform.position.x, this.transform.position.y + 2, this.transform.position.z);
            potion.GetComponent<Rigidbody>().isKinematic = false;
            potion.transform.SetParent(null, false);
            potion.GetComponent<Rigidbody>().AddForce(new Vector3(Random.Range(10, -10), 300, Random.Range(10, -10)));
        }
        else
        {
            Death();
        }
        ++timesHit;
        GetBoundaries(wanderCentre);
        NewGoal(CameraToGround());
    }

    public void Death()
    {
        this.gameObject.layer = 15;
    }

    IEnumerator FallThroughFloor(float waittimer)
    {
        yield return new WaitForSeconds(waittimer);
        GetComponentInChildren<Collider>().enabled = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "ChickenKiller")
        {
            Destroy(this.gameObject);
        }
    }
}
