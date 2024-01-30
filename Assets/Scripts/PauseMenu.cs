using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseMenu : MonoBehaviour
{
    public GameObject pauseScreenUI;

    public static bool gameIsPaused = false;

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            if(gameIsPaused )
            {
                Resume();
            }
            else
            {
                Pause();
            }
        }
    }

    private void Pause()
    {
        pauseScreenUI.SetActive(true);
        gameIsPaused = true;
    }

    private void Resume()
    {
        pauseScreenUI.SetActive(false);
        gameIsPaused = false;
    }
}
