using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class AIManager : MonoBehaviour {
    
    private GameObject[] enemies;
    private GameObject player;
    [SerializeField] private float updateTimer = 0.5f;
    private float timer = 1f;

	// Use this for initialization
	void Start ()
    {
        UpdateList();
        player = GameObject.FindGameObjectWithTag("Player");
	}
	
	// Update is called once per frame
	void Update ()
    {
        timer += Time.deltaTime;

        // Update every few seconds rather than every frame
        if (timer > updateTimer)
        {
            UpdateList();

            if (enemies.Count() <= 0)
            UpdateTargets();
            timer = 0f;
        }
	}

    public void UpdateList()
    {
        // Find all the enemies in the scene and store references to them
        enemies = GameObject.FindGameObjectsWithTag("Enemy");
    }

    // Determines which side of the player the AI's should move to.
    private void UpdateTargets()
    {
        // If increased to 2 players this will need to be overhauled.
        // Order the AI based on distance from the player
        enemies = enemies.OrderBy(enemy => Vector3.Distance(player.transform.position, enemy.transform.position)).ToArray();

        List<GameObject> leftSideList = new List<GameObject>();
        List<GameObject> rightSideList = new List<GameObject>();

        // Get the points the AI want to move to.
        GameObject leftside = player.transform.parent.Find("LeftSide").gameObject;
        GameObject Rightside = player.transform.parent.Find("RightSide").gameObject;

        // Determine which side of the player the AI should move to.
        foreach (GameObject enemy in enemies)
        {
            // Find what side of the player the AI is on (positive = right, negative = left)
            float side = player.transform.position.x - enemy.transform.position.x;
            
            if (rightSideList.Count == leftSideList.Count)      // if the lists are even move the AI to which ever side is closest.
            {
                if (side > 0)
                {
                    rightSideList.Add(enemy);
                    enemy.GetComponent<AIController>().moveTarget = Rightside;
                }
                else
                {
                    leftSideList.Add(enemy);
                    enemy.GetComponent<AIController>().moveTarget = leftside;
                }
            }
            else if (rightSideList.Count > leftSideList.Count)  // if lists aren't even move AI to the side with less units on it.
            {
                leftSideList.Add(enemy);
                enemy.GetComponent<AIController>().moveTarget = leftside;
            }
            else
            {
                rightSideList.Add(enemy);
                enemy.GetComponent<AIController>().moveTarget = Rightside;
            }
        }
    }
}
