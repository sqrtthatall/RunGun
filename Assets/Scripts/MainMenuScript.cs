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
        Debug.Log("Game closed");
        //Раскомментить при builde
        //Application.Quit();
        UnityEditor.EditorApplication.isPlaying = false;
    }
}
