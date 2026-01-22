using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public TextMeshProUGUI toggleText;

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
    public void ToggleLoopMode()
    {
       GameManager.GMInstance.isLinear = !GameManager.GMInstance.isLinear;
        toggleText.text = "Linear";
        if(GameManager.GMInstance.isLinear)
        {
            toggleText.text = "Linear";
        }
        else
        {
            toggleText.text = "Loop";
        }
    }
}