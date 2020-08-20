using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Warrior_Network : Player_Network
{

    const string xShieldScaleKey = "XS", yShieldScaleKey = "YS";
    const string shieldActiveKey = "SA";

    private Warrior_Movement myMovement;
    private float lastShieldScaleXReceived, lastShieldScaleYReceived;
    private bool lastShieldActiveReceived;

    private float lastShieldScaleXSent, lastShieldScaleYSent;
    private bool lastShieldActiveSent;

    public override void Awake()
    {
        base.Awake();
        myMovement = GetComponent<Warrior_Movement>();
    }

    public override void Start()
    {
        base.Start();

        lastShieldActiveSent = myMovement.shield.activeSelf;
        lastShieldScaleXSent = myMovement.shield.transform.localScale.x;
        lastShieldScaleYSent = myMovement.shield.transform.localScale.y;

        lastShieldActiveReceived = myMovement.shield.activeSelf;
        lastShieldScaleXReceived = myMovement.shield.transform.localScale.x;
        lastShieldScaleYReceived = myMovement.shield.transform.localScale.y;
    }

    #region SerializeShield
    private void StoreShield()
    {
        if (myMovement.shield.activeSelf != lastShieldActiveSent)
        {
            stringToSend += shieldActiveKey + myMovement.shield.activeSelf + ";";
            lastShieldActiveSent = myMovement.shield.activeSelf;
        }

        if (myMovement.shield.transform.localScale.x != lastShieldScaleXSent)
        {
            stringToSend += xShieldScaleKey + myMovement.shield.transform.localScale.x + ";";
            lastShieldScaleXSent = myMovement.shield.transform.localScale.x;
        }

        if (myMovement.shield.transform.localScale.y != lastShieldScaleYSent)
        {
            stringToSend += yShieldScaleKey + myMovement.shield.transform.localScale.y + ";";
            lastShieldScaleYSent = myMovement.shield.transform.localScale.y;
        }
    }

    private void ReadShield()
    {
        if (stringReceived.Contains(shieldActiveKey))
        {
            int shieldActiveIndex = stringReceived.IndexOf(shieldActiveKey);
            string newActive = "";

            for (int x = shieldActiveIndex + shieldActiveKey.Length; x < stringReceived.Length; x++)
            {
                if (stringReceived[x] != ';')
                    newActive += stringReceived[x];
                else
                    break;
            }

            lastShieldActiveReceived = bool.Parse(newActive);
        }

        if (stringReceived.Contains(xShieldScaleKey))
        {
            int xShieldIndex = stringReceived.IndexOf(xShieldScaleKey);
            string xNewScale = "";

            for (int x = xShieldIndex + xShieldScaleKey.Length; x < stringReceived.Length; x++)
            {
                if (stringReceived[x] != ';')
                    xNewScale += stringReceived[x];
                else
                    break;
            }

            lastShieldScaleXReceived = float.Parse(xNewScale);
        }

        if (stringReceived.Contains(yShieldScaleKey))
        {
            int yShieldIndex = stringReceived.IndexOf(yShieldScaleKey);
            string yNewScale = "";

            for (int x = yShieldIndex + yShieldScaleKey.Length; x < stringReceived.Length; x++)
            {
                if (stringReceived[x] != ';')
                    yNewScale += stringReceived[x];
                else
                    break;
            }

            lastShieldScaleYReceived = float.Parse(yNewScale);
        }
    }

    public bool UpdateShield()
    {
        float xScale = myMovement.shield.transform.localScale.x;
        float yScale = myMovement.shield.transform.localScale.y;

        myMovement.shield.SetActive(lastShieldActiveReceived);

        if (xScale != lastShieldScaleXReceived)
            xScale = lastShieldScaleXReceived;


        if (yScale != lastShieldScaleYReceived)
            yScale = lastShieldScaleYReceived;

        myMovement.shield.transform.localScale = Vector3.Lerp(myMovement.shield.transform.localScale, new Vector3(xScale, yScale, 1), 10f * Time.deltaTime);

        return myMovement.shield.transform.localScale == new Vector3(xScale, yScale, 1);
    }
    #endregion
    #region SerializeManager
    public override void PrepareToSerialize()
    {
        base.PrepareToSerialize();
        StoreShield();
    }

    public override void ReadString()
    {
        base.ReadString();
        ReadShield();
    }

    public override void UpdatePlayer()
    {
        updatedList = new List<bool>();

        if (sharePosition)
            updatedList.Add(UpdatePosition());
        if (shareRotation)
            updatedList.Add(UpdateRotation());
        if (shareVelocity)
            updatedList.Add(UpdateVelocity());
        if (shareState)
            updatedList.Add(UpdateState());

        updatedList.Add(UpdateShield());


        if (updatedList.Count > 0)
            updated = !updatedList.Contains(false);
    }
    #endregion

}
