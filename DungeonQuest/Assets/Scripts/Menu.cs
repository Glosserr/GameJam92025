using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{

    public GameObject pauseMenuUI;
    bool isPaused = false;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
            {
                pauseMenuUI.SetActive(false);
                Time.timeScale = 1f;
                isPaused = false;
            }
            else
            {
                pauseMenuUI.SetActive(true);
                Time.timeScale = 0f;
                isPaused = true;
            }
        }
    }

    public void ResumeButton()
    {
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1f;
        isPaused = false;
    }

    public void OnPlayButton()
    {
        SceneManager.LoadScene(1);
    }

    public void OnMainMenuButton()
    {
        SceneManager.LoadScene(0);
    }

    public void OnQuitButton()
    {
        Application.Quit();
    }
}
