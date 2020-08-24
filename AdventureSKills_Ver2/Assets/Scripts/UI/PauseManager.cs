using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

public class PauseManager : MonoBehaviour
{

    [SerializeField]
    private GameObject[] menus = null;
    [SerializeField]
    private GameObject pause = null;
    [SerializeField]
    private AudioMixer musicMixer = null, soundMixer = null;

    private Player_Inputs playerInputs;
    private PhotonView playerView = null;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && Time.timeScale > 0)
            Pause();
    }

    public void Pause()
    {
        if (playerView == null)
            Time.timeScale = 0f;

        playerInputs.enabled = false;
        pause.SetActive(true);
    }

    public void Resume()
    {
        if (playerView == null)
            Time.timeScale = 1f;

        playerInputs.enabled = true;
        pause.SetActive(false);
    }

    public void ChangeMenu(GameObject menuToLoad)
    {
        foreach (GameObject menu in menus)
            menu.SetActive(false);

        menuToLoad.SetActive(true);
    }

    public void ChangeSoundVolume(float sliderValue)
    {
        soundMixer.SetFloat("SoundVol", Mathf.Log10(sliderValue) * 20);
    }

    public void ChangeMusicVolume(float sliderValue)
    {
        musicMixer.SetFloat("MusicVol", Mathf.Log10(sliderValue) * 20);
    }

    public void CallExitRoom()
    {
        if (playerView != null)
            Launcher.singleton.CallLeaveRoom();
        else
            SceneManager.LoadScene("Menu");
    }

    public void SetParent(Transform _main, PhotonView _mainNetwork)
    {
        playerInputs = _main.GetComponent<Player_Inputs>();
        transform.SetParent(FindObjectOfType<Canvas>().transform);

        if (_mainNetwork != null) 
            playerView = _mainNetwork;


    }
}
