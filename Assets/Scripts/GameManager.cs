using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("UI References")]
    public GameObject gameOverUI;
    public GameObject victoryUI;
    public GameObject pauseMenuUI; // NEW

    public bool isPaused = false;
    private bool _gameHasEnded = false;

    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    void Update()
    {
        // Toggle Pause with ESC key
        if (Input.GetKeyDown(KeyCode.Escape) && !_gameHasEnded)
        {
            if (isPaused) ResumeGame();
            else PauseGame();
        }
    }

    public void ResumeGame()
    {
        if (pauseMenuUI != null) pauseMenuUI.SetActive(false);
        Time.timeScale = 1f; // Unfreeze time
        isPaused = false;
        // Lock cursor again
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void PauseGame()
    {
        if (pauseMenuUI != null) pauseMenuUI.SetActive(true);
        Time.timeScale = 0f; // Freeze time
        isPaused = true;
        // Unlock cursor so we can click buttons
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void LoadMenu()
    {
        Time.timeScale = 1f; // Reset time before leaving
        SceneManager.LoadScene(0); // Load Main Menu (Index 0)
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    // ... (Keep GameOver, Victory, RestartGame methods same as before) ...
    public void GameOver()
    {
        if (_gameHasEnded) return;
        _gameHasEnded = true;
        if (gameOverUI != null) gameOverUI.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        Invoke("RestartGame", 3.0f);
    }

    public void Victory()
    {
        if (_gameHasEnded) return;
        _gameHasEnded = true;
        if (victoryUI != null) victoryUI.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}