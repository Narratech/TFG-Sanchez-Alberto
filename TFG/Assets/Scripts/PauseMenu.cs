using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    public static bool gameIsPaused = false;

    public GameObject pauseMenuUI;
    public GameObject deathMenuUI;
    public GameObject talkMenuUI;
    public GameObject inventoryMenuUI;
    public GameObject dialogueBox;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (gameIsPaused)
            {
                Resume(pauseMenuUI);
            }
            else
            {
                Pause(pauseMenuUI);
            }
        }

        if (Input.GetKeyDown(KeyCode.T))
        {
            if (!gameIsPaused)
            {
                Pause(talkMenuUI);
            }
        }

        if (Input.GetKeyDown(KeyCode.I))
        {
            if (!gameIsPaused)
            {
                Pause(inventoryMenuUI);
            }
        }
    }

    public void Resume(GameObject menu)
    {
        menu.SetActive(false);
        Time.timeScale = 1f;
        gameIsPaused = false;
        AudioListener.pause = false;
        dialogueBox.SetActive(true);
    }

    public void Pause(GameObject menu)
    {
        menu.SetActive(true);
        Time.timeScale = 0f;
        gameIsPaused = true;
        AudioListener.pause = true;
        dialogueBox.SetActive(false);
    }

    public void LoadMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadSceneAsync(0);
    }

    public void QuitGame()
    {
        Debug.Log("Quiting game...");
        Application.Quit();
    }

    public void ResetGame()
    {
        Resume(pauseMenuUI);
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        UnsetGameIsPaused();
    }

    public void SetGameIsPaused()
    {
        gameIsPaused = true;
    }
    public void UnsetGameIsPaused()
    {
        gameIsPaused = false;
    }
}
