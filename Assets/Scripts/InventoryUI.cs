using UnityEngine;
using UnityEngine.UI;

public class InventoryUI : MonoBehaviour
{
    [Header("UI Slots")]
    public Image[] slotBackgrounds; // Drag Slot_1, Slot_2, Slot_3 (Backgrounds)
    public Image[] slotIcons;       // Drag Icon_1, Icon_2, Icon_3 (The inner images)

    [Header("Colors")]
    public Color selectedColor = Color.yellow;
    public Color defaultColor = Color.gray;
    public Color emptySlotColor = new Color(0, 0, 0, 0); // Transparent

    public void UpdateUI(WeaponData[] inventory, int activeIndex)
    {
        for (int i = 0; i < inventory.Length; i++)
        {
            // 1. Update Highlight (Active Slot)
            if (i == activeIndex)
                slotBackgrounds[i].color = selectedColor;
            else
                slotBackgrounds[i].color = defaultColor;

            // 2. Update Icons
            if (inventory[i].hasWeapon)
            {
                slotIcons[i].color = inventory[i].color; // Use weapon color as icon
            }
            else
            {
                slotIcons[i].color = emptySlotColor; // Hide icon if empty
            }
        }
    }
}