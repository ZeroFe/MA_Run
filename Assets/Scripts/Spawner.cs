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
    private WaitForSeconds checkWS;
    private static readonly float INTERVAL_REVISION_BONUS = 0.01f;

    public float spawnPosX = 20.0f;
    public float initHeight = 0.0f;
    public int maxHeight = 4;

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

        checkWS = new WaitForSeconds(checkSpawnInterval);
    }

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

    public void ResetObjects()
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
        StopCoroutine(nameof(IEStartGame));
        StartCoroutine(nameof(IEStartGame));
    }

    IEnumerator IEStartGame()
    {
        float obstacleSpawnRemainTime = 0.0f;
        float rewardSpawnRemainTime = 0.0f;

        remainObjCount = 0;
        obstacleSpawnCount = obstacleSpawnMaxCount;
        rewardSpawnCount = rewardSpawnMaxCount;
        // 이번에 생성할지 말지 판단한다
        while (obstacleSpawnCount > 0 || rewardSpawnCount > 0)
        {
            bool spawnedObstacle = obstacleSpawnRemainTime <= 0 &&
                                   obstacleSpawnCount > 0 &&
                                   Random.value < obstacleSpawnChance;
            // 함정 높이 결정 : 함정은 위나 아래에만 나오게
            bool isUpObstacle = Random.value > 0.5f;
            int obstacleHeight = isUpObstacle ? maxHeight - 1 : 0;
            // 함정 생성?
            if (spawnedObstacle)
            {
                obstacleSpawnRemainTime = obstacleMinSpawnInterval;
                Spawn(obstacleQueue, obstacleHeight);
                obstacleSpawnCount--;
                remainObjCount++;
            }
            else
            {
                // 빼기 수행 시 소수점 정확도 문제로 주기가 틀어질 수 있으므로 보정을 해준다
                obstacleSpawnRemainTime -= checkSpawnInterval + INTERVAL_REVISION_BONUS;
            }

            bool spawnedReward = rewardSpawnRemainTime <= 0 &&
                                 rewardSpawnCount > 0 &&
                                 Random.value < rewardSpawnChance;

            // 보상 높이 결정
            int halfHeight = maxHeight / 2;
            // 함정이 위에 있으면 보상은 아래만 나오게 설정
            int rewardHeight = isUpObstacle ? 
                Random.Range(0, halfHeight) : 
                Random.Range(halfHeight, maxHeight);

            // 보상 생성?
            if (spawnedReward)
            {
                rewardSpawnRemainTime = rewardMinSpawnInterval;
                Spawn(rewardQueue, rewardHeight);
                rewardSpawnCount--;
                remainObjCount++;
            }
            else
            {
                rewardSpawnRemainTime -= checkSpawnInterval + INTERVAL_REVISION_BONUS;
            }

            yield return checkWS;
        }
    }

    public void Spawn(Queue<MoveLeft> spawnQueue, int height)
    {
        // 밖으로 뺀 후 다시 받아오는 구조
        var obj = spawnQueue.Dequeue();
        spawnQueue.Enqueue(obj);
        obj.gameObject.SetActive(true);
        var y = initHeight + height;
        obj.transform.localPosition = new Vector3(spawnPosX, y, 0);
    }

    public void CheckGameEnd()
    {
        if (remainObjCount <= 0 && obstacleSpawnCount <= 0 && rewardSpawnCount <= 0)
        {
            onGameEnd?.Invoke();
        }
    }
}
