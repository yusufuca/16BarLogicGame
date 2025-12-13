using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    // Called by "Play" Button
    public void PlayGame()
    {
        // Loads the next scene in the Build Index (Scene 1)
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    // Called by "Quit" Button
    public void QuitGame()
    {
        Debug.Log("QUIT GAME REQUESTED");
        Application.Quit();
    }
}