using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class AIManager : MonoBehaviour {
    
    private GameObject[] enemies;
    private GameObject[] mounts;
    private GameObject player;
    [SerializeField] private float updateTimer = 2f;
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
            //Debug.Log("Updating Enemies List");
            UpdateList();

            if (enemies.Length > 0)
            {
                //Debug.Log("Updating Enemies targets");
                UpdateTargets();
            }
            timer = 0f;
        }
	}

    public void UpdateList()
    {
        // Find all the enemies in the scene and store references to them
        enemies = GameObject.FindGameObjectsWithTag("Enemy");
        mounts = GameObject.FindGameObjectsWithTag("Mount");
    }

    // Determines which side of the player the AI's should move to.
    private void UpdateTargets()
    {
        //Debug.Log("Updating targets");
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

            // sort the enemies by the sides they are on.
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

            //Debug.Log("Right side enemy count check 1: " + rightSideList.Count);
            //Debug.Log("Left side enemy count check 1: " + leftSideList.Count);

            // Balance out the number of enemies on each side of the player +/- 1.
            while (!(leftSideList.Count < rightSideList.Count + 1 &&
                leftSideList.Count > rightSideList.Count - 1))
            {
                if (leftSideList.Count > rightSideList.Count)
                {
                    int index = rightSideList.Count + 1;
                    if (leftSideList.Count > index)
                    {
                        GameObject enemySwapped = leftSideList[index];
                        leftSideList.RemoveAt(index);
                        rightSideList.Add(enemySwapped);
                        enemy.GetComponent<AIController>().moveTarget = Rightside;
                    }
                    else
                    {
                        break;
                    }
                }
                else
                {
                    int index = leftSideList.Count + 1;
                    if (rightSideList.Count > index)
                    {
                        GameObject enemySwapped = rightSideList[index];
                        rightSideList.RemoveAt(index);
                        leftSideList.Add(enemySwapped);
                        enemy.GetComponent<AIController>().moveTarget = leftside;
                    }
                    else
                    {
                        break;
                    }
                }
            }
        }

        foreach (GameObject mount in mounts)
        {
            if (!mount.GetComponent<MountingController>().isCurrentlyMounted)
            {
                int enemyIndex = 0;
                int closestEnemyIndex = 0;
                float closestEnemyDistance = 500f;
                foreach (GameObject enemy in enemies)
                {
                    float distance;

                    distance = Vector3.Distance(enemy.transform.position, mount.transform.position);

                    if (distance < closestEnemyDistance)
                        closestEnemyIndex = enemyIndex;

                    ++enemyIndex;
                }

                enemies[closestEnemyIndex].GetComponent<AIController>().moveTarget = mount;
            }
        }
    }
}
