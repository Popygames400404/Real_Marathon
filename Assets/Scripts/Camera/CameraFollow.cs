using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("追従対象")]
    public Transform target;      // カメラが追従する Player オブジェクト

    [Header("追従設定")]
    [Tooltip("カメラの追従速度（大きいほど追従が速い）")]
    public float smoothSpeed = 5f; // 補間速度

    [Tooltip("カメラの位置オフセット（Player からの相対位置）")]
    public Vector3 offset = new Vector3(2, 0, -20); // 初期値はプレイ画面に合わせて調整

    void LateUpdate()
    {
        if (target == null) return; // 追従対象がいなければ処理しない

        // 目標位置 = Player位置 + オフセット
        Vector3 desiredPos = target.position + offset;

        // 現在位置と目標位置を滑らかに補間
        Vector3 smoothed = Vector3.Lerp(transform.position, desiredPos, smoothSpeed * Time.deltaTime);

        // カメラを更新
        transform.position = smoothed;
    }
}

