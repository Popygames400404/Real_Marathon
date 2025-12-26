using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackGroundScroll : MonoBehaviour
{
    [Header("Scroll Settings")]
    public float scrollMultiplier = 0.2f;   // Player速度に対する倍率

    [Header("References")]
    public PlayerController player;         // PlayerController を割り当てる

    private Vector3 startPosition;

    void Start()
    {
        // 初期位置を記録
        startPosition = transform.position;

        // Player 未設定チェック（初心者向け安全策）
        if (player == null)
        {
            Debug.LogWarning("BackGroundScroll: PlayerController が設定されていません");
        }
    }

    void Update()
    {
        if (player == null) return;

        float speed = player.GetCurrentSpeed();

        // X方向にスクロール（背景は遅く動く）
        float moveX = speed * scrollMultiplier * Time.deltaTime;

        transform.position += new Vector3(moveX, 0f, 0f);
    }
}
