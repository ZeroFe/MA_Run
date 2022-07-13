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

        // �浹ü �ʱ�ȭ
        foreach (var collisionDrawer in collisionDrawers)
        {
            collisionDrawer.CollisionState = (int)CollisionDrawer.CollisionType.None;
        }
        
        // ������ ����, ���� ����
        spawner.ResetObjects();
        
        currentGameTime = gameTime;
    }

    private void Update()
    {
        // �ð� ����
        currentGameTime -= Time.deltaTime;
        // ������ �ð� ������ ������
        if (currentGameTime < 0)
        {
            //print("Game End");
            SetReward(0.1f);
            EndEpisode();
        }
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        // ���� ���� ���� Ƚ��
        sensor.AddObservation(currentJumpCount);
        // ���� ����
        sensor.AddObservation(transform.position.y);
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
            currentJumpCount--;
            // ����
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
        // �� ����
        currentJumpCount = maxJumpCount;
    }

    private void OnTriggerEnter(Collider other)
    {
        // ���� �浹
        if (other.CompareTag("Obstacle"))
        {
            // ���̳ʽ� ����
            //print("Hit Obstacle");
            SetReward(-0.05f);
            EndEpisode();
        }
        // ����(��Ű)
        else if (other.CompareTag("Reward"))
        {
            SetReward(0.01f);
            other.gameObject.SetActive(false);
        }
    }
}
