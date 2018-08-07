﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MOMovementController : MonoBehaviour
{
    private GameObject scriptEntity;
    [SerializeField] public Collider[] attackTrigger;
    public float entityTurnSpeed = 10.0f;
    private float timerA = 0.0f;                            // Attack speed limiter
    private float timerC = 0.0f;                            // Resets attack timer
    private float timerJ = 0.0f;                            // Jump timer
    [SerializeField] private GameObject shadowPrefab;             
    private GameObject shadow;                                     // Shadow for a reference point when jumping  
    private int shadowRayMask = 1 << 17;                           // https://docs.unity3d.com/Manual/Layers.html
    [HideInInspector] public bool dead = false;
    [HideInInspector] public int attackCounter;

    // Animation feedback
    [HideInInspector] public bool freeze = false;                   // Freeze Character when hit by magic
    [HideInInspector] public bool attackingAnim = false;            // Don't let character move until attack is over
    [HideInInspector] public bool hurtAnim = false;                 // Check if character is hurt
    [HideInInspector] public bool knockedDownAnim = false;          // Check if character is knocked down
    [HideInInspector] public bool castingMagic = false;             // Check if character is casting magic

    [HideInInspector] public bool mounted = false;          // Redirects animations to mount
    [HideInInspector] public GameObject mount;              // The mount gameobject

    // Ground check data
    public Transform m_GroundCheck;                         // A position marking where to check if the player is grounded.
    const float k_GroundedRadius = .01f;                    // Radius of the overlap circle to determine if grounded
    private bool m_Grounded;                                // Whether or not the character is grounded.
    [SerializeField] private LayerMask m_WhatIsGround;      // A mask determining what is ground to the character

    // Magic Data
    [SerializeField] private float magicDamage;             // Base magic damage 
    [SerializeField] private int magicLevel;                // magicDamage multiplied by magicLevel

    // Character movement variables
    public int entityID = 0;
    public Vector3 entityRotation;

    private Rigidbody m_Rigidbody;
    private Animator m_Anim;

    private AudioSource m_Audio;
    [SerializeField] private AudioClip[] m_AudioClips;

    // Use this for initialization
    protected virtual void Awake()
    {
        //check what entity the script is attached to used for debugging and testing reasons and possible mounting controls
        if (entityID == 1) // ID 1 = PLAYER
        {
            scriptEntity = GameObject.FindGameObjectWithTag("Player");
            shadow = Instantiate(shadowPrefab);
        }
        else if (entityID == 2) // ID 2 = ENEMY
        {
            scriptEntity = this.gameObject;
        }
        else // if no ID matched return this error with what entity it is not working for
        {
            Debug.Log("Controller not working for " + gameObject.name);
        }

        m_Rigidbody = GetComponentInParent<Rigidbody>();
        m_GroundCheck = GetComponentInParent<Transform>();
        m_Anim = GetComponent<Animator>();
        m_Anim.SetBool("grounded", true);
        m_Audio = GetComponent<AudioSource>();

        foreach (Collider collider in attackTrigger)
        {
            collider.enabled = false;
        }
        attackCounter = 0;
    }

    protected virtual void Update()
    {
        //scriptEntity.transform.rotation = Quaternion.Euler(entityRotation*entityTurnSpeed*Time.deltaTime);
        scriptEntity.transform.rotation =
            Quaternion.Lerp(scriptEntity.transform.rotation,
            Quaternion.Euler(entityRotation), entityTurnSpeed * Time.deltaTime);

        // Set shadow position
        RaycastHit hit;
        if (Physics.Raycast(transform.parent.position,Vector3.down, out hit, Mathf.Infinity, shadowRayMask))
        {
            shadow.transform.position = hit.point;
        }

        // Reset attacks
        if (timerA > 0)
        {
            timerA -= Time.deltaTime;
        }
        else if (!attackingAnim)
        {
            if (this.gameObject.tag == "Player")
            {
                // Reset players attacks
                if (GetComponent<MOPlayerInputController>().playerCharacter == PlayerCharacters.Lilith)
                {
                    Debug.Log("Player attack reset");
                    m_Anim.SetBool("attack", false);
                    attackTrigger[0].enabled = false;
                    attackTrigger[1].enabled = false;
                    attackTrigger[0].gameObject.GetComponent<Attack>().attack2 = false;
                    attackTrigger[0].gameObject.GetComponent<Attack>().attack3 = false;
                    attackTrigger[1].gameObject.GetComponent<Attack>().attack2 = false;
                    attackTrigger[1].gameObject.GetComponent<Attack>().attack3 = false;
                    attackCounter = 0;
                    timerA = 0;
                }
                else
                {
                    if (attackTrigger[0].enabled == true)
                    {
                        Debug.Log("Player attack reset");
                        attackTrigger[0].enabled = false;
                        attackTrigger[0].gameObject.GetComponent<Attack>().attack2 = false;
                        attackTrigger[0].gameObject.GetComponent<Attack>().attack3 = false;
                        attackCounter = 0;
                        m_Anim.SetBool("attack", false);
                        timerA = 0;
                    }
                }

                if (mounted)
                {
                    mount.GetComponent<MountingController>().AttackOff();
                    timerA = 0;
                }
            }
            else
            {
                // Reset AI attacks
                if (attackTrigger[0].enabled == true)
                {
                    Debug.Log("Player attack reset");
                    attackTrigger[0].enabled = false;
                    attackTrigger[0].gameObject.GetComponent<Attack>().attack2 = false;
                    attackTrigger[0].gameObject.GetComponent<Attack>().attack3 = false;
                    attackCounter = 0;
                    m_Anim.SetBool("attack", false);
                    timerA = 0;
                }

                if (mounted)
                {
                    mount.GetComponent<MountingController>().AttackOff();
                    timerA = 0;
                }
            }
            //Debug.Log("attack trigger for " + scriptEntity + " is active = " + attackTrigger.activeSelf);
        }

        if (timerC > 0)
        {
            timerC -= Time.deltaTime;
        }

        if (timerJ > 0)
        {
            timerJ -= Time.deltaTime;
            //  Debug.Log("timerJ = " + timerJ);
        }

        m_Grounded = false;

        // The player is grounded if a circlecast to the groundcheck position hits anything designated as ground
        // This can be done using layers instead but Sample Assets will not overwrite your project settings.
        Collider[] colliders = Physics.OverlapSphere(m_GroundCheck.position, k_GroundedRadius, m_WhatIsGround);
        for (int i = 0; i < colliders.Length; i++)
        {
            if (colliders[i].gameObject != gameObject)
            {
                m_Grounded = true;
                m_Anim.SetBool("jump", false);
            }
        }

        if (m_Anim != null)
            m_Anim.SetBool("grounded", m_Grounded);
    }

    private void LateUpdate()
    {
        Vector3 groundVelocity = m_Rigidbody.velocity;
        groundVelocity.y = 0f;

        m_Anim.SetFloat("velocity", groundVelocity.magnitude);
    }

    // method is called when needed from an input script
    public void Move(Vector3 mov, float speed)
    {
        if (!hurtAnim && !attackingAnim && !freeze && !knockedDownAnim && !dead && !castingMagic)
        {
            bool inAir = false;
            if (this.gameObject.tag != "Player" && !m_Grounded)
            {
                inAir = true;
            }

            //move the gameobject based on the vars from the input script
            //scriptEntity.transform.parent.Translate(mov * speed * Time.deltaTime);
            if (!inAir)
            {
                m_Rigidbody.velocity = new Vector3(mov.x * speed, m_Rigidbody.velocity.y, mov.z * speed);

                m_Audio.clip = m_AudioClips[0];
                m_Audio.Play();
                m_Audio.volume = 0.2f;
            }

            if (mov.x < -0.2f && (mov.z < 0.2f && mov.z > -0.2f)) //left
            {
                entityRotation.Set(0, 270, 0);
            }
            else if (mov.x > 0.2f && (mov.z < 0.2f && mov.z > -0.2f))//right
            {
                entityRotation.Set(0, 90, 0);
            }
            else if (mov.z > 0.2f && (mov.x < 0.2f && mov.x > -0.2f))//up
            {
                entityRotation.Set(0, 0, 0);
            }
            else if (mov.z < -0.2f && (mov.x < 0.2f && mov.x > -0.2f))//down
            {
                entityRotation.Set(0, 180, 0);
            }
            else if (mov.x < -0.2f && mov.z > 0.2f)//up and left
            {
                entityRotation.Set(0, 315, 0);
            }
            else if (mov.x > 0.2f && mov.z > 0.2f)//up and right
            {
                entityRotation.Set(0, 45, 0);
            }
            else if (mov.x < -0.2f && mov.z < -0.2f)//down and left
            {
                entityRotation.Set(0, 225, 0);
            }
            else if (mov.x > 0.2f && mov.z < -0.2f)//down and right
            {
                entityRotation.Set(0, 135, 0);
            }
        }
    }

    //called from input controller 
    public void Jump(float height)
    {
        if (!hurtAnim && !attackingAnim && !freeze && !knockedDownAnim && !dead && !castingMagic)
        {
            if (timerJ <= 0.0f && m_Grounded)
            {
                //Debug.Log(scriptEntity.name + " jumping");
                m_Anim.SetBool("jump", true);
                m_Rigidbody.AddForce(0, height, 0);
                timerJ = 2.0f;
            }
        }
    }

    //called from input controller
    public void Attack(bool sprinting)
    {
        if (!hurtAnim && !freeze && !knockedDownAnim && !dead && !castingMagic)
        {
            if (m_Grounded)
                m_Rigidbody.velocity = new Vector3(0, 0, 0);

            // Debug.Log(scriptEntity.name + " attacking");
            if (timerA <= 0)
            {
                if (!mounted)
                {
                    if (timerJ > 0 && !m_Grounded)
                    {
                        // Jump attack
                        //Debug.Log("jump attack used");
                        attackCounter = 0;
                        m_Anim.SetBool("attack", true);
                        attackTrigger[0].enabled = true;
                        //Debug.Log("attack trigger for " + scriptEntity + " is active = " + attackTrigger.activeSelf);
                        timerA = 1.0f;
                        //put jump attack here
                    }
                    else if (attackCounter >= 2)
                    {
                        // Combo finisher (third attack)
                        Debug.Log("3 attack combo used");
                        attackCounter = 0;
                        m_Anim.SetBool("attack", true);
                        attackTrigger[0].enabled = true;
                        //Debug.Log("attack trigger for " + scriptEntity + " is active = " + attackTrigger.activeSelf);
                        timerA = 0.25f;
                        //put knockback here
                    }
                    else if (sprinting)
                    {
                        // Charge Attack
                        //Debug.Log("charge attack used");
                        m_Anim.SetBool("charge", true);
                        attackTrigger[0].enabled = true;
                        //Debug.Log("attack trigger for " + scriptEntity + " is active = " + attackTrigger.activeSelf);
                        //put knockback here
                    }
                    else
                    {
                        // Normal attack (attack 1 and 2)
                        m_Anim.SetBool("attack", true);
                        
                        timerA = 0.25f;
                        Debug.Log("Basic attack used, length is " + timerA);
                        //attack animation and stuff here?
                        if (attackCounter == 1)
                        {
                            if (this.tag == "Player")
                            {
                                switch (GetComponent<MOPlayerInputController>().playerCharacter)
                                {
                                    // Use left hand if lilith otherwise use right
                                    case PlayerCharacters.Lilith:
                                        attackTrigger[1].enabled = true;
                                        break;
                                    default:
                                        attackTrigger[0].enabled = true;
                                        break;
                                }
                            }
                            else
                            {
                                // Use right hand if not second attack
                                attackTrigger[0].enabled = true;
                            }
                        }
                        else
                        {
                            // normal attack used by AI and players
                            attackTrigger[0].enabled = true;
                        }
                        
                        attackCounter += 1;

                        if (timerC == 0)
                        {
                            timerC = 1;
                        }
                    }
                }
                else
                {
                    // Mount attack (freeze for duration of attack)
                    mount.GetComponent<MountingController>().Attack();
                    timerA = 2.2f;
                }
            }
        }
    }

    public void BossAttack(int attackNumber)
    {
        if (!m_Anim.GetCurrentAnimatorStateInfo(0).IsName("hurt") && !m_Anim.GetBool("dead") && !freeze
            && !m_Anim.GetCurrentAnimatorStateInfo(0).IsName("Knocked Down") &&
            !m_Anim.GetCurrentAnimatorStateInfo(0).IsName("Get Up") && !dead)
        {
            m_Rigidbody.velocity = new Vector3(0, 0, 0);
            // Debug.Log(scriptEntity.name + " attacking");
            if (attackNumber == 1)
            {
                attackTrigger[0].enabled = true;
                m_Anim.SetBool("Attack 1", true);
            }
            else if (attackNumber == 2)
            {
                attackTrigger[0].enabled = true;
                m_Anim.SetBool("Attack 2", true);
            }
            else if (attackNumber == 3)
            {
                attackTrigger[0].enabled = true;
                m_Anim.SetBool("Attack 3", true);
            }
        }
    }
    
    //called from player's input controller only
    public void Magic()
    {
        // Cast magic
        if (!hurtAnim && !attackingAnim && !freeze && !knockedDownAnim && !dead)
        {
            // Cast magic if their level is greater than 0
            if (GetComponent<PlayerHealth>().currentMagicLevel > 0)
            {
                //Debug.Log(scriptEntity.name + " using magic");
                GetComponent<Magic>().CastMagic(this.gameObject, magicDamage,
                    GetComponent<PlayerHealth>().currentMagicLevel, GetComponent<MOPlayerInputController>().playerCharacter);

                // Use up mana
                GetComponent<PlayerHealth>().UseMana();

                // Animations and sounds
                m_Audio.clip = m_AudioClips[1];
                m_Audio.Play();
                m_Audio.volume = 1f;

                if (mounted)
                {
                    mount.GetComponent<MountingController>().m_Anim.SetBool("magic", true);
                }
                else
                {
                    m_Anim.SetBool("magic", true);
                }
            }
        }
    }

    public void KnockBack(float dir)
    {
        // Knock back mechanic which sends this character flying backwards
        m_Anim.SetBool("knockedDown", true);
        m_Rigidbody.velocity = new Vector3(0, 0, 0);
        m_Rigidbody.AddForce((dir * 250), 250, 0);
    
        //Dis-mount character if knocked back
        if(mounted)
        {
            m_Anim.SetBool("mounted", false);
            mount.GetComponent<MountingController>().UnMounted();
            m_GroundCheck = GetComponentInParent<Transform>();
        }
    }

    public void Death()
    {
        dead = true;
        // Dis-mount character if killed
        if (mounted)
        {
            m_Anim.SetBool("mounted", false);
            mount.GetComponent<MountingController>().UnMounted();
            m_GroundCheck = GetComponentInParent<Transform>();
        }

        // Kill character
        m_Anim.SetBool("dead", true);
        m_Anim.SetBool("knockedDown", true);
        m_Audio.clip = m_AudioClips[0];
        m_Audio.Play();
        m_Audio.volume = 1f;
        this.gameObject.layer = 15;
        StartCoroutine(FallThroughFloor(5));

        // Destroy character if it's not a player
        if (this.tag != "Player")
            Destroy(this.transform.parent.gameObject, 7);
    }

    IEnumerator FallThroughFloor(float waittimer)
    {
        yield return new WaitForSeconds(waittimer);
        GetComponent<Collider>().enabled = false;
    }
}
