using UnityEngine;
using TMPro;

public class DebugUIRegister : MonoBehaviour
{
    [Header("UI")]
    public TextMeshProUGUI myStatesText;
    public TextMeshProUGUI myTimerText;
    public GameObject gameOverUI;
    public GameObject victoryUI;
    public GameObject pauseMenuUI;



    void Start()
    {
      
        if (AudioManager.AMInstance != null)
        {
            AudioManager.AMInstance.RegisterDebugUI(myStatesText, myTimerText);
        }
        if(GameManager.GMInstance != null)
        {
            GameManager.GMInstance.RegisterDebugUI(gameOverUI,victoryUI,pauseMenuUI);
        }
    }
}