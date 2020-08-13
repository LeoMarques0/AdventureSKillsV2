using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_OnlineInputs : Player_Inputs
{
    [SerializeField]
    private float inputDelay = 0.1f;

    [SerializeField]
    private List<float> earlyHor = new List<float>();
    private bool startDelay = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        earlyHor.Add(Input.GetAxisRaw("Horizontal"));

        if (!startDelay)
            StartCoroutine(RegisterHorizontal());

        if (Input.GetButtonDown("Jump"))
        {
            StartCoroutine(RegisterInput(jumpButtonName));
            Invoke("JumpHold", inputDelay);
        }
        if(Input.GetButtonUp("Jump"))
        {
            Invoke("JumpLetGo", inputDelay);
        }

        if (Input.GetButtonDown("Attack"))
        {
            StartCoroutine(RegisterInput(attackButtonName));
        }

        if (Input.GetButtonDown("Skill"))
        {
            StartCoroutine(RegisterInput(skillButtonName));
        }
    }

    private IEnumerator RegisterInput(string buttonName)
    {
        yield return new WaitForSeconds(inputDelay);

        if (buttonsPressed.Contains(buttonName))
            buttonsPressed.Remove(buttonName);

        StopCoroutine(ButtonTimer(bufferTimer, buttonName));
        StartCoroutine(ButtonTimer(bufferTimer, buttonName));
    }

    private IEnumerator RegisterHorizontal()
    {
        startDelay = true;

        yield return new WaitForSeconds(inputDelay);

        while (true)
        {
            hor = earlyHor[0];
            earlyHor.RemoveAt(0);
            yield return null;
        }
    }

    private void JumpHold()
    {
        holdingJump = true;
    }

    private void JumpLetGo()
    {
        holdingJump = false;
    }

    private IEnumerator ButtonTimer(float timer, string buttonName)
    {
        buttonsPressed.Add(buttonName);
        yield return new WaitForSecondsRealtime(timer);
        buttonsPressed.Remove(buttonName);
    }
}
