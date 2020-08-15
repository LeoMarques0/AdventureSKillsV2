using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    [SerializeField]
    private GameObject[] menus = null;
    [SerializeField]
    private AudioMixer musicMixer = null, soundMixer = null;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

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

    public void CallCreateRoom()
    {
        Launcher.singleton.CreateRoom();
    }

    public void Exit()
    {
        Application.Quit();
    }
}
