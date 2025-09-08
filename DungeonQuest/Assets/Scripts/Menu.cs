using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{
    public GameObject More;

    public void OnPlayButton()
    {
        SceneManager.LoadScene(2);
    }

    public void OnMainMenuButton()
    {
        SceneManager.LoadScene(0);
    }

    public void MoreButton()
    {
        More.SetActive(true);
    }

    public void MoreButtonOff()
    {
        More.SetActive(false);
    }

    public void OnQuitButton()
    {
        Application.Quit();
    }
}
