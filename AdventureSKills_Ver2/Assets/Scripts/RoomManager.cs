using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class RoomManager : MonoBehaviourPun
{

    public static RoomManager singleton;

    [HideInInspector]
    public int playerIndex;

    // Start is called before the first frame update
    void Awake()
    {
        if (singleton == null)
            singleton = this;
        else if (singleton != this)
            Destroy(gameObject);

        DontDestroyOnLoad(gameObject);

        SceneManager.activeSceneChanged += CheckOnlineStatus;

        SetPlayerIndex();
    }

    private void SetPlayerIndex()
    {
        for (int x = 0; x < PhotonNetwork.CurrentRoom.MaxPlayers; x++)
        {
            print(PhotonNetwork.LocalPlayer.ActorNumber);
            if (PhotonNetwork.CurrentRoom.CustomProperties["Player" + x] == null || !PlayerIsOnRoom((string)PhotonNetwork.CurrentRoom.CustomProperties["Player" + x]))
            {         
                string playerID = PhotonNetwork.LocalPlayer.ActorNumber.ToString();
                Hashtable hash = new Hashtable();

                hash.Add("Player" + x, playerID);
                PhotonNetwork.CurrentRoom.SetCustomProperties(hash);

                playerIndex = x;
                Debug.Log("Index Set");
                return;
            }
        }
        Debug.LogError("Failed to set player index");
    }

    private void CheckOnlineStatus(Scene previousScene, Scene currentScene)
    {
        if(!PhotonNetwork.InRoom)
        {
            SceneManager.activeSceneChanged -= CheckOnlineStatus;
            Destroy(gameObject);
        }
    }

    private bool PlayerIsOnRoom(string id)
    {
        foreach(Player p in PhotonNetwork.PlayerList)
        {
            if (p.ActorNumber == int.Parse(id))
                return true;
        }
        return false;
    }
}
