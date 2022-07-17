using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class Spawner : MonoBehaviour
{
    [Tooltip("����� ��ֹ� Object ���")] 
    public List<MoveLeft> obstacleObjects;
    private Queue<MoveLeft> obstacleQueue = new Queue<MoveLeft>();
    [Tooltip("����� ���� Object ���")] 
    public List<MoveLeft> rewardObjects;
    private Queue<MoveLeft> rewardQueue = new Queue<MoveLeft>();

    [Header("Spawn Setting")]
    public float obstacleMinSpawnInterval = 1.5f;
    [Range(0, 1)]
    public float obstacleSpawnChance = 0.3f;
    [Tooltip("��ֹ��� �ִ� �� �� ��������(1000 Step ����)")]
    public int obstacleSpawnMaxCount = 3;
    public float rewardMinSpawnInterval = 0.45f;
    [Range(0, 1)]
    public float rewardSpawnChance = 0.6f;
    [Tooltip("������ �ִ� �� �� ��������(1000 Step ����)")]
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

        // ������ ���� ������ ������ �Ʒ��� ������ �����ϴµ� ���
        halfHeight = maxHeight / 2;

        // ���� �� Init
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

        // ���� ����
        bool isObstacleSpawned = SpawnObstacle(out bool isUpObstacle);
        obstacleSpawnRemainTime -= Time.fixedDeltaTime;
        
        // ���� ����
        SpawnReward(isObstacleSpawned, isUpObstacle);
        rewardSpawnRemainTime -= Time.fixedDeltaTime;

        // �̹� Fixed Delta Time���� �����ϰ� ����
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

        // ���� ���� ���� : ������ ���� �Ʒ����� ������
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

        // ���� ���� ���� ����
        int rewardHeight;
        if (isObstacleSpawned)
        {
            // ������ ���� ������ ������ �Ʒ��� ������ ����
            rewardHeight = isUpObstacle ?
                Random.Range(0, halfHeight) :
                Random.Range(halfHeight, maxHeight);
        }
        else
        {
            // ������ ���ٸ� ���̰� ���� ������ �ʿ� ����
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
        // ������ �� �� �ٽ� �޾ƿ��� ����
        var obj = spawnQueue.Dequeue();
        spawnQueue.Enqueue(obj);
        obj.gameObject.SetActive(true);
        var y = initHeight + height;
        obj.transform.localPosition = new Vector3(spawnPosX, y, 0);
    }

    #endregion

    /// <summary>
    /// MaxStep�� �°� �ִ� ���� ī��Ʈ�� �����Ѵ�
    /// (�ʱ� 500 ������ ��ü�� �����ϴ� �ð� ����ġ)
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

        // ������ ������ϸ� ���� ���� �ֱ⵵ �ʱ�ȭ�ؾ��Ѵ�
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
