using UnityEngine;
using UnityEngine.SceneManagement;


public class GameManager : MonoBehaviour
{
    public static GameManager GMInstance;

    void Awake()
    {

        if (GMInstance != null && GMInstance != this)

        {

            Destroy(gameObject);

        }

        else

        {

            GMInstance = this;

            DontDestroyOnLoad(gameObject);

        }
    }



    [Header("UI References")]
    public GameObject gameOverUI;
    public GameObject victoryUI;
    public GameObject pauseMenuUI; 

    public bool isPaused = false;
    public bool isLinear = true;
    public float gameRestartTimer = 16;
    public bool _gameHasEnded = false;

    public GameObject texts;



    void Update()
    {
        
        if (Input.GetKeyDown(KeyCode.Escape) && !_gameHasEnded)
        {
            if (isPaused) ResumeGame();
            else PauseGame();
        }
        if (Input.GetKeyDown(KeyCode.Tab)) texts.SetActive(!texts.activeSelf);
        if (!_gameHasEnded)
        {
            if (AudioManager.AMInstance.currentState == "Win" || AudioManager.AMInstance.currentState == "Die")
            {
                _gameHasEnded = true;
                Invoke("RestartGame", gameRestartTimer);
               

            }
            
        }
    }
    public void RegisterDebugUI(GameObject gameOver, GameObject victory, GameObject pauseMenu)
    {
        gameOverUI = gameOver;
        victoryUI = victory;
        pauseMenuUI = pauseMenu;    
    }

    public void ResumeGame()
    {
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1f; // Unfreeze time
        isPaused = false;
        // Lock cursor again
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void PauseGame()
    {
        pauseMenuUI.SetActive(true);
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
        AudioManager.AMInstance.ambInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
        AudioManager.AMInstance.ambInstance.release();
        AudioManager.AMInstance.loopInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
        AudioManager.AMInstance.loopInstance.release();
        if (AudioManager.AMInstance != null && GameManager.GMInstance != null)
        {
            if (GameManager.GMInstance._gameHasEnded)
            {

                //  AudioManager.AMInstance.linearInstance.release();
                // AudioManager.AMInstance.transitionInstance.release();

                AudioManager.AMInstance.queuedState = "Explore";
            }
        }
    }

    public void QuitGame()
    {
        Application.Quit();
    }

   
    public void GameOver()
    {
      
        if (_gameHasEnded) return;
       
        if (gameOverUI != null) gameOverUI.SetActive(true);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        


    }

    public void Victory()
    {
        
        if (_gameHasEnded) return;
       
        if (victoryUI != null) victoryUI.SetActive(true);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
     
    }

    void RestartGame()
    {
     
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex -1);

        
        if (AudioManager.AMInstance != null)
        {
            if (_gameHasEnded)
            {

                //  AudioManager.AMInstance.linearInstance.release();
                // AudioManager.AMInstance.transitionInstance.release();

                AudioManager.AMInstance.isVictory = false;
                AudioManager.AMInstance.isPlayerDeath = false;
                AudioManager.AMInstance.setEpicState = false;
                AudioManager.AMInstance.setAnxietyState = false;
                AudioManager.AMInstance.queuedState = "Explore";
                AudioManager.AMInstance.ambInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
                AudioManager.AMInstance.ambInstance.release();
                AudioManager.AMInstance.loopInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
                AudioManager.AMInstance.loopInstance.release();
              
               
            }
        }
        _gameHasEnded = false;
        Cursor.lockState= CursorLockMode.None;
        Cursor.visible= true;
    }
}