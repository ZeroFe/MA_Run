using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Destroyer : MonoBehaviour
{
    // ��ü ����
    private void OnTriggerEnter(Collider other)
    {
        other.gameObject.SetActive(false);
    }
}
