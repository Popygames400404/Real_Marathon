using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

// Rigidbody2D を必ずアタッチさせる宣言
[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    [Header("Speed Settings")]
    [Tooltip("通常・最低速度（これより遅くならない）")]
    public float baseSpeed = 2f;
    [Tooltip("加速時の最高速度")]
    public float maxSpeed = 6f;
    [Tooltip("Dキーでの加速度（単位：速度/秒）")]
    public float acceleration = 4f;
    [Tooltip("Aキーでの減速度（単位：速度/秒）")]
    public float deceleration = 2f;

    [Header("Stamina / スタミナ")]
    [Tooltip("PlayerStamina スクリプトを Inspector で割当てる")]
    public PlayerStamina stamina;                 // ← 変更部分：PlayerStamina に統合
    [Tooltip("スタミナ消費倍率")]
    public float staminaDrainMultiplier = 1f;     // ← 変更部分：PlayerStamina と連携

    [Header("Controls")]
    public KeyCode accelKey = KeyCode.D;
    public KeyCode decelKey = KeyCode.A;
    public KeyCode jamKey = KeyCode.J;            // 将来ジャマキー用

    [Header("Behavior")]
    public bool applyVelocityInFixed = true;

    private Rigidbody2D rb;
    private float nextFrameTargetSpeed;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0f;
        rb.freezeRotation = true;

        rb.velocity = new Vector2(baseSpeed, 0f);
        nextFrameTargetSpeed = baseSpeed;
    }

    void Update()
    {
        // ===== スタミナ自然回復 =====
        if (stamina != null)
        {
            // Dキーを押していない時だけ回復
            if (!Input.GetKey(accelKey))
            {
                stamina.Regenerate(stamina.regeneRate * Time.deltaTime);
            }
        }


        float computedSpeed = ComputeSpeedFromInput();

        if (applyVelocityInFixed)
            nextFrameTargetSpeed = computedSpeed;
        else
            rb.velocity = new Vector2(computedSpeed, 0f);

        HandleJamInput();
        Debug.Log("Speed: " + rb.velocity.x);

    }

    void FixedUpdate()
    {
        if (applyVelocityInFixed)
        {
            rb.velocity = new Vector2(nextFrameTargetSpeed, 0f);
        }
    }

    /// <summary>
    /// 入力に基づき速度計算＋スタミナ消費
    /// </summary>
    private float ComputeSpeedFromInput()
    {
        float currentSpeed = rb.velocity.x;

        // Aキーで減速（スタミナ消費なし）
        if (Input.GetKey(decelKey))
            currentSpeed -= deceleration * Time.deltaTime;

        // Dキーで加速（スタミナが残っている場合のみ）
        if (Input.GetKey(accelKey))
        {
            if (stamina == null)
            {
                // PlayerStamina 未設定時はデバッグ用に加速
                currentSpeed += acceleration * Time.deltaTime;
            }
            else
            {
                if (!stamina.IsExhausted)
                {
                    currentSpeed += acceleration * Time.deltaTime;

                    // スタミナ消費（PlayerStamina 側の drainRate を使用） ← 変更部分
                    float drain = stamina.drainRate * staminaDrainMultiplier * Time.deltaTime;
                    stamina.Drain(drain);
                }
                // Exhausted なら加速不可
            }
        }

        // 速度クランプ
        currentSpeed = Mathf.Clamp(currentSpeed, baseSpeed, maxSpeed);

        return currentSpeed;
    }

    /// <summary>
    /// 将来の「ライバルをジャマする」アクションの受け口
    /// </summary>
    private void HandleJamInput()
    {
        if (Input.GetKeyDown(jamKey))
        {
            Debug.Log("[Jam] ジャマアクション発動（未実装）");
        }
    }

    // 外部から現在速度を取得（ヘルパー）
    public float GetCurrentSpeed()
    {
        return rb != null ? rb.velocity.x : 0f;
    }

    // 外部から目標速度を強制セット（デバッグ用）
    public void SetSpeed(float target)
    {
        float clamped = Mathf.Clamp(target, baseSpeed, maxSpeed);
        if (applyVelocityInFixed)
            nextFrameTargetSpeed = clamped;
        else if (rb != null)
            rb.velocity = new Vector2(clamped, 0f);
    }
}
