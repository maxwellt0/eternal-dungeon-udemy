using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    private AudioManager audioManager;
    private GameProperties gameProperties;

    public GameObject confirmationPanel;
    public GameObject mainMenuPanel;
    
    void Start()
    {
        audioManager = FindObjectOfType<AudioManager>();
        gameProperties = FindObjectOfType<GameProperties>();
        
        audioManager.PlayMenuMusic();
        confirmationPanel.SetActive(false);
    }

    public void ContinueGame()
    {
        SceneManager.LoadScene("Game");
    }

    public void StartNewGame()
    {
        confirmationPanel.SetActive(true);
        mainMenuPanel.SetActive(false);
    }

    public void ExitGame()
    {
        Application.Quit();
    }

    public void ConfirmNewGame()
    {
        gameProperties.ResetLastLevel();
        SceneManager.LoadScene("Game");
    }

    public void CancelNewGame()
    {
        confirmationPanel.SetActive(false);
        mainMenuPanel.SetActive(true);
    }
    
}
