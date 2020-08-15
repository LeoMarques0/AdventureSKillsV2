using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{

    public static GameManager singleton;

    public GameObject[] characters;
    public GameObject myCharacter;
    public GameObject currentPlayer;

    public int targetFrameRate = 60;

    // Start is called before the first frame update
    void Awake()
    {
        if (singleton == null)
            singleton = this;
        else if (singleton != this)
            Destroy(gameObject);

        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SelectCharacter(int index, Vector2 startPos)
    {
        myCharacter = characters[index];

        if(PhotonNetwork.IsConnected)
        {
            currentPlayer = PhotonNetwork.Instantiate(myCharacter.name, startPos, Quaternion.identity);
        }
    }

    public void DeletePlayer()
    {
        if (PhotonNetwork.IsConnected)
            PhotonNetwork.Destroy(currentPlayer);
        else
            Destroy(currentPlayer);
    }
}
