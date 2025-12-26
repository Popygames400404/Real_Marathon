using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;

public class PlayerStamina : MonoBehaviour
{
    [Header("Stamina Settings")]
    public float maxStamina = 100f;
    [Tooltip("自然回復速度（1秒あたり）")]
    public float regeneRate = 8f;
    [Tooltip("高速走行時の消費速度（1秒あたり）")]
    public float drainRate = 20f;

    [Header("Exhaustion")]
    [Tooltip("スタミナがここ以下で疲労状態と見なす")]
    public float exhaustionThreshold = 2f;
    [Tooltip("疲労中の最低速度倍率（0～1）")]
    public float exhaustedSpeedMultiplier = 0.5f;

    [Header("UI")]
    public Slider staminaSlider; // Inspectorで割当て

    private float currentStamina;
    public bool IsExhausted { get; private set; }

    public event Action OnExhausted;      // 疲労発生時イベント
    public event Action OnRecovered;      // 回復して疲労解除時イベント

    void Awake()
    {
        currentStamina = maxStamina;
        UpdateUI();
    }

    void Update()
    {
        // UIを毎フレーム更新（UI負荷は小さいのでOK）
        UpdateUI();
    }

    public float GetStaminaNormalized() => Mathf.Clamp01(currentStamina / maxStamina);

    public float GetCurrentStamina() => currentStamina;

    // drainAmountは秒あたりの消費量ではなく、既に *Time.deltaTime* がかかった後の値を想定して渡すのも可。
    public void Drain(float amount)
    {
        if (amount <= 0f) return;
        currentStamina = Mathf.Max(0f, currentStamina - amount);
        CheckExhaustion();
        UpdateUI();
    }

    public void Regenerate(float amount)//スタミナ回復
    {
        if (amount <= 0f) return;
        float prev = currentStamina;
        currentStamina = Mathf.Min(maxStamina, currentStamina + amount);
        if (IsExhausted && currentStamina > exhaustionThreshold)
        {
            IsExhausted = false;
            OnRecovered?.Invoke();
        }
        if (prev != currentStamina) UpdateUI();
    }

    private void CheckExhaustion()
    {
        if (!IsExhausted && currentStamina <= exhaustionThreshold)
        {
            IsExhausted = true;
            OnExhausted?.Invoke();
        }
    }

    private void UpdateUI()
    {
        if (staminaSlider != null)
            staminaSlider.value = GetStaminaNormalized();
    }
}
