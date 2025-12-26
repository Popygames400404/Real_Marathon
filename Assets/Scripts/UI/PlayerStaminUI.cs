using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStaminUI : MonoBehaviour
{
    [Header("References")]
    public PlayerStamina stamina;  // Player ÇÃ PlayerStamina
    public Slider slider;          // UI Slider

    [Header("Color Settings")]
    public Image fillImage;
    public Color normalColor = Color.green;
    public Color exhaustedColor = Color.red;

    private void Start()
    {
        if (slider != null)
        {
            // PlayerStamina ÇÕ normalizedÅi0Å`1ÅjÇ≈ê›íËÇ∑ÇÈÇÃÇ≈
            // Slider ÇÃ Min=0, Max=1 Ç≈OK
            slider.minValue = 0f;
            slider.maxValue = 1f;
        }
    }

    private void Update()
    {
        if (stamina == null || slider == null) return;

        // PlayerStamina.cs Ç™ normalized Çï‘Ç∑ÇÃÇ≈ÅAÇªÇÍÇÇªÇÃÇ‹Ç‹îΩâf
        slider.value = stamina.GetStaminaNormalized();

        if (fillImage != null)
        {
            fillImage.color = stamina.IsExhausted ? exhaustedColor : normalColor;
        }
    }
}
