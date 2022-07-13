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


    private Rigidbody rb;
    
    public override void Initialize()
    {
        rb = GetComponent<Rigidbody>();
    }

    public override void OnEpisodeBegin()
    {
        if (transform.localPosition.y > 0.5)
        {
            rb.angularVelocity = Vector3.zero;
            rb.velocity = Vector3.zero;
            transform.localPosition = Vector3.up * 0.5f;

            currentJumpCount = maxJumpCount;
        }

        // 충돌체 초기화
        foreach (var collisionDrawer in collisionDrawers)
        {
            collisionDrawer.CollisionState = (int)CollisionDrawer.CollisionType.None;
        }
        
        // 생성된 함정, 보상 리셋
        spawner.ResetObjects();
        
        currentGameTime = gameTime;
    }

    private void Update()
    {
        // 시간 측정
        currentGameTime -= Time.deltaTime;
        // 설정한 시간 지나면 끝내기
        if (currentGameTime < 0)
        {
            //print("Game End");
            SetReward(0.1f);
            EndEpisode();
        }
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        // 현재 점프 가능 횟수
        sensor.AddObservation(currentJumpCount);
        // 현재 높이
        sensor.AddObservation(transform.position.y);
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
            currentJumpCount--;
            // 점프
            rb.AddForce(Vector3.up * jumpPower, ForceMode.Impulse);
        }
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var discreteActionsOut = actionsOut.DiscreteActions;
        discreteActionsOut[0] = Input.GetKey(KeyCode.Space) ? 1 : 0;
    }

    private void OnCollisionEnter(Collision collision)
    {
        // 땅 만남
        currentJumpCount = maxJumpCount;
    }

    private void OnTriggerEnter(Collider other)
    {
        // 함정 충돌
        if (other.CompareTag("Obstacle"))
        {
            // 마이너스 보상
            //print("Hit Obstacle");
            SetReward(-0.05f);
            EndEpisode();
        }
        // 보상(쿠키)
        else if (other.CompareTag("Reward"))
        {
            SetReward(0.01f);
            other.gameObject.SetActive(false);
        }
    }
}
