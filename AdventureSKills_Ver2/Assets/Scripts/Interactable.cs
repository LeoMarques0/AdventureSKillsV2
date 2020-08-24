using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Interactable : MonoBehaviourPun
{
    [SerializeField]
    private GameObject healthUI = null;

    public float health = 10, maxHealth = 10;

    public virtual void Awake()
    {
        health = maxHealth;

        SetUI();
    }

    public virtual void OnHit(Vector2 attackerPos, float dmg)
    {
        health -= dmg;
    }

    public virtual void SetUI()
    {
        if (healthUI == null)
            return;

        Canvas canvas = GameObject.Find("Canvas").GetComponent<Canvas>();
        Health_UI healthInstance = Instantiate(healthUI, canvas.transform).GetComponent<Health_UI>();

        healthInstance.SetParent(transform, photonView);
    }
}
