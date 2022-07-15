using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Destroyer : MonoBehaviour
{
    public Spawner spawner;

    // 물체 삭제
    private void OnTriggerStay(Collider other)
    {
        other.gameObject.SetActive(false);
        spawner.RemainObjCount--;
    }
}
