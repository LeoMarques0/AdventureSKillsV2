using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

public class LobbyManager : MonoBehaviourPun
{

    public Carousel[] carousels;
    public Button readyButton;
    public Text readyButtonText;

    private bool isChoosingClass = true;
    private bool readyCountDown;
    private Carousel myCarousel;

    // Start is called before the first frame update
    void Start()
    {
        readyButton.onClick.AddListener(ChooseClass);
        Debug.Log("StartLobby");
        if (PhotonNetwork.IsConnected)
        {
            Debug.Log("StartAssign");
            AssingCarousel();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(isChoosingClass && myCarousel != null)
        {
            if (Input.GetKeyDown(KeyCode.Q))
                ChangeCarouselIndex(-1);
            if (Input.GetKeyDown(KeyCode.E))
                ChangeCarouselIndex(1);

            if(Input.GetKeyDown(KeyCode.Return))
            {
                ChooseClass();
            }
        }
        else if(GameManager.singleton.currentPlayer != null)
        {
            if(Input.GetKeyDown(KeyCode.Escape))
            {
                if (!myCarousel.ready)
                {
                    isChoosingClass = true;
                    GameManager.singleton.DeletePlayer();

                    readyButton.onClick.RemoveAllListeners();
                    readyButton.onClick.AddListener(ChooseClass);
                }
                else
                    CallReadyUp();
            }
            if (Input.GetKeyDown(KeyCode.Return))
                CallReadyUp();
        }

        if (CheckIfAllReady() && !readyCountDown)
        {
            readyCountDown = true;
            StartCoroutine(ReadyCountDown());
        }
        else if(!CheckIfAllReady() && readyCountDown)
        {
            readyCountDown = false;
            StopAllCoroutines();
        }
    }

    void ChooseClass()
    {
        isChoosingClass = false;
        GameManager.singleton.SelectCharacter(myCarousel.targetIndex, myCarousel.transform.position);
        readyButton.onClick.RemoveAllListeners();
        readyButton.onClick.AddListener(CallReadyUp);
    }

    public void AssingCarousel()
    {
        for (int x = 0; x < carousels.Length; x++)
        {
            if (x == RoomManager.singleton.playerIndex)
                myCarousel = carousels[x];
            else
            {
                foreach(Button btn in carousels[x].arrowsBtns)
                {
                    btn.interactable = false;
                }
            }
        }
        myCarousel = carousels[RoomManager.singleton.playerIndex];
        Debug.Log("CarouselChosen");
    }

    void ChangeCarouselIndex(int num)
    {
        myCarousel.CallChangeColumn(num);
    }

    bool CheckIfAllReady()
    {
        for(int i = 0; i < PhotonNetwork.CurrentRoom.PlayerCount; i++)
        {
            if (!carousels[i].ready)
                return false;
        }
        return true;
    }

    public void CallReadyUp()
    {
        for(int x = 0; x < carousels.Length; x++)
        {
            if (myCarousel == carousels[x])
            {
                photonView.RPC("ReadyUp", RpcTarget.AllBuffered, x);
                return;
            }
        }
        
    }

    [PunRPC]
    public void ReadyUp(int index)
    {
        Carousel carousel = carousels[index];

        carousel.ready = !carousel.ready;

        if (myCarousel.ready)
            readyButtonText.text = "CANCEL";
        else
            readyButtonText.text = "GO";
    }

    IEnumerator ReadyCountDown()
    {
        for (int x = 3; x >= 0; x--)
        {
            readyButtonText.text = x.ToString();
            yield return new WaitForSeconds(1);
        }
        if (PhotonNetwork.IsMasterClient)
            PhotonNetwork.LoadLevel("Level");
    }
}
