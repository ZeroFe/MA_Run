using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// �浹�� �����ؼ� ���¸� �����ϰ� ���� ĥ���ִ� ��ũ��Ʈ
/// </summary>
public class CollisionDrawer : MonoBehaviour
{
    /// <summary>
    /// �浹 ���� ǥ��
    /// ��Ʈ �÷��� ��� - ���� ��ġ�� ��쵵 ���
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

    // ��Ʈ ���� Ȱ��. ����� ������ ��ġ�� ��� ���� �� �ֱ� ����
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
