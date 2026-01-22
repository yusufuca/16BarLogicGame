using TMPro;
using UnityEngine;
using UnityEngine.UI; 

public class HealthBar : MonoBehaviour
{
    public Slider slider; 
    public TextMeshProUGUI hp;


    public void SetMaxHealth(int health)
    {
        slider.maxValue = health;
        slider.value = health;
        if (hp != null)
        {
            hp.text = health.ToString();
        }
    }

    public void SetHealth(int health)
    {
        slider.value = health;
        if (hp != null)
        {
            hp.text = health.ToString();
        }
    }
}