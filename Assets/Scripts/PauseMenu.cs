using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public GameObject pausePanel;

    private Board board;

    // Start is called before the first frame update
    void Start()
    {
        board = FindObjectOfType<Board>();
        
        pausePanel.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void GoToMainMenu()
    {
        SceneManager.LoadScene("Main Menu");
    }

    public void Pause()
    {
        board.isPaused = true;
        Time.timeScale = 0;
        pausePanel.SetActive(true);
    }

    public void Unpause()
    {
        StartCoroutine(UnpauseCo());
    }

    private IEnumerator UnpauseCo()
    {
        yield return new WaitForSecondsRealtime(1);
        
        board.isPaused = false;
        Time.timeScale = 1;
        pausePanel.SetActive(false);
    }
}