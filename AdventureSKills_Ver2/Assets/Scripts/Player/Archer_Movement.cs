using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Archer_Movement : Player_Movement
{

    public GameObject[] attacks;

    private bool canDoubleJump = false;

    public void Shoot()
    {
        rb.AddForce(-_transform.right * groundMaxSpd, ForceMode2D.Impulse);
        for (int x = 0; x < attacks.Length; x++)
        {
            if (!attacks[x].activeSelf)
            {
                attacks[x].SetActive(true);
                break;
            }
        }
    }

    public override void Skill()
    {
        if (inputs.GetButton("JUMP") && !isGrounded && canDoubleJump)
        {
            canDoubleJump = false;
            rb.velocity = new Vector2(rb.velocity.x, .5f);
            rb.AddForce(Vector2.up * Mathf.Sqrt(-2 * jumpHeight * gravity), ForceMode2D.Impulse);
            if (inputs.hor == 0)
                state = PlayerStates.JUMP;
            else
                state = PlayerStates.JUMPMOVE;
        }
    }

    public override void CollisionsCheck()
    {
        base.CollisionsCheck();

        if (isGrounded && !canDoubleJump && (state == PlayerStates.IDLE || state == PlayerStates.RUN))
            canDoubleJump = true;
    }

    public override void AnimationManager()
    {
        base.AnimationManager();
        anim.SetBool("DoubleJump", !canDoubleJump);
    }
}
