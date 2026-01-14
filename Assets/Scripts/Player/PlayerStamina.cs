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
    public float regeneRate = 8f;//← 注意: regeneRate が PlayerController で使用されます
    [Tooltip("高速走行時の消費速度（1秒あたり）")]
    public float drainRate = 20f;

    [Header("Exhaustion")]
    [Tooltip("スタミナがここ以下で疲労状態とみなす")]
    public float exhaustionThreshold = 2f;　//疲労境界線
    [Tooltip("疲労中の最低速度倍率（0～1）")]
    public float exhaustedSpeedMultiplier = 0.5f;

    [Header("UI")]
    public Slider staminaSlider; // Inspectorで割り当て

    public float currentStamina;
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
        //  UIを毎フレーム更新
        UpdateUI();
    }

    public float GetStaminaNormalized() => Mathf.Clamp01(currentStamina / maxStamina);//正規化
    //計算結果が０未満であれば0に、1を超えたら1にする。
//| currentStamina | maxStamina | 結果|
//| -------------- | ---------- | --- |
//| 100            | 100        | 1.0 |     currentStamina/maxStamina
//| 50             | 100        | 0.5 |      =      50    /    100   =0.5
//| 0              | 100        | 0.0 |      =     100    /    100   =1.0     割合に変換している。


    public float GetCurrentStamina() => currentStamina;//現在は使われていない。

    // スタミナ消費　PlayerContlloler.csでタイミングを管理。
    public void Drain(float amount)
    {
        if (amount <= 0f) return;
        currentStamina = Mathf.Max(0f, currentStamina - amount);
        //PlayerController.cs依存箇所--------------------------------------------------------------------------------
        CheckExhaustion();
        UpdateUI();
    }

    public void Regenerate(float amount)//
    {
        if (amount <= 0f) return; 
        float prev = currentStamina;//回復・消費前スタミナ
        currentStamina = Mathf.Min(maxStamina, currentStamina + amount);//MaxStaminaをCurrentStamina+amountが超えないようにスタミナ上限管理。
        if (IsExhausted && currentStamina > exhaustionThreshold)　　　　//IsExhauted＝trueだが、回復が進み境界線を越えた場合。
        {
            IsExhausted = false;
            OnRecovered?.Invoke();　//Onrecoverd（Action箱）がnullじゃなければ、まとめて実行（Invoke）
        }
        if (prev != currentStamina) UpdateUI();//回復後のスタミナとしてUpdateUI（）へ返す。
    }

    private void CheckExhaustion()　//疲労状態かどうか、境界線よりCurrentStaminaが下回った場合、IsExhauted（疲労状態）がtrueになる。
    {
        if (!IsExhausted && currentStamina <= exhaustionThreshold)
        {
            IsExhausted = true;
            OnExhausted?.Invoke();
        }
    }

    private void UpdateUI()
    {
        if (staminaSlider != null)//Sliderがnullでなければ、実行
            staminaSlider.value = GetStaminaNormalized();//Regenerate・Drainからの計算内容を正規化。
    }
}

//Awake()
//CurrentStamina＝Max
//↓
//PlayerController.cs 
//加速→Drain呼び出し　減速→Regenerate呼び出し
//↓
//UpdateUI（）
//Drain・Regenerateの計算内容を取得。
//↓
//Update（）
//計算内容を正規化
//↓
//UpdateUI（）
//PlayerStaminaUI.csが計算内容を取得しに来る。