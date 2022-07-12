using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Destroyer : MonoBehaviour
{
    // 물체 삭제
    private void OnTriggerEnter(Collider other)
    {
        other.gameObject.SetActive(false);
    }
}
