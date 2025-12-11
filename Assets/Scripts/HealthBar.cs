using UnityEngine;
using UnityEngine.UI; // Required for UI components

public class HealthBar : MonoBehaviour
{
    public Slider slider; // Drag the Slider component here

    public void SetMaxHealth(int health)
    {
        slider.maxValue = health;
        slider.value = health;
    }

    public void SetHealth(int health)
    {
        slider.value = health;
    }
}