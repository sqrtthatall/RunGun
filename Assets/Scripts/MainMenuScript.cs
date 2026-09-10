using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuScript : MonoBehaviour
{
    public string firstLevelName;

    public void LoadNextLevel()
    {
        SceneManager.LoadScene(firstLevelName);
    }

    public void ExitGame()
    {
        SceneManager.LoadScene("Login");
    }
}
