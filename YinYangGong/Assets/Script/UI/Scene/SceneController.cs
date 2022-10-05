using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
    [SerializeField]
    private GameObject MainMenu;
    [SerializeField]
    private GameObject Gym;
    [SerializeField]
    private GameObject Level;

    [SerializeField]
    private GameObject FirstMainMenuBtn;
    [SerializeField]
    private GameObject FirstGymMenuBtn;
    [SerializeField]
    private GameObject FirstLevelMenuBtn;


    private void Start()
    {
        if(MainMenu != null)
            ActivateMainMenu();
    }

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

    public void ActivateMainMenu()
    {
        MainMenu.SetActive(true);
        Gym.SetActive(false);
        Level.SetActive(false);


        EventSystem.current.SetSelectedGameObject(FirstMainMenuBtn);
        EventSystem.current.firstSelectedGameObject = FirstGymMenuBtn;
    }

    public void ActivateGymMenu()
    {
        MainMenu.SetActive(false);
        Gym.SetActive(true);
        Level.SetActive(false);


        EventSystem.current.SetSelectedGameObject(FirstGymMenuBtn);
        EventSystem.current.firstSelectedGameObject = FirstGymMenuBtn;
    }

    public void ActivateLevelMenu()
    {
        MainMenu.SetActive(false);
        Gym.SetActive(false);
        Level.SetActive(true);


        EventSystem.current.SetSelectedGameObject(FirstLevelMenuBtn);
        EventSystem.current.firstSelectedGameObject = FirstLevelMenuBtn;
    }


}