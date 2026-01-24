using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonUpdater : MonoBehaviour
{
   public void ResumeGame()
    {
        GameManager.GMInstance.ResumeGame();
    }
    public void LoadMenu()
    {
        GameManager.GMInstance.LoadMenu();
    }
    public void QuitGame()
    {
        GameManager.GMInstance.QuitGame();  
    }


}
