using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    private AudioManager audioManager;
    
    void Start()
    {
        audioManager = FindObjectOfType<AudioManager>();
        
        audioManager.PlayMenuMusic();
    }

    public void ContinueGame()
    {
        SceneManager.LoadScene("Game");
    }

    public void StartNewGame()
    {
        SceneManager.LoadScene("Game");
    }

    public void ExitGame()
    {
        Application.Quit();
    }
    
}
