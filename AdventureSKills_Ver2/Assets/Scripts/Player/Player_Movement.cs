using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum PlayerStates
{
    IDLE,
    RUN,
    JUMP,
    JUMPMOVE,
    AIR,
    AIRMOVE,
    ATTACK,
    AIRATTACK,
    SKILL,
    AIRSKILL,
    DAMAGED,
    DEAD
}

public class Player_Movement : Interactable
{
    public PlayerStates state = new PlayerStates();
    private PlayerStates previousState = new PlayerStates();

    [SerializeField]
    private GameObject[] projectile = null;   
    private Animator anim;

    [HideInInspector]
    public Player_Inputs inputs;
    [HideInInspector]
    public Transform _transform;
    [HideInInspector]
    public Rigidbody2D rb;

    public SpriteRenderer[] sprites;

    [SerializeField]
    private Vector2 groundCheckSize = Vector2.zero, groundCheckOffset = Vector2.zero;
    [SerializeField]
    private LayerMask groundMask = new LayerMask();
    [HideInInspector]
    public bool isGrounded;
    private bool canJump = false;

    public float idleFriction = 12f, turnAroundSpeed = 4f;
    public float groundAcceleration = 80f, groundMaxSpd = 12f;
    public float airAcceleration = 80f, airMaxSpd = 12f;
    public float deadMaxSpd = 5;
    public float airAttackAcceleration = 20;
    public float jumpHeight = 5f, fallMultiplier = .75f, highFallMultiplier = 3f, fallMaxSpd = 60f;
    public float knockbackPower = 100f;

    [HideInInspector]
    public float gravity;

    private Vector2 velocityAux;

    // Start is called before the first frame update
    void Start()
    {
        _transform = GetComponent<Transform>();
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        inputs = GetComponent<Player_Inputs>();

        gravity = rb.gravityScale * Physics2D.gravity.y;

        previousState = PlayerStates.IDLE;

        sprites = GetComponentsInChildren<SpriteRenderer>();
    }

    // Update is called once per frame
    public virtual void Update()
    {
        CollisionsCheck();
        StateManager();
        StatesMoveManager();
        AnimationManager();

        if (state != previousState)
            previousState = state;
    }

    private void FixedUpdate()
    {
        switch (state)
        {

            case PlayerStates.IDLE:

                rb.velocity = new Vector2(Mathf.Lerp(rb.velocity.x, 0, idleFriction * Time.deltaTime), rb.velocity.y);
                Attack();

                break;
            case PlayerStates.RUN:

                Move(groundAcceleration);
                Attack();

                break;

            case PlayerStates.JUMP:

                if (!inputs.holdingJump)
                    rb.velocity += Vector2.up * gravity * highFallMultiplier * Time.deltaTime;

                rb.velocity = new Vector2(Mathf.Lerp(rb.velocity.x, 0, idleFriction * Time.deltaTime), rb.velocity.y);
                Attack();

                break;

            case PlayerStates.JUMPMOVE:

                if (!inputs.holdingJump)
                    rb.velocity += Vector2.up * gravity * highFallMultiplier * Time.deltaTime;

                Move(airAcceleration);
                Attack();

                break;

            case PlayerStates.AIR:

                Attack();

                rb.velocity = new Vector2(Mathf.Lerp(rb.velocity.x, 0, idleFriction * Time.deltaTime), rb.velocity.y);

                rb.velocity += Vector2.up * gravity * fallMultiplier * Time.deltaTime;

                break;

            case PlayerStates.AIRMOVE:

                Attack();
                Move(airAcceleration);

                rb.velocity += Vector2.up * gravity * fallMultiplier * Time.deltaTime;

                break;

            case PlayerStates.ATTACK:

                rb.velocity = new Vector2(Mathf.Lerp(rb.velocity.x, 0, (idleFriction / 2) * Time.deltaTime), rb.velocity.y);

                if (!isGrounded && anim.GetCurrentAnimatorStateInfo(0).normalizedTime < .1f)
                {
                    float animTime = anim.GetCurrentAnimatorStateInfo(0).normalizedTime;
                    state = PlayerStates.AIRATTACK;
                    anim.Play("Player_AirAttack", 0, animTime);
                }

                break;

            case PlayerStates.AIRATTACK:

                if (isGrounded)
                {
                    if (anim.GetCurrentAnimatorStateInfo(0).normalizedTime < .1f)
                    {
                        float animTime = anim.GetCurrentAnimatorStateInfo(0).normalizedTime;
                        state = PlayerStates.ATTACK;
                        anim.Play("Player_Attack", 0, animTime);
                    }
                    else if (inputs.hor == 0)
                        state = PlayerStates.IDLE;
                    else
                        state = PlayerStates.RUN;
                }

                Move(airAttackAcceleration);

                if (!inputs.holdingJump && rb.velocity.y > .5f)
                    rb.velocity += Vector2.up * gravity * highFallMultiplier * Time.deltaTime;

                if (rb.velocity.y < .5f)
                    rb.velocity += Vector2.up * gravity * fallMultiplier * Time.deltaTime;

                break;

            case PlayerStates.SKILL:

                SkillState();

                break;

            case PlayerStates.AIRSKILL:

                AirSkillState();

                break;

            case PlayerStates.DAMAGED:

                rb.velocity = new Vector2(Mathf.Lerp(rb.velocity.x, 0, idleFriction * Time.deltaTime), rb.velocity.y);

                break;

            case PlayerStates.DEAD:

                rb.velocity = new Vector2(Mathf.Lerp(rb.velocity.x, 0, idleFriction * Time.deltaTime), rb.velocity.y);

                if (rb.velocity.y < .5f)
                    rb.velocity += Vector2.up * gravity * fallMultiplier * Time.deltaTime;

                break;
        }
    }

