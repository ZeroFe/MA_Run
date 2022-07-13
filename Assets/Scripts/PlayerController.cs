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
    public int maxJumpCount = 1;
    public int currentJumpCount;
    public float jumpPower = 3;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Start()
    {
        currentJumpCount = maxJumpCount;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && currentJumpCount > 0)
        {
            // Jump
            rb.AddForce(Vector3.up * jumpPower, ForceMode.Impulse);
            currentJumpCount--;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        // 땅인 경우
        currentJumpCount = maxJumpCount;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Obstacle"))
        {
            // 게임 끝
            print("Game End");
        }
        else if (other.CompareTag("Reward"))
        {
            print("Reward");
            other.gameObject.SetActive(false);
        }
    }
}
