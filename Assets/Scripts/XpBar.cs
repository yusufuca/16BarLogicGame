using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class XpBar : MonoBehaviour
{
    public Slider slider;
    public TextMeshProUGUI xpText;
    public void SetMaxXP(int xp, int level)
    {
        slider.maxValue = xp;
        if (xpText != null)
        { 
            xpText.text = level.ToString();
        }
    }

    public void SetXp(int xp)
    {
        slider.value = xp;
    
    }
}
