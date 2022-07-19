using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameUICanvas : MonoBehaviour
{
    public TextMeshProUGUI levelTime;
    public TextMeshProUGUI levelNumber;
    public GameObject gameOverPanel;
    public GameObject gameUIPanel;

    private void Start()
    {
        gameOverPanel.SetActive(false);
    }

    public void UpdateLevelTime(float time)
    {
        int minutes = (int)(time / 60f);
        int seconds = (int)(time % 60f);
        levelTime.text = "" + minutes + ":" + seconds.ToString("D2");
    }

    public void UpdateLevelNumber(int level)
    {
        levelNumber.text = "Level " + level;
    }

    public void ShowGameOver()
    {
        gameOverPanel.SetActive(true);
        gameUIPanel.SetActive(false);
    }

    public void PlayAgain()
    {
        SceneManager.LoadScene("Game");
    }

    public void GoToMainMenu()
    {
        SceneManager.LoadScene("Main Menu");
    }
}
