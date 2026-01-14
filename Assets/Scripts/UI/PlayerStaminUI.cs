using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStaminaUI : MonoBehaviour
{
    [Header("References")]
    public PlayerStamina stamina;  // Player の PlayerStamina を Inspector で割り当て
    public Slider slider;          // UI の Slider 本体

    [Header("Color Settings")]
    public Image fillImage;        // Slider の Fill エリア
    public Color normalColor = Color.green;
    public Color exhaustedColor = Color.red;

    private void Start()
    {
        if (slider != null)
        {
            // PlayerStamina は 0～1 の正規化値を返すので Slider の Min/Max を設定
            slider.minValue = 0f;
            slider.maxValue = 1f;
        }

        // 色の初期化
        if (fillImage != null)
            fillImage.color = normalColor;
    }

    private void Update()
    {
        if (stamina == null || slider == null) return;

        // Slider の値を PlayerStamina から取得
        slider.value = stamina.GetStaminaNormalized();

        // Fill の色変更（疲労時は赤く）
        if (fillImage != null)
        {
            fillImage.color = stamina.IsExhausted ? exhaustedColor : normalColor;
        }
    }
}

