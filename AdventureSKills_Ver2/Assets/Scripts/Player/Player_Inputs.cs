using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Inputs : MonoBehaviour
{
    public string jumpButtonName = "JUMP", attackButtonName = "ATTACK", skillButtonName = "SKILL";
    [HideInInspector]
    public bool holdingJump = false, holdingSkill = false;
    public float bufferTimer = .3f;

    public List<string> buttonsPressed = new List<string>();
    public float hor;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        hor = Input.GetAxisRaw("Horizontal");

        holdingJump = Input.GetButton("Jump");
        holdingSkill = Input.GetButton("Skill");

        if (Input.GetButtonDown("Jump"))
        {
            if (buttonsPressed.Contains(jumpButtonName))
                buttonsPressed.Remove(jumpButtonName);

            StopCoroutine(ButtonTimer(bufferTimer, jumpButtonName));
            StartCoroutine(ButtonTimer(bufferTimer, jumpButtonName));
        }

        if (Input.GetButtonDown("Attack"))
        {
            if (buttonsPressed.Contains(attackButtonName))
                buttonsPressed.Remove(attackButtonName);

            StopCoroutine(ButtonTimer(bufferTimer, attackButtonName));
            StartCoroutine(ButtonTimer(bufferTimer, attackButtonName));
        }

        if (Input.GetButtonDown("Skill"))
        {
            if (buttonsPressed.Contains(skillButtonName))
                buttonsPressed.Remove(skillButtonName);

            StopCoroutine(ButtonTimer(bufferTimer, skillButtonName));
            StartCoroutine(ButtonTimer(bufferTimer, skillButtonName));
        }
    }

    private IEnumerator ButtonTimer(float timer, string buttonName)
    {
        buttonsPressed.Add(buttonName);
        yield return new WaitForSecondsRealtime(timer);
        buttonsPressed.Remove(buttonName);
    }

    public bool GetButton(string buttonName)
    {
        if (buttonsPressed.Count > 0 && buttonsPressed[0] == buttonName)
        {
            StopCoroutine(ButtonTimer(bufferTimer, buttonName));
            buttonsPressed.Remove(buttonName);
            return true;
        }
        else return false;
    }
}
