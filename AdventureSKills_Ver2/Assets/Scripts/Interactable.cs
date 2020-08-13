using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactable : MonoBehaviour
{

    public float health = 10;

    public virtual void OnHit(Vector2 attackerPos, float dmg)
    {
        health -= dmg;
    }
}
