using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stats : MonoBehaviour
{

    private float health;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public event Action<Vector2> onHit;
    public void TakeDamage(Vector2 attackerPos, float dmg)
    {
        health -= dmg;

        if (onHit != null)
            onHit(attackerPos);
    }
}
