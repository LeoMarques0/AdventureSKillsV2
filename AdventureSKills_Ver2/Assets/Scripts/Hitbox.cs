using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hitbox : MonoBehaviour
{

    public float dmg = 3f;

    [HideInInspector]
    public Transform root;
    private Transform[] exceptions;

    public virtual void Awake()
    {
        root = transform.root;
        exceptions = transform.root.gameObject.GetComponentsInChildren<Transform>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Interactable hit;
        collision.TryGetComponent(out hit);

        if(hit != null && !IsAnException(hit.transform))
        {
            hit.OnHit(root.position, dmg);
        }
    }

    public bool IsAnException(Transform hit)
    {
        for(int x = 0; x < exceptions.Length; x++)
        {
            if (hit == exceptions[x])
                return true;
        }
        return false;
    }
}
