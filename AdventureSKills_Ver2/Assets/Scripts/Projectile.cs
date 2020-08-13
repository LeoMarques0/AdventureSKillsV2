using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Projectile : Hitbox
{
    public float force = 50f, disableTimer = 1f, gravityScale = 1f;
    private Rigidbody2D rb;

    public override void Awake()
    {
        base.Awake();
        transform.parent = null;
        rb = GetComponent<Rigidbody2D>();
        gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        transform.position = root.position;
        rb.gravityScale = gravityScale;
        rb.velocity = root.right * force;
        StartCoroutine(DisableProjectile(disableTimer));
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        collision.TryGetComponent(out Interactable hit);

        if (hit != null && !IsAnException(hit.transform))
        {
            hit.OnHit(root.position, dmg);
            rb.velocity = Vector2.zero;
            rb.gravityScale = 0;
            transform.parent = hit.transform;
        }
    }

    IEnumerator DisableProjectile(float timer)
    {
        yield return new WaitForSeconds(timer);
        transform.parent = null;
        gameObject.SetActive(false);
    }
}
