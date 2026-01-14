using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController_VSCode : MonoBehaviour
{
    [Header("Speed Settings")]
    [Tooltip("最低速度")]
    public float baseSpeed = 2f;
    [Tooltip("最高速度")]
    public float maxSpeed = 6f;
    [Tooltip("加速値")]
    public float acceleration = 4f;
    [Tooltip("減速値")]
    public float deceleration = 2f;

    [Header("Stamina / スタミナ")]
    [Tooltip("PlayerStamina スクリプトをインスペクターで割り当てる")]
    public PlayerStamina stamina;                 // PlayerStamina コンポーネントを割当て
    [Tooltip("スタミナ消費倍率")]
    public float staminaDrainMultiplier = 1f;     // スタミナ消費量を調整

    [Header("Controls")]
    public KeyCode accelKey = KeyCode.D;
    public KeyCode decelKey = KeyCode.A;
    public KeyCode jamKey = KeyCode.J;            // 将来ジャマキー

    [Header("Behavior")]
    public bool applyVelocityInFixed = true;//計算内用のUpdateかFixed（）へのぶち込み方スイッチ
    private Rigidbody2D rb;
    private float nextFrameTargetSpeed;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0f;
        rb.freezeRotation = true;

        rb.velocity = new Vector2(baseSpeed, 0f);//newによってVector2という設計図にｘ,ｙという型が入る。
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
                stamina.Regenerate(stamina.regeneRate * Time.deltaTime);//PlayerStamina.回復の使いどころ（タイミング）を支持

            }
        }
        //PlayerStamina.cs依存箇所--------------------------------------------------------------------------------


        //// ===== Fixed（）へのぶち込み方 =====
        float computedSpeed = ComputeSpeedFromInput();

        if (applyVelocityInFixed==true)
            nextFrameTargetSpeed = computedSpeed; //物理計算得意のFixed（）を挟んで、rbに計算内容をぶち込む方法。
        else
            rb.velocity = new Vector2(computedSpeed, 0f); //Update（）から直接rbにぶち込む方法。

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
        float currentSpeed = rb.velocity.x;//CurrentSpeedに現在のSpeedをコピー保管。

        // Aキーで減速（スタミナ消費なし）
        if (Input.GetKey(decelKey))
            currentSpeed -= deceleration * Time.deltaTime;//現在速度＝現在速度-減速値

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
                if (!stamina.IsExhausted)//疲れ切っていない場合。
                {
                    currentSpeed += acceleration * Time.deltaTime;//現在速度＝現在速度+加速値

                  
                    // PlayerStaminaの消費量を調整。→PlayerStaminaのDrain（）に返す。
                    float drain = stamina.drainRate * staminaDrainMultiplier * Time.deltaTime;
                    stamina.Drain(drain);
                }
            }
        }
        //PlayerStamina.cs依存箇所--------------------------------------------------------------------------------

        //現在速度を最低速度と最高速度の間に抑える。　速度制限
        currentSpeed = Mathf.Clamp(currentSpeed, baseSpeed, maxSpeed);

        return currentSpeed; //ComputeSpeedFromInput()の計算内容を現在速度に返す。
    }

    /// <summary>
    /// 入力に基づき速度計算＋スタミナ消費
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

//     PlayerContlloler.cs

// 定義 	計算内用のUpdateかFixed（）へのぶち込み方スイッチ(applyVelocityInFixe) 
// 	↓
//  Start（） 	nextframespeed＝BaseSpeed
// 	↓　　　　　　　　　　　　　　　　　
// ComputeSpeedFromInput()	加速した場合・減速した場合をそれぞれ計算。 と共に加速した場合のスタミナ消費量もPlayerStaminacsと連携して計算　　
// 	↓　　　　　　　　　　　　　　　　　　　　　　　　　　　　
// Update（） 	計算結果をapplyVelocityInFixedにしたがってnextframespeedにぶち込む。
// 	↓
// Fixed（） 	nextframespeedに格納された計算結果に従って 物理計算。

}