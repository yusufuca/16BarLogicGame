using UnityEngine;
using UnityEngine.UI;

public class ScreenStateEffects : MonoBehaviour
{
    [Header("UI Reference")]
    public Image vignetteImage;

    [Header("State Colors")]
    public Color activeCombatColor = new Color(1f, 0f, 0f, 0.4f);  // Kırmızı (Savaş)
    public Color spawnerColor = new Color(1f, 0.5f, 0f, 0.3f);     // Turuncu (Spawner'a yakın, tedirgin)
    public Color idleColor = new Color(0f, 1f, 0f, 0.15f);         // Hafif Yeşil (Güvenli)
    public Color defaultColor = new Color(0f, 0f, 0f, 0f);         // Tamamen Şeffaf (Explore, Epic vb.)

    [Header("Settings")]
    public float fadeSpeed = 3f;

    private Color targetColor;

    void Start()
    {
        if (vignetteImage == null) vignetteImage = GetComponent<Image>();

        if (vignetteImage != null)
        {
            vignetteImage.color = defaultColor;
            targetColor = defaultColor;
        }
    }

    void Update()
    {
        if (AudioManager.AMInstance == null || vignetteImage == null) return;

        string currentState = AudioManager.AMInstance.currentState;

        if (currentState == "Combat")
        {
            // AudioManager'daki private isCombatActive() mantığını public değişkenlerle simüle ediyoruz.
            // Eğer son saldırıdan bu yana combatCooldown süresi geçmediyse, gerçekten savaştayız demektir.
            bool isReallyInCombat = Time.time < (AudioManager.AMInstance.lastCombatTriggerTimer + AudioManager.AMInstance.combatCooldown);

            if (isReallyInCombat)
            {
                targetColor = activeCombatColor; // Aktif savaşta ekran kırmızı olur
            }
            else if (AudioManager.AMInstance.isSpawnerThere)
            {
                targetColor = spawnerColor;      // Savaş bitti ama spawner hala aktifse/yakınsa turuncu olur
            }
            else
            {
                targetColor = activeCombatColor; // Fallback
            }
        }
        else if (currentState == "Idle")
        {
            targetColor = idleColor;             // Boşta kalınca yeşil
        }
        else
        {
            targetColor = defaultColor;          // Explore gibi diğer durumlarda efekt kaybolur
        }

        // Rengi aniden değil, yumuşak bir geçişle (fade) uygula
        vignetteImage.color = Color.Lerp(vignetteImage.color, targetColor, Time.deltaTime * fadeSpeed);
    }
}