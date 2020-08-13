using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IgnoreScripts : MonoBehaviourPun
{

    public MonoBehaviour[] scriptsToIgnore;

    // Start is called before the first frame update
    void Start()
    {
        if (!photonView.IsMine)
            TurnScriptsOff();
    }

    void TurnScriptsOff()
    {
        foreach (MonoBehaviour script in scriptsToIgnore)
            script.enabled = false;
    }
}
