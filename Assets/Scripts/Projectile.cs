using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour {

    public int attackDamage = 10;
    public bool playerRider;
    [SerializeField] private float speed = 10;
	
	// Update is called once per frame
	void Update ()
    {
        transform.Translate(Vector3.right * speed * Time.deltaTime);
	}

    private void OnTriggerEnter(Collider other)
    {
        // Only hit the player if someone not a player fired the projectile
        if (other.tag == "Player" && !playerRider)
        {
            // Determine which direction to send the target in when hit
            float dir = 0;
            if (other.transform.position.x > transform.position.x)
            {
                dir = 1;
            }
            else
            {
                dir = -1;
            }

            // Deal damage and knock back target
            other.gameObject.GetComponent<PlayerHealth>().TakeDamage(attackDamage, true, dir);
            DestroySelf();
        }

        // Only hit enemies if the player fired the projectile
        if (other.tag == "Enemy" && playerRider)
        {
            // Determine which direction to send the target in when hit
            float dir = 0;
            if (other.transform.position.x > transform.position.x)
            {
                dir = 1;
            }
            else
            {
                dir = -1;
            }

            // Deal damage and knock back target
            other.gameObject.GetComponent<Health>().TakeDamage(attackDamage, true, dir);
            DestroySelf();
        }
    }

    private void DestroySelf()
    {
        Destroy(this.gameObject, 1);
    }
}
