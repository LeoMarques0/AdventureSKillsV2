using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Health_UI : MonoBehaviour
{

    private Camera mainCam;
    private Canvas canvas;
    [SerializeField]
    private Text nameText = null;
    [SerializeField]
    private Slider healthSlider = null;
    private Transform playerTransform;
    private Vector2 healthPos;

    [SerializeField]
    private float heightOffset = 0;
    [SerializeField]
    private Vector2 healthOffset = new Vector2(0, 5);

    Interactable main;

    // Start is called before the first frame update
    void Start()
    {
        print("Start");
        mainCam = Camera.main;

        canvas = GameObject.Find("Canvas").GetComponent<Canvas>();
        transform.SetParent(canvas.transform);
    }

    // Update is called once per frame
    void Update()
    {
        print("Update");
        if (main == null)
        {
            Destroy(gameObject);
            return;
        }
        healthSlider.value = main.health;
    }

    private void LateUpdate()
    {
        if (main == null)
        {
            Destroy(gameObject);
            return;
        }
        healthPos = playerTransform.position;
        healthPos.y += heightOffset;

        if (canvas.renderMode == RenderMode.ScreenSpaceCamera)
        {
            transform.position = healthPos + healthOffset;
        }
        else if (canvas.renderMode == RenderMode.ScreenSpaceOverlay)
        {
            if (mainCam == null)
                mainCam = FindObjectOfType<Camera>();
            transform.position = (Vector2)mainCam.WorldToScreenPoint(healthPos) + healthOffset;
        }
    }

    public void SetParent(Transform _main, PhotonView _mainNetwork)
    {
        print("SetParent");
        main = _main.GetComponent<Interactable>();

        if (_mainNetwork != null && _main.tag == "Player")
            playerTransform = _mainNetwork.transform;
        else
        {
            playerTransform = _main;
            nameText.text = "";
        }

        mainCam = FindObjectOfType<Camera>();

        healthSlider.maxValue = main.maxHealth;
    }
}
