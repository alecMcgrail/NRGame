using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

//Ninja Runner Main Menu class
//Basic main menu, flesh it out later

public class MainMenu : MonoBehaviour {
    public UnityEngine.UI.Button playButton;

    public void Start()
    {
        playButton.Select();
    }
    public void PlayGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void QuitGame()
    {
        Debug.Log("quit");
        Application.Quit();
    }
}
