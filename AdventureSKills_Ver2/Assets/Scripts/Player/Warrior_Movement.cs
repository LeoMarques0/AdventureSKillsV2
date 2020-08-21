using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Warrior_Movement : Player_Movement
{
    public GameObject shield;
    public float shieldHealth = 100, shieldRecoverSpd = 15f, shieldMinimumSize = .1f;

    private float x, shieldMaxSize;

    public override void Awake()
    {
        base.Awake();
        x = 1 - shieldMinimumSize;
        shieldMaxSize = shield.transform.localScale.x;
    }

    public override void Update()
    {
        base.Update();
        ShieldRecorver();
    }

    public void ShieldRecorver()
    {
        if (shieldHealth < 100 && state != PlayerStates.SKILL && state != PlayerStates.AIRSKILL)
        {
            shieldHealth += shieldRecoverSpd * Time.deltaTime;
            shieldHealth = Mathf.Clamp(shieldHealth, 0, 100);

            if (shield.activeSelf)
                shield.SetActive(false);
        }
    }

    public override void SkillState()
    {
        shield.SetActive(true);
        shieldHealth -= 15 * Time.deltaTime;

        rb.velocity = new Vector2(Mathf.Lerp(rb.velocity.x, 0, idleFriction * Time.deltaTime), rb.velocity.y);

        shield.transform.localScale = Vector3.one * (((shieldHealth / 100) * x) + shieldMinimumSize) * shieldMaxSize;
        Mathf.Clamp(shieldHealth, 0, 100);

        if (!isGrounded)
            state = PlayerStates.AIRSKILL;

        if (shieldHealth <= 0)
        {
            state = PlayerStates.DAMAGED;
            StartCoroutine(Hitstun(3f));
        }

        if (!inputs.holdingSkill)
            state = PlayerStates.IDLE;

    }

    public override void AirSkillState()
    {
        shield.SetActive(true);
        shieldHealth -= 15 * Time.deltaTime;

        shield.transform.localScale = Vector3.one * (((shieldHealth / 100) * x) + shieldMinimumSize) * shieldMaxSize;
        Mathf.Clamp(shieldHealth, 0, 100);

        if (isGrounded)
            state = PlayerStates.SKILL;

        if (shieldHealth <= 0)
        {
            state = PlayerStates.DAMAGED;
            StartCoroutine(Hitstun(3f));
        }

        if (!inputs.holdingSkill)
            state = PlayerStates.IDLE;
    }

    public override void OnHit(Vector2 attackerPos, float dmg)
    {
        if(state == PlayerStates.SKILL || state == PlayerStates.AIRSKILL)
        {
            float xDir = transform.position.x > attackerPos.x ? 1 : -1;
            rb.AddForce(Vector2.right * xDir * knockbackPower, ForceMode2D.Impulse);

            shieldHealth -= dmg / 1.5f;
            if(shieldHealth <= 0)
            {
                state = PlayerStates.DAMAGED;
                StartCoroutine(Hitstun(3f));
            }        
        }
        else if (state != PlayerStates.DAMAGED)
        {
            state = PlayerStates.DAMAGED;

            float xDir = transform.position.x > attackerPos.x ? 1 : -1;
            health -= dmg;

            rb.AddForce(Vector2.right * xDir * knockbackPower, ForceMode2D.Impulse);

            if (health < 0)
            {
                health = 0;
                state = PlayerStates.DEAD;
            }

            if (state != PlayerStates.DEAD)
                StartCoroutine(Hitstun(.5f));
        }
    }
}
