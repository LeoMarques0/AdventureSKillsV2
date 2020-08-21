using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rogue_Network : Player_Network
{
    const string invisibilityAlphaKey = "AK";

    private SpriteRenderer baseSprite;
    
    private float lastAlphaSent;
    private float lastAlphaReceived;

    public override void Awake()
    {
        base.Awake();

        baseSprite = movement.sprites[0];
    }

    public override void Start()
    {
        base.Start();

        lastAlphaReceived = baseSprite.color.a;
        lastAlphaSent = baseSprite.color.a;
    }

    #region SerializeInvisibility
    private void StoreInvisibility()
    {
        if(baseSprite.color.a != lastAlphaSent)
        {
            stringToSend += invisibilityAlphaKey + baseSprite.color.a + ";";
            lastAlphaSent = baseSprite.color.a;
        }
    }

    private void ReadInvisibility()
    {
        if(stringReceived.Contains(invisibilityAlphaKey))
        {
            int alphaIndex = stringReceived.IndexOf(invisibilityAlphaKey);
            string newAlpha = "";

            for(int x = alphaIndex + invisibilityAlphaKey.Length; x < stringReceived.Length; x++)
            {
                if (stringReceived[x] != ';')
                    newAlpha += stringReceived[x];
                else
                    break;
            }

            lastAlphaReceived = float.Parse(newAlpha);
        }
    }

    private bool UpdateInvisibility()
    {
        foreach(SpriteRenderer sprite in movement.sprites)
        {
            Color spriteColor = sprite.color;
            spriteColor.a = lastAlphaReceived;
            sprite.color = spriteColor;
        }

        return true;
    }
    #endregion

    public override void PrepareToSerialize()
    {
        base.PrepareToSerialize();
        StoreInvisibility();
    }

    public override void ReadString()
    {
        base.ReadString();
        ReadInvisibility();
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

        updatedList.Add(UpdateInvisibility());


        if (updatedList.Count > 0)
            updated = !updatedList.Contains(false);
    }
}
