using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public enum ConnectAction
{
    CREATE,
    FIND,
    RANDOM
}

public class Launcher : MonoBehaviourPunCallbacks
{
    #region Serialized Fields
    [SerializeField]
    private Text maxPlayersText = null;
    #endregion
    #region Public Fields
    public byte maxPlayers = 4;
    public bool isPublic;

    public static Launcher singleton;
    #endregion
    #region Private Fields
    ConnectAction connectAction = new ConnectAction();

    string gameVersion = "1";
    string roomID = "";
    #endregion

    #region MonoBehaviour Callbacks

    private void Awake()
    {
        if (singleton == null)
            singleton = this;
        else if (singleton != this)
            Destroy(gameObject);

        PhotonNetwork.AutomaticallySyncScene = true;

        DontDestroyOnLoad(gameObject);
    }

    private void Update()
    {
        if(SceneManager.GetActiveScene().buildIndex == 0 && maxPlayersText != null)
            maxPlayersText.text = maxPlayers.ToString();
    }
    #endregion

    #region Public Methods

    public void FindRandomRoom()
    {
        roomID = string.Empty;
        connectAction = ConnectAction.RANDOM;

        Connect();
    }

    public void FindRoom(string _roomID)
    {
        if(_roomID == string.Empty || _roomID.Length < 5)
        {
            print("O ID " + _roomID + " é invalido");
        }
        else
        {
            roomID = _roomID;
            connectAction = ConnectAction.FIND;

            Connect();
        }
    }

    public void CreateRoom()
    {
        roomID = GenerateID.NumberOnly(5);
        connectAction = ConnectAction.CREATE;

        Connect();
    }

    public void Connect()
    {

        if (PhotonNetwork.IsConnected)
        {//Se está conectado entra em uma sala aleatóriamente
            ConnectToRoom();
        }
        else
        {//Não está conectado... cria conexão com o Photon Server
            PhotonNetwork.ConnectUsingSettings();
            PhotonNetwork.GameVersion = gameVersion;
        }
    }
    #endregion

    #region Private Methods

    private void ConnectToRoom()
    {
        switch (connectAction)
        {
            case ConnectAction.CREATE:
                PhotonNetwork.CreateRoom(roomID, new RoomOptions { MaxPlayers = maxPlayers, IsVisible = isPublic });
                break;

            case ConnectAction.FIND:
                PhotonNetwork.JoinRoom(roomID);
                break;

            case ConnectAction.RANDOM:
                PhotonNetwork.JoinRandomRoom();
                break;
        }
    }

    #endregion

    #region MonoBehaviourPunCallbacks
    public override void OnConnectedToMaster()
    {
        Debug.Log("Conectado no servidor Photon");
        ConnectToRoom();  
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        Debug.LogWarning("Desconectado. Causa: " + cause);
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        Debug.Log("Falhou ao se conectar a uma sala... Criando Sala");
        roomID = GenerateID.NumberOnly(5);
        PhotonNetwork.CreateRoom(roomID, new RoomOptions { MaxPlayers = maxPlayers });
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        Debug.Log("Falhou ao encontrar a sala de ID " + roomID);
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("Conectado na Sala: " + PhotonNetwork.CurrentRoom);

        if (PhotonNetwork.IsMasterClient)
            PhotonNetwork.LoadLevel("Lobby");
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        
    }
    #endregion
}
