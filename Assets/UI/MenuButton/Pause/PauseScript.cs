using UnityEngine;

public class PauseScript : MonoBehaviour
{
    public GameObject pauseMenu;
    public bool isPaused = false;

    void Start()
    {
        pauseMenu.SetActive(false);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isPaused)
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
        pauseMenu.SetActive(false);
        Time.timeScale = 1f;
        isPaused = false;
    } 

    public void Pause()
    {
        pauseMenu.SetActive(true);
        Time.timeScale = 0f;
        isPaused = true;
    }

    public void Options()
    {

    }

    public void ExitGame()
    {
        Debug.Log("Game closed");
        //Раскомментить при builde
        //Application.Quit();
        UnityEditor.EditorApplication.isPlaying = false;
    }


}
