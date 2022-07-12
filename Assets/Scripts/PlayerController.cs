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
