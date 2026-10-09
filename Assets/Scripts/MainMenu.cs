using UnityEngine;
using UnityEngine.SceneManagement;

// Main Menu: choose one of the three games or exit.
public class MainMenu : MonoBehaviour
{
    void Start()
    {
        Time.timeScale = 1f;
    }

    public void PlayMadDriver()
    {
        SceneManager.LoadScene("DrivingGame");
    }

    public void PlayFlyLikeABird()
    {
        SceneManager.LoadScene("FlyingGame");
    }

    public void PlaySumo()
    {
        SceneManager.LoadScene("SumoGame");
    }

    public void ExitGame()
    {
        Debug.Log("Exit selected");
#if UNITY_EDITOR
        // Stop Play Mode when running inside the Unity Editor
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
