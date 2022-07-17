using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class Spawner : MonoBehaviour
{
    [Tooltip("사용할 장애물 Object 목록")] 
    public List<MoveLeft> obstacleObjects;
    private Queue<MoveLeft> obstacleQueue = new Queue<MoveLeft>();
    [Tooltip("사용할 보상 Object 목록")] 
    public List<MoveLeft> rewardObjects;
    private Queue<MoveLeft> rewardQueue = new Queue<MoveLeft>();

    [Header("Spawn Setting")]
    public float obstacleMinSpawnInterval = 1.5f;
    [Range(0, 1)]
    public float obstacleSpawnChance = 0.3f;
    [Tooltip("장애물이 최대 몇 개 생성될지(1000 Step 기준)")]
    public int obstacleSpawnMaxCount = 3;
    public float rewardMinSpawnInterval = 0.45f;
    [Range(0, 1)]
    public float rewardSpawnChance = 0.6f;
    [Tooltip("보상이 최대 몇 개 생성될지(1000 Step 기준)")]
    public int rewardSpawnMaxCount = 7;

    private int obstacleSpawnCount;
    private int rewardSpawnCount;
    private int remainObjCount;

    public int RemainObjCount
    {
        get => remainObjCount;
        set
        {
            remainObjCount--;
            CheckGameEnd();
        }
    }

    public float checkSpawnInterval = 0.5f;
    private float currentSpawnInterval;
    private float obstacleSpawnRemainTime = 0.0f;
    private float rewardSpawnRemainTime = 0.0f;

    public float spawnPosX = 20.0f;
    public float initHeight = 0.0f;
    public int maxHeight = 4;
    private int halfHeight;

    public event Action onGameEnd;

    void Start()
    {
        foreach (var t in obstacleObjects)
        {
            obstacleQueue.Enqueue(t);
        }
        foreach (var t in rewardObjects)
        {
            rewardQueue.Enqueue(t);
        }

        // 함정이 위에 있으면 보상은 아래에 나오게 결정하는데 사용
        halfHeight = maxHeight / 2;

        // 실행 전 Init
        ResetGame();
    }

    #region Spawn
    private void FixedUpdate()
    {
        CheckSpawn();
    }

    private void CheckSpawn()
    {
        currentSpawnInterval -= Time.fixedDeltaTime;

        // 함정 생성
        bool isObstacleSpawned = SpawnObstacle(out bool isUpObstacle);
        obstacleSpawnRemainTime -= Time.fixedDeltaTime;
        
        // 보상 생성
        SpawnReward(isObstacleSpawned, isUpObstacle);
        rewardSpawnRemainTime -= Time.fixedDeltaTime;

        // 이번 Fixed Delta Time에만 생성하게 조절
        if (currentSpawnInterval < 0)
        {
            currentSpawnInterval = checkSpawnInterval;
        }
    }

    private bool SpawnObstacle(out bool isUpObstacle)
    {
        if (obstacleSpawnRemainTime > 0 ||
            obstacleSpawnCount <= 0 ||
            currentSpawnInterval > 0 ||
            Random.value > obstacleSpawnChance)
        {
            isUpObstacle = false;
            return false;
        }

        // 함정 높이 결정 : 함정은 위나 아래에만 나오게
        isUpObstacle = Random.value > 0.5f;
        int obstacleHeight = isUpObstacle ? maxHeight - 1 : 0;
        obstacleSpawnRemainTime = obstacleMinSpawnInterval;
        Spawn(obstacleQueue, obstacleHeight);
        obstacleSpawnCount--;
        remainObjCount++;
        return true;
    }

    private bool SpawnReward(bool isObstacleSpawned, bool isUpObstacle)
    {
        if (rewardSpawnRemainTime > 0 || 
            rewardSpawnCount <= 0 ||
            currentSpawnInterval > 0 ||
            Random.value > rewardSpawnChance)
        {
            return false;
        }

        // 보상 생성 높이 결정
        int rewardHeight;
        if (isObstacleSpawned)
        {
            // 함정이 위에 있으면 보상은 아래만 나오게 설정
            rewardHeight = isUpObstacle ?
                Random.Range(0, halfHeight) :
                Random.Range(halfHeight, maxHeight);
        }
        else
        {
            // 함정이 없다면 높이가 딱히 조정될 필요 없음
            rewardHeight = Random.Range(0, maxHeight);
        }

        rewardSpawnRemainTime = rewardMinSpawnInterval;
        Spawn(rewardQueue, rewardHeight);
        rewardSpawnCount--;
        remainObjCount++;

        return true;
    }

    private void Spawn(Queue<MoveLeft> spawnQueue, int height)
    {
        // 밖으로 뺀 후 다시 받아오는 구조
        var obj = spawnQueue.Dequeue();
        spawnQueue.Enqueue(obj);
        obj.gameObject.SetActive(true);
        var y = initHeight + height;
        obj.transform.localPosition = new Vector3(spawnPosX, y, 0);
    }

    #endregion

    /// <summary>
    /// MaxStep에 맞게 최대 생성 카운트를 조절한다
    /// (초기 500 스텝은 물체가 도달하는 시간 보정치)
    /// </summary>
    /// <param name="maxStep"></param>
    public void SetMaxCount(int maxStep)
    {
        float maxStepPermile = (float)(maxStep - 1000) / 1000.0f;
        obstacleSpawnMaxCount = (int)(obstacleSpawnMaxCount * maxStepPermile);
        rewardSpawnMaxCount = (int)(rewardSpawnMaxCount * maxStepPermile);
    }

    public void ResetGame()
    {
        foreach (var obj in obstacleObjects)
        {
            obj.gameObject.SetActive(false);
        }
        foreach (var obj in rewardObjects)
        {
            obj.gameObject.SetActive(false);
        }

        // 게임을 재시작하면 기촌 생성 주기도 초기화해야한다
        remainObjCount = 0;
        obstacleSpawnCount = obstacleSpawnMaxCount;
        rewardSpawnCount = rewardSpawnMaxCount;

        currentSpawnInterval = checkSpawnInterval;
        obstacleSpawnRemainTime = 0.0f;
        rewardSpawnRemainTime = 0.0f;
    }

    public void CheckGameEnd()
    {
        if (remainObjCount <= 0 && obstacleSpawnCount <= 0 && rewardSpawnCount <= 0)
        {
            onGameEnd?.Invoke();
        }
    }
}
