using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour {

    public static bool GameIsPaused;

    public GameObject pauseMenuUI;
    public GameObject frogCrosshair;
    public GameObject mouseCrosshair;

    // Update is called once per frame
    void Update () {
		if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (GameIsPaused)
            {
                Resume();
            }
            else
            {
                Pause();
            }
        }
	}

    public void Resume()
    {
        pauseMenuUI.SetActive(false);
        frogCrosshair.SetActive(true);
        mouseCrosshair.SetActive(true);

        Time.timeScale = 1;
        GameIsPaused = false;
    }

    void Pause()
    {
        pauseMenuUI.SetActive(true);
        frogCrosshair.SetActive(false);
        mouseCrosshair.SetActive(false);

        Time.timeScale = 0;
        GameIsPaused = true;
    }

    public void QuitToMenu()
    {
        Resume();
        SceneManager.LoadScene(0);
    }
}
