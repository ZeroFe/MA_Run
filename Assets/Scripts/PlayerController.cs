using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// �׽�Ʈ�� �÷��̾� ��Ʈ�ѷ�
/// </summary>
public class PlayerController : MonoBehaviour
{
    private Rigidbody rb;

    // ���� ���� Ƚ��
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
        // ���� ���
        currentJumpCount = maxJumpCount;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Obstacle"))
        {
            // ���� ��
            print("Game End");
        }
        else if (other.CompareTag("Reward"))
        {
            print("Reward");
            other.gameObject.SetActive(false);
        }
    }
}
