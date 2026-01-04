using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;      // Player を割当てる
    public float smoothSpeed = 5; // 追従速度

    [Tooltip("カメラのオフセット X,Y")]
    public Vector3 offset = new Vector3(2, 0, -20);

    void LateUpdate()
    {
        if (target == null) return;

        Vector3 desiredPos = target.position + offset;
        Vector3 smoothed = Vector3.Lerp(transform.position, desiredPos, smoothSpeed * Time.deltaTime);

        transform.position = smoothed;
    }
}
