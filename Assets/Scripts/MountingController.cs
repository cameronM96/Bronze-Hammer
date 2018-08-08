using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MountingController : MonoBehaviour
{
    public GameObject mountedCharacter;
    private Rigidbody rb;
    public Animator m_Anim;
    [SerializeField] private bool ranged;
    [SerializeField] private GameObject attack;
    [SerializeField] private GameObject attackPoint;
    [SerializeField] private float attackRange = 8;

    public bool isCurrentlyMounted;
    [SerializeField] private float mountDelay = 1f;

    // Use this for initialization
    void Awake()
    {
        isCurrentlyMounted = false;
        m_Anim.GetComponent<Animator>();
        //gameObject.transform.parent = null;
    }

    private void Update()
    {
        if (mountedCharacter != null)
        {
            // Animations
            Vector3 groundVelocity = rb.velocity;
            groundVelocity.y = 0f;

            m_Anim.SetFloat("velocity", groundVelocity.magnitude);
        }
        else
        {
            // Idle Animation
            m_Anim.SetFloat("velocity", 0);
            isCurrentlyMounted = false;
        }
    }

    public void Attack()
    {
        if (mountedCharacter != null)
        {
            m_Anim.SetBool("attack", true);
            StartCoroutine(AttackDelay(mountedCharacter));
        }
    }

    IEnumerator AttackDelay(GameObject rider)
    {
        float waitTimer = 0.7f;
        if (ranged)
            waitTimer = 1.04f;

        yield return new WaitForSeconds(waitTimer);
        if (ranged)
        {
            // Long ranged attack
            GameObject projectile = Instantiate(attack, attackPoint.transform);
            projectile.transform.parent = null;
            if (rider.tag == "Player")
            {
                projectile.GetComponent<Projectile>().playerRider = true;
            }
            else
            {
                projectile.GetComponent<Projectile>().playerRider = false;
            }
        }
        else
        {
            // Short ranged attack
            if (rider.tag == "Player")
            {
                attack.SetActive(true);
                attack.GetComponent<MountAttack>().playerAttack = true;
            }
            else
            {
                attack.SetActive(true);
                attack.GetComponent<MountAttack>().playerAttack = false;
            }
        }
    }

    public void AttackOff()
    {
        // Turns off the attack animation
        m_Anim.SetBool("attack", false);
        if (!ranged)
        {
            attack.SetActive(false);
        }
    }

    public void UnMounted()
    {
        // Unmounts the rider from the mount
        mountedCharacter = null;
        rb = null;
        isCurrentlyMounted = false;
        this.transform.parent.parent = null;
        AttackOff();
        StartCoroutine(MountDelay());
    }

    IEnumerator MountDelay()
    {
        yield return new WaitForSeconds(mountDelay);
        GetComponent<Collider>().enabled = true;
    }

    private void MountCharacter (Collider rider)
    {
        //Debug.Log("Collided with character, mounting");
        //check not already mounted by something
        if (!isCurrentlyMounted && !rider.GetComponent<MOMovementController>().knockedDownAnim
            && !rider.GetComponent<MOMovementController>().mounted)
        {
            //Debug.Log(gameObject.name + " Being mounted by " + other);
            //set mounted character to the object collided with
            mountedCharacter = rider.transform.gameObject;

            //adjust the bool
            isCurrentlyMounted = true;

            //adjust mounts position to appear under character as if riding
            mountedCharacter.transform.parent.GetComponent<Rigidbody>().isKinematic = true;
            mountedCharacter.transform.parent.position = new Vector3(this.transform.parent.position.x, this.transform.parent.position.y, this.transform.parent.position.z);
            mountedCharacter.transform.parent.GetComponent<Rigidbody>().isKinematic = false;

            //set collided with game object as the mounts parent
            this.transform.parent.parent = mountedCharacter.transform;
            this.transform.parent.rotation = new Quaternion(mountedCharacter.transform.rotation.x, mountedCharacter.transform.rotation.y, mountedCharacter.transform.rotation.z, mountedCharacter.transform.rotation.w);
            mountedCharacter.transform.position = new Vector3(mountedCharacter.transform.position.x, mountedCharacter.transform.position.y, mountedCharacter.transform.position.z);
            this.transform.parent.localPosition = new Vector3(0, 0, 0);

            mountedCharacter.GetComponent<Animator>().SetBool("mounted", true);
            mountedCharacter.GetComponent<MOMovementController>().mounted = true;
            mountedCharacter.GetComponent<MOMovementController>().mount = this.gameObject;
            mountedCharacter.GetComponent<MOMovementController>().m_GroundCheck = GetComponentInParent<Transform>();

            rb = mountedCharacter.transform.parent.GetComponent<Rigidbody>();

            if (rider.tag == "Enemy")
                rider.GetComponent<AIController>().meleeAttackDistance = attackRange;

            GetComponent<Collider>().enabled = false;
        }

    }

    private void OnTriggerEnter(Collider other)
    {
        //check so that you can't have 2 mounts ride each other. 
        // Mount player (First so it prioritizes the player over enemies)
        if (other.tag == "Player")
        {
            MountCharacter(other);
        }
        // Mount Enemy
        else if (other.tag == "Enemy")
        {
            MountCharacter(other);
        }
    }
}
