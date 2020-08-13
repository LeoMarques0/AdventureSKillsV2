using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mage_Movement : Player_Movement
{

    public GameObject[] attacks;

    public float healAmount, healRadius;
    public LayerMask healTargets;

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

    public override void SkillEvent()
    {
        Collider2D[] targets = Physics2D.OverlapCircleAll(_transform.position, healRadius, healTargets);

        if(targets != null)
        {
            List<Interactable> finalTargets = new List<Interactable>();

            for (int x = 0; x < targets.Length; x++)
            {
                Interactable interactable = targets[x].GetComponent<Interactable>();
               if(interactable != null)
                {
                    finalTargets.Add(interactable);
                }
            }

            if(finalTargets.Count > 0)
            {
                foreach(Interactable interactable in finalTargets)
                {
                    interactable.health += healAmount;
                }
            }
        }
    }
}