    private void StateManager()
    {
        switch(state)
        {
            case PlayerStates.IDLE:

                if (inputs.hor != 0)
                    state = PlayerStates.RUN;

                if (!isGrounded)
                    state = PlayerStates.AIR;

                Jump();
                Skill();

                break;

            case PlayerStates.RUN:

                if (inputs.hor == 0)
                    state = PlayerStates.IDLE;

                if (!isGrounded)
                    state = PlayerStates.AIRMOVE;

                Jump();
                Skill();

                break;

            case PlayerStates.JUMP:

                if (isGrounded && rb.velocity.y <= 0.5f)
                    state = PlayerStates.IDLE;

                else if(!isGrounded && rb.velocity.y < -0.05f)
                    state = PlayerStates.AIR;

                if (inputs.hor != 0)
                    state = PlayerStates.JUMPMOVE;

                Skill();

                break;

            case PlayerStates.JUMPMOVE:

                if (isGrounded && rb.velocity.y <= 0.5f)
                    state = PlayerStates.RUN;

                if (!isGrounded && rb.velocity.y < 0f)
                    state = PlayerStates.AIRMOVE;

                if (inputs.hor == 0)
                    state = PlayerStates.JUMP;

                Skill();

                break;

            case PlayerStates.AIR:      

                if (isGrounded)
                    state = PlayerStates.IDLE;

                if (inputs.hor != 0)
                    state = PlayerStates.AIRMOVE;

                Jump();
                Skill();

                break;

            case PlayerStates.AIRMOVE:

                if (isGrounded)
                    state = PlayerStates.RUN;

                if (inputs.hor == 0)
                    state = PlayerStates.AIR;

                Jump();
                Skill();

                break;

            case PlayerStates.DEAD:

                if (health > 0)
                    state = PlayerStates.IDLE;

                break;
        }
    }

    public virtual void CollisionsCheck()
    {
        isGrounded = Physics2D.OverlapBox((Vector2)_transform.position + groundCheckOffset, groundCheckSize, 0, groundMask);

        if (!isGrounded && canJump)
            StartCoroutine(CoyoteTime());

        if (isGrounded && !canJump && (state == PlayerStates.IDLE || state == PlayerStates.RUN))
            canJump = true;
    }

    private void Move(float acceleration)
    {
        if(inputs.hor * rb.velocity.x > 0)
            rb.AddForce(Vector2.right * inputs.hor * acceleration, ForceMode2D.Force);
        else
            rb.AddForce(Vector2.right * inputs.hor * acceleration * turnAroundSpeed, ForceMode2D.Force);
    }

    public void StatesMoveManager()
    {
        switch (state)
        {
            case PlayerStates.RUN:
            case PlayerStates.IDLE:

                MoveManager(groundMaxSpd);

                if (inputs.hor != 0)
                    _transform.eulerAngles = Vector3.up * (inputs.hor == 1 ? 0 : 180);

                break;

            case PlayerStates.JUMP:
            case PlayerStates.JUMPMOVE:
            case PlayerStates.AIR:
            case PlayerStates.AIRMOVE:

                MoveManager(airMaxSpd);

                if (inputs.hor != 0)
                    _transform.eulerAngles = Vector3.up * (inputs.hor == 1 ? 0 : 180);

                break;

            case PlayerStates.ATTACK:

                MoveManager(groundMaxSpd);

                break;

            case PlayerStates.AIRATTACK:

                MoveManager(airMaxSpd);

                break;

            case PlayerStates.DAMAGED:

                if(isGrounded)
                    MoveManager(groundMaxSpd);
                else
                    MoveManager(airMaxSpd);

                break;

            case PlayerStates.DEAD:

                MoveManager(deadMaxSpd);

                break;

        }
    }

