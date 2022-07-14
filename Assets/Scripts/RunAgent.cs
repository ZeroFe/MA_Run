using System;
using System.Collections;
using System.Collections.Generic;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using Unity.VisualScripting;
using UnityEngine;
using Random = UnityEngine.Random;

public class RunAgent : Agent
{
    private static readonly float earnCookieReward = 0.03f;
    private static readonly float gameEndReward = 0.1f;
    private static readonly float hitObstacleReward = -0.01f;

    [Header("Jump")]
    public int maxJumpCount = 1;
    private int currentJumpCount;
    public float jumpPower = 7;

    [Header("Spawn Setting")] 
    public Spawner spawner;

    [Header("Game Time Setting")]
    public float gameTime = 10;
    private float currentGameTime;

    // Observation Setting
    [Header("Observation Setting")] 
    public CollisionDrawer[] collisionDrawers;
    private static readonly int collisionRaw = 8;
    
    // Physics Setting
    private Rigidbody rb;
    
    public override void Initialize()
    {
        rb = GetComponent<Rigidbody>();
    }

    public override void OnEpisodeBegin()
    {
        // 점프 중 죽었으면 초기화
        if (transform.localPosition.y > 0.5)
        {
            rb.velocity = Vector3.zero;
            transform.localPosition = Vector3.up * 0.5f;
            currentJumpCount = maxJumpCount;
        }


        // 충돌 감지 박스 초기화
        foreach (var collisionDrawer in collisionDrawers)
        {
            collisionDrawer.CollisionState = (int)CollisionDrawer.CollisionType.None;
        }
        
        // 생성된 함정, 보상 리셋
        spawner.ResetObjects();
        
        currentGameTime = gameTime;
    }

    private void FixedUpdate()
    {
        UpdateGameTime();
    }

    private void UpdateGameTime()
    {
        // 시간 측정
        currentGameTime -= Time.fixedDeltaTime;
        // 설정한 시간 지나면 끝내기
        if (currentGameTime < 0)
        {
            print("Game End");
            AddReward(gameEndReward);
            EndEpisode();
        }
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        // 현재 점프 가능 횟수
        sensor.AddObservation(currentJumpCount);
        // 현재 높이
        sensor.AddObservation(transform.position.y);
        // 현재 속도
        sensor.AddObservation(rb.velocity.y);
        // 충돌체 세팅
        for (int i = 0; i < collisionDrawers.Length; i++)
        {
            sensor.AddObservation(collisionDrawers[i].CollisionState);
        }
    }

    public override void OnActionReceived(ActionBuffers actionBuffers)
    {
        // Actions, Discrete size = 1
        // 점프 설정
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
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var discreteActionsOut = actionsOut.DiscreteActions;
        discreteActionsOut[0] = Input.GetKey(KeyCode.Space) ? 1 : 0;
    }

    private void OnCollisionEnter(Collision collision)
    {
        // 바닥과 충돌
        currentJumpCount = maxJumpCount;
    }

    private void OnTriggerEnter(Collider other)
    {
        int height = (int)other.transform.localPosition.y;
        if (height > 3) height = 3; // 임시 : 최대 높이 설정
        collisionDrawers[height * collisionRaw].CollisionState = 0;
        // 함정 충돌
        if (other.CompareTag("Obstacle"))
        {
            // 마이너스 보상
            //print("Hit Obstacle");
            //AddReward(hitObstacleReward);
            EndEpisode();
        }
        // 보상(쿠키)
        else if (other.CompareTag("Reward"))
        {
            AddReward(earnCookieReward);
            other.gameObject.SetActive(false);
        }

    }
}
