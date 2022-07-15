using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 충돌을 감지해서 상태를 저장하고 색을 칠해주는 스크립트
/// </summary>
public class CollisionDrawer : MonoBehaviour
{
    /// <summary>
    /// 충돌 상태 표시
    /// 비트 플래그 사용 - 둘이 겹치는 경우도 고려
    /// </summary>
    public enum CollisionType
    {
        None = 0,
        Obstacle = 1,
        Reward = 2,
    }

    // Property
    public int collisionState = 0;

    public int CollisionState
    {
        get => collisionState;
        set
        {
            collisionState = value;
            meshRenderer.material = colorMats[collisionState];
        }
    }

    public Material[] colorMats = new Material[4];

    // 비트 연산 활용. 보상과 함정이 겹치는 경우 생길 수 있기 때문
    // 
    private MeshRenderer meshRenderer;

    private void Awake()
    {
        meshRenderer = GetComponent<MeshRenderer>();
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Obstacle"))
        {
            CollisionState |= (int)CollisionType.Obstacle;
        }
        else if (other.CompareTag("Reward"))
        {
            CollisionState |= (int)CollisionType.Reward;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Obstacle"))
        {
            CollisionState &= ~(int)CollisionType.Obstacle;
        }
        else if (other.CompareTag("Reward"))
        {
            CollisionState &= ~(int)CollisionType.Reward;
        }
    }
}
