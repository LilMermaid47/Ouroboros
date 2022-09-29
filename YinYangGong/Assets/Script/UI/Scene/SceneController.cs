using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{

    public void LoadScene(ScenesNames sceneName)
    {
        SceneManager.LoadScene(sceneName.SceneName.ToString());
    }

    public void RetryLevel()
    {
        string activeScene = SceneManager.GetActiveScene().name;

        SceneManager.LoadScene(activeScene);
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}