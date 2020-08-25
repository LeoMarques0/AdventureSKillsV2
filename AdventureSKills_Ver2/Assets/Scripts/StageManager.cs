using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StageManager : MonoBehaviour
{
    [SerializeField]
    private Transform[] playersSpawns = null;

    public Text playerList;

    // Start is called before the first frame update
    void Awake()
    {
        int playerIndex = 1;
        PhotonNetwork.Instantiate(GameManager.singleton.myCharacter.name, playersSpawns[playerIndex].position, Quaternion.identity);
    }

    // Update is called once per frame
    void Update()
    {
        print("number of players: " + PhotonNetwork.PlayerList.Length.ToString());
        for(int x = 0; x < PhotonNetwork.PlayerList.Length; x++)
        {
            print("Player " + x + ": " + PhotonNetwork.PlayerList[x].UserId);
        }
        
    }
}
