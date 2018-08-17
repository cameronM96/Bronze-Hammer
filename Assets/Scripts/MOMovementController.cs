using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MOMovementController : MonoBehaviour
{
    private GameObject scriptEntity;
    public Collider[] attackTrigger;
    public Collider chargeCollider;
    public float entityTurnSpeed = 10.0f;
    private float timerA = 0.0f;                            // Attack speed limiter
    private float timerC = 0.0f;                            // Resets attack timer
    private float timerJ = 0.0f;                            // Jump timer
    [SerializeField] private GameObject shadowPrefab;             
    private GameObject shadow;                                     // Shadow for a reference point when jumping  
    [HideInInspector] public bool dead = false;
    [HideInInspector] public int attackCounter;

    // Animation feedback
    [HideInInspector] public bool freeze = false;                   // Freeze Character when hit by magic
    [HideInInspector] public bool attackingAnim = false;            // Don't let character move until attack is over
    [HideInInspector] public bool hurtAnim = false;                 // Check if character is hurt
    [HideInInspector] public bool knockedDownAnim = false;          // Check if character is knocked down
    [HideInInspector] public bool castingMagic = false;             // Check if character is casting magic
    [HideInInspector] public bool charging = false;                 // Check if character is charging
    [HideInInspector] public bool knockback = false;                // Knocks the player back in fixedUpdate
    [HideInInspector] public float knockbackxDir = 0f;               // Determines direction the character should be knocked back along x axis.
    [HideInInspector] public float knockbackzDir = 0f;               // Determines direction the character should be knocked back along z axis.

    [HideInInspector] public bool mounted = false;          // Redirects animations to mount
    [HideInInspector] public GameObject mount;              // The mount gameobject

    // Ground check data
    public Transform m_GroundCheck;                         // A position marking where to check if the player is grounded.
    const float k_GroundedRadius = .02f;                    // Radius of the overlap circle to determine if grounded
    private bool m_Grounded;                                // Whether or not the character is grounded.
    [SerializeField] private LayerMask m_WhatIsGround;      // A mask determining what is ground to the character

    // Jump smoothing data
    public float fallMultiplier = 2.5f;
    public float lowJumpMultiplier = 2f;

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
        if (entityID == 1)
        {
            Vector3 origin = new Vector3(transform.parent.position.x, transform.parent.position.y + 1f, transform.parent.position.z);
            RaycastHit hit;
            if (Physics.Raycast(origin, Vector3.down, out hit, Mathf.Infinity, m_WhatIsGround))
            {
                shadow.transform.position = hit.point;
            }
            else
            {
                shadow.transform.position = transform.parent.position;
            }
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
                    //Debug.Log("Player attack reset");
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
                        //Debug.Log("Player attack reset");
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
                    //Debug.Log("Player attack reset");
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
                if (mounted)
                    mount.GetComponent<MountingController>().m_Anim.SetBool("jump", false);
            }
        }

        // TODO: Add raycast down and change only the animator (this should fix the landing animation)

        if (m_Rigidbody.velocity.y < 0 && !m_Grounded && m_Anim != null)
        {
            Vector3 origin = new Vector3(transform.parent.position.x, transform.parent.position.y + 1f, transform.parent.position.z);
            RaycastHit hit;
            if (Physics.Raycast(origin, Vector3.down, out hit, 5f, m_WhatIsGround))
            {
                m_Anim.SetBool("grounded", true);
            }
        }
        else if (m_Anim != null)
        {
            m_Anim.SetBool("grounded", m_Grounded);
        }
    }

    protected virtual void FixedUpdate()
    {
        // Jump physics
        if (m_Rigidbody.velocity.y < 0)
        {
            // If falling increase gravity to make them fall faster
            m_Rigidbody.velocity += Vector3.up * Physics.gravity.y * (fallMultiplier - 1) * Time.fixedDeltaTime;
        }
        else if (this.tag == "Player")
        {
            // If falling and not pressing jump button increase gravity to make them fall faster
            if (m_Rigidbody.velocity.y > 0 && !Input.GetButtonDown("Jump Button")) {
                m_Rigidbody.velocity += Vector3.up * Physics.gravity.y * (lowJumpMultiplier - 1) * Time.fixedDeltaTime;
            }
        }

        // Execute Charge Mechanic
        if (this.tag == "Player")
        {
            if (charging)
            {
                Charge();
            }
            else if (!charging && chargeCollider.enabled == true)
            {
                chargeCollider.enabled = false;
                m_Rigidbody.velocity = Vector3.zero;
            }
        }

        // Execute Knockback mechanic
        if (knockback)
        {
            KnockBack(knockbackxDir, knockbackzDir);
            knockbackxDir = 0;
            knockbackzDir = 0;
            knockback = false;
        }
    }

    private void LateUpdate()
    {
        // Running animation
        Vector3 groundVelocity = m_Rigidbody.velocity;
        groundVelocity.y = 0f;

        m_Anim.SetFloat("velocity", groundVelocity.magnitude);

        if (groundVelocity.magnitude > 0.1f && m_Grounded)
        {
            m_Audio.clip = m_AudioClips[0];
            m_Audio.volume = 0.7f;
            m_Audio.Play();
            m_Audio.loop = true;
        }
        else
        {
            m_Audio.loop = false;
        }
    }

    // method is called when needed from an input script
    public void Move(Vector3 mov)
    {
        if (!attackingAnim && !freeze && !knockedDownAnim && !dead && !castingMagic && !charging)
        {
            if (this.tag == "Player" || !hurtAnim)
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
                    m_Rigidbody.velocity = new Vector3(mov.x, m_Rigidbody.velocity.y, mov.z);
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
    }

    //called from input controller 
    public void Jump(float height)
    {
        if (!hurtAnim && !attackingAnim && !freeze && !knockedDownAnim && !dead && !castingMagic && !charging)
        {
            if (timerJ <= 0.0f && m_Grounded)
            {
                //Debug.Log(scriptEntity.name + " jumping");
                //m_Rigidbody.AddForce(0, height, 0);
                m_Rigidbody.velocity = Vector3.up * height;

                timerJ = 2.0f;
                if (mounted)
                    mount.GetComponent<MountingController>().m_Anim.SetBool("jump", true);
                else
                    m_Anim.SetBool("jump", true);
            }
        }
    }

    //called from input controller
    public void Attack(bool sprinting)
    {
        if (!hurtAnim && !freeze && !knockedDownAnim && !dead && !castingMagic)
        {
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
                        // Charge Attack
                        //Debug.Log("charge attack used");
                        m_Anim.SetBool("charge", true);
                        chargeCollider.enabled = true;
                        //Debug.Log("attack trigger for " + scriptEntity + " is active = " + attackTrigger.activeSelf);
                        //put knockback here
                    }
                    else
                    {
                        // Normal attack (attack 1 and 2)
                        m_Anim.SetBool("attack", true);
                        
                        timerA = 0.25f;
                        //Debug.Log("Basic attack used, length is " + timerA);
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
                    timerA = 2.3f;
                }
            }
        }
    }

    public void BossAttack(int attackNumber)
    {
        Debug.Log("Boss Attack Called with attack "+ attackNumber);
        if (!hurtAnim && !freeze && !knockedDownAnim && !dead && !castingMagic)
        {
            // Debug.Log(scriptEntity.name + " attacking");
            if (attackNumber == 1)
            {
                attackTrigger[0].enabled = true;
                m_Anim.SetBool("Attack 1", true);
            }
            else if (attackNumber == 2)
            {
                attackTrigger[1].enabled = true;
                m_Anim.SetBool("Attack 2", true);
            }
            else if (attackNumber == 3)
            {
                attackTrigger[2].enabled = true;
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
                m_Audio.volume = 1f;
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

    public void Charge()
    {
        // Make character move quicking left or right
        // Determine direction
        float mov;
        if (entityRotation.y >= 0 && entityRotation.y <= 180)
            mov = 1f;
        else
            mov = -1f;

        // Find speed
        float speed = 1f;
        if (this.tag == "Player")
            speed = GetComponent<MOPlayerInputController>().moveSpeed;
        else if (this.tag == "Enemy")
            speed = GetComponent<AIController>().moveSpeed;

        // Execute physics part of charge (moving forward quickly)
        m_Rigidbody.velocity = new Vector3(mov * speed * 2, m_Rigidbody.velocity.y, m_Rigidbody.velocity.z);
    }

    private void KnockBack(float xdir, float zdir)
    {
        // Knock back mechanic which sends this character flying backwards
        //Debug.Log("Knockback enabled");
        m_Anim.SetBool("knockedDown", true);
        knockedDownAnim = true;

        // Sounds
        m_Audio.clip = m_AudioClips[3];
        m_Audio.volume = 1f;
        m_Audio.Play();

        //Dis-mount character if knocked back
        DisMount();

        // Actually knockback enemy
        m_Rigidbody.velocity = new Vector3((xdir * 15), 10, (zdir * 15));
    }

    public void DisMount()
    {
        //Dis-mount character if knocked back
        if (mounted)
        {
            m_Anim.SetBool("mounted", false);
            mount.GetComponent<MountingController>().UnMounted();
            mount = null;
            m_GroundCheck = GetComponentInParent<Transform>();
            mounted = false;
            if (this.tag == "Enemy")
                GetComponent<AIController>().meleeAttackDistance = GetComponent<AIController>().attackDistance;
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
        m_Audio.clip = m_AudioClips[2];
        m_Audio.volume = 1f;
        m_Audio.Play();
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
