using FMOD.Studio;
using FMODUnity;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{

    public EventReference menuIdle;
    public EventReference menuAmb;
    private EventInstance menuInstance;
    private EventInstance menuAmbInstance;

    private void Awake()
    {
        menuInstance = RuntimeManager.CreateInstance(menuIdle);
        menuAmbInstance = RuntimeManager.CreateInstance(menuAmb);
    }

    private void Start()
    {
        menuInstance.start();
        menuAmbInstance.start();
        if (GameManager.GMInstance != null)
        {
            GameManager.GMInstance._gameHasEnded = false;
        }
        

    }
    private void Update()
    {
        
    }
    public void PlayGame()
    {
        menuAmbInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
        menuAmbInstance.release();
        menuInstance.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
        menuInstance.release();

        if(AudioManager.AMInstance != null)
        {
            AudioManager.AMInstance.loopInstance.start();
            AudioManager.AMInstance.ambInstance.start();
           
        }
      
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

  
    public void QuitGame()
    {
        Debug.Log("QUIT GAME REQUESTED");
        Application.Quit();
    }
  /*  public void ToggleLoopMode()
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
  */
}