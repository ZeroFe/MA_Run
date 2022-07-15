using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Destroyer : MonoBehaviour
{
    public Spawner spawner;

    // ��ü ����
    private void OnTriggerStay(Collider other)
    {
        other.gameObject.SetActive(false);
        spawner.RemainObjCount--;
    }
}
