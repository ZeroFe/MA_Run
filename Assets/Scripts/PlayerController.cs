using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 테스트용 플레이어 컨트롤러
/// </summary>
public class PlayerController : MonoBehaviour
{
    private Rigidbody rb;

    // 점프 가능 횟수
    public float jumpPower = 3;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            rb.AddForce(Vector3.up * jumpPower, ForceMode.Impulse);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        print("Triggered");
    }
}
