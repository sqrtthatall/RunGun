using UnityEngine;
using UnityEngine.SceneManagement;
public class FinishScript : MonoBehaviour
{
    public string nextLevelName;

    private void OnTriggerEnter2D(Collider2D colision)
    {
        if (colision.CompareTag("Player"))
        {
            Debug.Log("Level complete!");
            LoadNextLevel();
        }
    }
    void LoadNextLevel()
    {
        SceneManager.LoadScene(nextLevelName);
    }
}
