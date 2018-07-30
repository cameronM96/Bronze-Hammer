using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MOMovementController : MonoBehaviour
{
    private GameObject scriptEntity;
    [SerializeField] public Collider[] attackTrigger;
    public float entityTurnSpeed = 10.0f;
    private float timerA = 0.0f;
    private float timerC = 0.0f;
    private float timerJ = 0.0f;
    [HideInInspector] public int attackCounter;
    [HideInInspector] public bool freeze;                   // Freeze Character when hit by magic
    [HideInInspector] public bool mounted = false;          // Redirects animations to mount
    [HideInInspector] public GameObject mount;              // The mount gameobject

    public Transform m_GroundCheck;                         // A position marking where to check if the player is grounded.
    const float k_GroundedRadius = .01f;                    // Radius of the overlap circle to determine if grounded
    private bool m_Grounded;                                // Whether or not the character is grounded.
    [SerializeField] private LayerMask m_WhatIsGround;      // A mask determining what is ground to the character

    [SerializeField] private float magicDamage;             // Base magic damage 
    [SerializeField] private int magicLevel;                // magicDamage multiplied by magicLevel
    
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

        if (timerA > 0)
        {
            timerA -= Time.deltaTime;
        }
        else if (timerA <= 0 )
        {
            if (!mounted && attackTrigger[0].enabled == true)
            {
                attackTrigger[0].enabled = false;
                m_Anim.SetBool("attack", false);
                timerA = 0;
            }
            else if (mounted)
            {
                timerA = 0;
                mount.GetComponent<MountingController>().AttackOff();
            }
            //Debug.Log("attack trigger for " + scriptEntity + " is active = " + attackTrigger.activeSelf);
        }

        if (timerC>0)
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

        // Reset relevent animation parameters
        //m_Anim.SetBool("attack", false);
        //m_Anim.SetBool("magic", false);
        //m_Anim.SetBool("charge", false);
    }

    // method is called when needed from an input script
    public void Move(Vector3 mov, float speed)
    {
        if (!m_Anim.GetCurrentAnimatorStateInfo(0).IsName("hurt") && !m_Anim.GetBool("dead") && !m_Anim.GetBool("attack") && 
            !freeze && !m_Anim.GetCurrentAnimatorStateInfo(0).IsName("knocked Down") && 
            !m_Anim.GetCurrentAnimatorStateInfo(0).IsName("Get Up"))
        {
            //move the gameobject based on the vars from the input script
            //scriptEntity.transform.parent.Translate(mov * speed * Time.deltaTime);
            m_Rigidbody.velocity = new Vector3(mov.x * speed, m_Rigidbody.velocity.y, mov.z * speed);

            m_Audio.clip = m_AudioClips[0];
            m_Audio.Play();

            if (mov.x < 0 & mov.z == 0) //left
            {
                entityRotation.Set(0, 270, 0);
            }
            else if (mov.x > 0 & mov.z == 0)//right
            {
                entityRotation.Set(0, 90, 0);
            }
            else if (mov.z > 0 & mov.x == 0)//up
            {
                entityRotation.Set(0, 0, 0);
            }
            else if (mov.z < 0 & mov.x == 0)//down
            {
                entityRotation.Set(0, 180, 0);
            }
            else if (mov.x < 0 & mov.z > 0)//up and left
            {
                entityRotation.Set(0, 315, 0);
            }
            else if (mov.x > 0 & mov.z > 0)//up and right
            {
                entityRotation.Set(0, 45, 0);
            }
            else if (mov.x < 0 & mov.z < 0)//down and left
            {
                entityRotation.Set(0, 225, 0);
            }
            else if (mov.x > 0 & mov.z < 0)//down and right
            {
                entityRotation.Set(0, 135, 0);
            }
        }
    }

    //called from input controller 
    public void Jump(float height)
    {
        if (!m_Anim.GetCurrentAnimatorStateInfo(0).IsName("hurt") && !m_Anim.GetBool("dead") && !freeze
            && !m_Anim.GetCurrentAnimatorStateInfo(0).IsName("knocked Down") && 
            !m_Anim.GetCurrentAnimatorStateInfo(0).IsName("Get Up"))
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
        if (!m_Anim.GetCurrentAnimatorStateInfo(0).IsName("hurt") && !m_Anim.GetBool("dead") && !freeze 
            && !m_Anim.GetCurrentAnimatorStateInfo(0).IsName("Knocked Down") && 
            !m_Anim.GetCurrentAnimatorStateInfo(0).IsName("Get Up"))
        {
            m_Rigidbody.velocity = new Vector3(0, 0, 0);
            // Debug.Log(scriptEntity.name + " attacking");
            if (timerA == 0)
            {
                if (!mounted)
                {
                    if (timerJ > 0 && !m_Grounded)
                    {
                        //Debug.Log("jump attack used");
                        attackCounter = 0;
                        m_Anim.SetBool("attack", true);
                        attackTrigger[0].enabled = true;
                        //Debug.Log("attack trigger for " + scriptEntity + " is active = " + attackTrigger.activeSelf);
                        timerA = 1.0f;
                        //put jump attack here
                    }
                    else if (attackCounter >= 3)
                    {
                        //Debug.Log("3 attack combo used");
                        attackCounter = 0;
                        m_Anim.SetBool("attack", true);
                        attackTrigger[0].enabled = true;
                        //Debug.Log("attack trigger for " + scriptEntity + " is active = " + attackTrigger.activeSelf);
                        timerA = 0.25f;
                        //put knockback here
                    }
                    else if (sprinting)
                    {
                        //Debug.Log("charge attack used");
                        m_Anim.SetBool("charge", true);
                        attackTrigger[0].enabled = true;
                        //Debug.Log("attack trigger for " + scriptEntity + " is active = " + attackTrigger.activeSelf);
                        //put knockback here
                    }
                    else
                    {
                        m_Anim.SetBool("attack", true);
                        //Debug.Log("attack trigger for " + scriptEntity + " is active = " + attackTrigger.activeSelf);
                        timerA = 0.25f;
                        //attack animation and stuff here?
                        attackCounter += 1;

                        if (attackCounter == 2)
                        {
                            if (this.tag == "Player")
                            {
                                switch (GetComponent<MOPlayerInputController>().playerCharacter)
                                {
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
                                attackTrigger[0].enabled = true;
                            }
                        }
                        else
                        {
                            attackTrigger[0].enabled = true;
                        }

                        if (timerC == 0)
                        {
                            timerC = 1;
                        }
                    }
                }
                else
                {
                    mount.GetComponent<MountingController>().Attack();
                    timerA = 2f;
                }
            }
        }
    }

    //called from player's input controller only
    public void Magic()
    {
        if (!m_Anim.GetCurrentAnimatorStateInfo(0).IsName("hurt") && !m_Anim.GetBool("dead") && !freeze
            && !m_Anim.GetCurrentAnimatorStateInfo(0).IsName("knocked Down") && 
            !m_Anim.GetCurrentAnimatorStateInfo(0).IsName("Get Up"))
        {
            if (GetComponent<PlayerHealth>().currentMagicLevel > 0)
            {
                //Debug.Log(scriptEntity.name + " using magic");
                GetComponent<Magic>().CastMagic(this.gameObject, magicDamage, 
                    GetComponent<PlayerHealth>().currentMagicLevel, GetComponent<MOPlayerInputController>().playerCharacter);
                GetComponent<PlayerHealth>().UseMana();

                m_Audio.clip = m_AudioClips[1];
                m_Audio.Play();

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
        m_Anim.SetBool("knockedDown", true);
        m_Rigidbody.velocity = new Vector3(0, 0, 0);
        m_Rigidbody.AddForce((dir * 500), 500,0);
    
        if(mounted)
        {
            mount.GetComponent<MountingController>().UnMounted();
        }
    }

    public void Death()
    {
        m_Anim.SetBool("dead", true);
        m_Anim.SetBool("knockedDown", true);
        m_Audio.clip = m_AudioClips[0];
        m_Audio.Play();
        this.gameObject.layer = 15;
        StartCoroutine(FallThroughFloor(5));
        if (this.tag != "Player")
        {
            Destroy(this.transform.parent.gameObject, 7);
        }
    }

    IEnumerator FallThroughFloor (float waittimer)
    {
        yield return new WaitForSeconds(waittimer);
        GetComponent<Collider>().enabled = false;
    }
}
