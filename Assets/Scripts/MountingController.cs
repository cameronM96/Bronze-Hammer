﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MountingController : MonoBehaviour
{
    private GameObject mountedCharacter;
    private Rigidbody rb;
    public Animator m_Anim;
    [SerializeField] private bool ranged;
    [SerializeField] private GameObject attack;
    [SerializeField] private GameObject attackPoint;

    public bool isCurrentlyMounted;

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
        }
    }

    public void Attack()
    {
        m_Anim.SetBool("attack", true);
        if (ranged)
        {
            // Long ranged attack
            GameObject projectile = Instantiate(attack, attackPoint.transform);
            projectile.transform.parent = null;
            if (mountedCharacter.tag == "Player")
            {
                projectile.transform.GetChild(0).GetComponentInChildren<MountAttack>().playerAttack = true;
            }
            else
            {
                projectile.transform.GetChild(0).GetComponentInChildren<MountAttack>().playerAttack = false;
            }
        }
        else
        {
            // Short ranged attack
            if(mountedCharacter.tag == "Player")
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
        m_Anim.SetBool("attack", false);
        if (!ranged)
        {
            attack.SetActive(false);
        }
    }

    public void UnMounted()
    {
        mountedCharacter = null;
        rb = null;
        isCurrentlyMounted = false;
        this.transform.parent.parent = null;
    }

    private void OnTriggerEnter(Collider other)
    {
        //check so that you can have 2 mounts ride each other. 
        if (other.tag == "Player" || other.tag == "Enemy")
        {
            Debug.Log("Collided with character, mounting");
            //check not already mounted by something
            if (!isCurrentlyMounted && !other.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsName("knocked Down"))
            {
                Debug.Log(gameObject.name + " Being mounted by " + other);
                //set mounted character to the object collided with
                mountedCharacter = other.transform.gameObject;

                //adjust the bool
                isCurrentlyMounted = true;

                //adjust mounts position to appear under character as if riding
                mountedCharacter.transform.parent.GetComponent<Rigidbody>().isKinematic = true;
                mountedCharacter.transform.parent.position = new Vector3(this.transform.parent.position.x, this.transform.parent.position.y, this.transform.parent.position.z);
                mountedCharacter.transform.parent.GetComponent<Rigidbody>().isKinematic = false;

                //set collided with game object as the mounts parent
                this.transform.parent.parent = mountedCharacter.transform;
                this.transform.parent.rotation = new Quaternion(mountedCharacter.transform.rotation.x, mountedCharacter.transform.rotation.y, mountedCharacter.transform.rotation.z, mountedCharacter.transform.rotation.w);
                mountedCharacter.transform.position = new Vector3(mountedCharacter.transform.position.x, mountedCharacter.transform.position.y + 0.8f, mountedCharacter.transform.position.z);
                this.transform.parent.localPosition = new Vector3(0, -0.2f, 0);

                mountedCharacter.GetComponent<Animator>().SetBool("mounted", true);
                mountedCharacter.GetComponent<MOMovementController>().mounted = true;
                mountedCharacter.GetComponent<MOMovementController>().mount = this.gameObject;
                mountedCharacter.GetComponent<MOMovementController>().m_GroundCheck = GetComponentInParent<Transform>();

                rb = mountedCharacter.transform.parent.GetComponent<Rigidbody>();
            }
        }
    }
}
