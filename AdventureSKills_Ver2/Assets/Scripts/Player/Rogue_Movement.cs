using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rogue_Movement : Player_Movement
{

    private bool invisibility;
    private float invisibilityTime = 100;

    public override void Update()
    {
        base.Update();

        if (!invisibility && invisibilityTime < 100)
        {
            invisibilityTime += 15f * Time.deltaTime;
            invisibilityTime = Mathf.Clamp(invisibilityTime, 0, 100);

            if(sprites[0].color.a < 1)
                InvisibilityOff();
        }
    }

    public override void Skill()
    {
        if (inputs.GetButton("SKILL"))
        {
            invisibility = !invisibility;

            if (invisibility)
                StartCoroutine(TurnInvisible());
            else
            {
                StopCoroutine("TurnInvisible");

                InvisibilityOff();
            }
        }
    }

    IEnumerator TurnInvisible()
    {

        while (invisibilityTime > 0)
        {
            foreach (SpriteRenderer sr in sprites)
            {
                Color srColor = sr.color;

                srColor.a -= Time.deltaTime;
                srColor.a = Mathf.Clamp(srColor.a, Mathf.Abs(rb.velocity.magnitude) < 1 ? 0 : .5f, 100);

                sr.color = srColor;
            }

            invisibilityTime -= 10 * Time.deltaTime;
            yield return null;
        }

        invisibilityTime = 0;
        invisibility = false;
    }

    private void InvisibilityOff()
    {
        foreach (SpriteRenderer sr in sprites)
        {
            Color srColor = sr.color;

            srColor.a = 1;
            sr.color = srColor;
        }
    }
}
