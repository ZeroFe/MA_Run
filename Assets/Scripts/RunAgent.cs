using System;
using System.Collections;
using System.Collections.Generic;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine;
using Random = UnityEngine.Random;

public class RunAgent : Agent
{
    private static readonly float earnCookieReward = 0.1f;
    private static readonly float gameEndReward = 0.1f;
    private static readonly float hitObstacleReward = -0.1f;

    [Header("Jump")]
    public int maxJumpCount = 1;
    private int currentJumpCount;
    public float jumpPower = 7;

    [Header("Spawn Setting")] 
    public Spawner spawner;

    // Observation Setting
    [Header("Observation Setting")] 
    public CollisionDrawer[] collisionDrawers;
    private static readonly int collisionRaw = 8;

    [Header("Test")]
    private int totalPlayCount = 0;
    private int successPlayCount = 0;

    // Physics Setting
    private Rigidbody rb;
    private Animator anim;
    
    public override void Initialize()
    {
        rb = GetComponent<Rigidbody>();
        anim = GetComponentInChildren<Animator>();

        spawner.SetMaxCount(MaxStep);
        spawner.onGameEnd += OnGameEnd;
    }

    public override void OnEpisodeBegin()
    {
        // ���� �� �׾����� �ʱ�ȭ
        if (transform.localPosition.y > 0.5)
        {
            rb.velocity = Vector3.zero;
            transform.localPosition = Vector3.up * 0.5f;
            currentJumpCount = maxJumpCount;
        }


        // �浹 ���� �ڽ� �ʱ�ȭ
        foreach (var collisionDrawer in collisionDrawers)
        {
            collisionDrawer.CollisionState = (int)CollisionDrawer.CollisionType.None;
        }
        
        // ������ ����, ���� ����
        spawner.ResetGame();

        // �����
        totalPlayCount++;
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        // ���� ���� ���� Ƚ��
        sensor.AddObservation(currentJumpCount);
        // ���� ����
        sensor.AddObservation(transform.position.y);
        // ���� �ӵ�
        sensor.AddObservation(rb.velocity.y);
        // �浹ü ����
        for (int i = 0; i < collisionDrawers.Length; i++)
        {
            sensor.AddObservation(collisionDrawers[i].CollisionState);
        }
    }

    public override void OnActionReceived(ActionBuffers actionBuffers)
    {
        // Actions, Discrete size = 1
        // ���� ����
        var jumpAction = actionBuffers.DiscreteActions[0];
        if (jumpAction == 1 && currentJumpCount > 0)
        {
            Jump();
        }
    }

    private void Jump()
    {
        currentJumpCount--;
        rb.AddForce(Vector3.up * jumpPower, ForceMode.Impulse);
        anim.SetTrigger("Jump_trig");
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var discreteActionsOut = actionsOut.DiscreteActions;
        discreteActionsOut[0] = Input.GetKey(KeyCode.Space) ? 1 : 0;
    }

    private void OnCollisionEnter(Collision collision)
    {
        // �ٴڰ� �浹
        currentJumpCount = maxJumpCount;
    }

    private void OnTriggerEnter(Collider other)
    {
        int height = (int)other.transform.localPosition.y;
        // 
        collisionDrawers[height * collisionRaw].CollisionState = 0;
        collisionDrawers[height * collisionRaw + 1].CollisionState = 0;
        // ���� �浹
        if (other.CompareTag("Obstacle"))
        {
            // ���̳ʽ� ����
            //print("Hit Obstacle");
            AddReward(hitObstacleReward);
            //LogGameFail();
            EndEpisode();
        }
        // ����(��Ű)
        else if (other.CompareTag("Reward"))
        {
            AddReward(earnCookieReward);
            other.gameObject.SetActive(false);
            spawner.RemainObjCount--;
        }
    }

    private void OnGameEnd()
    {
        //AddReward(gameEndReward);
        successPlayCount++;
        //LogGameClear();
        EndEpisode();
    }

    private void LogGameFail()
    {
        print("hit obstacle");
        print($"Clear Rate : {(successPlayCount * 100) / totalPlayCount}%");
    }

    private void LogGameClear()
    {
        print("Game Clear");
        print($"Clear Rate : {(successPlayCount * 100) / totalPlayCount}%");
    }
}