    private void MoveManager(float maxSpd)
    {
        velocityAux = rb.velocity;
        velocityAux.x = Mathf.Clamp(velocityAux.x, -maxSpd, maxSpd);
        velocityAux.y = Mathf.Clamp(velocityAux.y, -fallMaxSpd, fallMaxSpd);
        rb.velocity = velocityAux;
    }

    private void Jump()
    {
        if(canJump && inputs.GetButton("JUMP"))
        {
            canJump = false;
            rb.velocity = new Vector2(rb.velocity.x, .5f);
            rb.AddForce(Vector2.up * Mathf.Sqrt(-2 * jumpHeight * gravity), ForceMode2D.Impulse);
            if (inputs.hor == 0)
                state = PlayerStates.JUMP;
            else
                state = PlayerStates.JUMPMOVE;
        }
    }

    private IEnumerator CoyoteTime()
    {
        yield return new WaitForSeconds(.05f);
        canJump = false;
    }

    public void Attack()
    {

        if (inputs.GetButton("ATTACK"))
        {
            if (isGrounded)
                state = PlayerStates.ATTACK;
            else
                state = PlayerStates.AIRATTACK;
        }
    }

    public void EndAttack()
    {
        state = PlayerStates.IDLE;
    }

    public virtual void Skill()
    {
        if(inputs.GetButton("SKILL"))
        {
            if (isGrounded)
                state = PlayerStates.SKILL;
            else
                state = PlayerStates.AIRSKILL;
        }
    }

    public virtual void SkillState()
    {
        rb.velocity = new Vector2(Mathf.Lerp(rb.velocity.x, 0, (idleFriction / 2) * Time.deltaTime), rb.velocity.y);

        if (!isGrounded && anim.GetCurrentAnimatorStateInfo(0).normalizedTime < .1f)
        {
            float animTime = anim.GetCurrentAnimatorStateInfo(0).normalizedTime;
            state = PlayerStates.AIRSKILL;
            anim.Play("Player_AirSkill", 0, animTime);
        }
    }

    public virtual void AirSkillState()
    {
        if (isGrounded)
        {
            if (anim.GetCurrentAnimatorStateInfo(0).normalizedTime < .1f)
            {
                float animTime = anim.GetCurrentAnimatorStateInfo(0).normalizedTime;
                state = PlayerStates.SKILL;
                anim.Play("Player_Skill", 0, animTime);
            }
            else if (inputs.hor == 0)
                state = PlayerStates.IDLE;
            else
                state = PlayerStates.RUN;
        }

        if (!inputs.holdingJump && rb.velocity.y > .5f)
            rb.velocity += Vector2.up * gravity * highFallMultiplier * Time.deltaTime;

        if (rb.velocity.y < .5f)
            rb.velocity += Vector2.up * gravity * fallMultiplier * Time.deltaTime;
    }

    public virtual void SkillEvent()
    {
        rb.AddForce(-_transform.right * groundMaxSpd, ForceMode2D.Impulse);
        for (int x = 0; x < projectile.Length; x++)
        {
            if (!projectile[x].activeSelf)
            {
                projectile[x].SetActive(true);
                break;
            }
        }
    }

    public void AnimationManager()
    {
        anim.SetInteger("playerState", (int)state);
        anim.SetFloat("hor", Mathf.Abs(inputs.hor));
    }

    public override void OnHit(Vector2 attackerPos, float dmg)
    {
        if (state != PlayerStates.DAMAGED)
        {
            state = PlayerStates.DAMAGED;

            float xDir = transform.position.x > attackerPos.x ? 1 : -1;
            health -= dmg;

            rb.AddForce(Vector2.right * xDir * knockbackPower, ForceMode2D.Impulse);

            if(health < 0)
            {
                health = 0;
                state = PlayerStates.DEAD;
            }

            if(state != PlayerStates.DEAD)
                StartCoroutine(Hitstun(.5f));
        }       
    }

    public IEnumerator Hitstun(float time)
    {
        yield return new WaitForSeconds(time);
        state = PlayerStates.IDLE;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube((Vector2)transform.position + groundCheckOffset, groundCheckSize);
    }
}
