using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 왼쪽으로 움직이는 물체
/// </summary>
public class MoveLeft : MonoBehaviour
{
    public float speed = 2.0f;

    private void Update()
    {
        transform.Translate(Vector3.left * (speed * Time.deltaTime));
    }
}
