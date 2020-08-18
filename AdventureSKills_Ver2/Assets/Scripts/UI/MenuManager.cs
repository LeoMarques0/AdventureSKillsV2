using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    [SerializeField]
    private GameObject[] menus = null;
    [SerializeField]
    private AudioMixer musicMixer = null, soundMixer = null;
    [SerializeField]
    private Text maxPlayersText = null;
    [SerializeField]
    private string roomToSearch = "";

    public void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    public void LoadScene(int sceneBuildIndex)
    {
        SceneManager.LoadScene(sceneBuildIndex);
    }

    public void ChangeMenu(GameObject menuToLoad)
    {
        foreach(GameObject menu in menus)
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

    public void ChangeMaxPlayers(int x)
    {
        Launcher.singleton.maxPlayers += (byte)x;
        Launcher.singleton.maxPlayers = (byte)Mathf.Clamp(Launcher.singleton.maxPlayers, 2, 4);

        maxPlayersText.text = Launcher.singleton.maxPlayers.ToString();
    }

    public void SearchRoomText(string newValue)
    {
        roomToSearch = newValue;
    }

    public void CallCreateRoom()
    {
        Launcher.singleton.CreateRoom();
    }

    public void CallFindRoom()
    {
        if(roomToSearch != "")
            Launcher.singleton.FindRoom(roomToSearch);
    }

    public void CallFindRandomRoom()
    {
        Launcher.singleton.FindRandomRoom();
    }

    public void ChangeRoomName(string newValue)
    {
        Launcher.singleton.roomID = newValue;
    }

    public void IsPrivate(bool newValue)
    {
        Launcher.singleton.isPublic = newValue;
    }

    public void Exit()
    {
        Application.Quit();
    }
}
