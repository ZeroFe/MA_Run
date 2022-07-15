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

        // ������ ������ϸ� ���� ���� �ֱ⵵ �ʱ�ȭ�ؾ��Ѵ�
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
        // �̹��� �������� ���� �Ǵ��Ѵ�
        while (obstacleSpawnCount > 0 || rewardSpawnCount > 0)
        {
            bool spawnedObstacle = obstacleSpawnRemainTime <= 0 &&
                                   obstacleSpawnCount > 0 &&
                                   Random.value < obstacleSpawnChance;
            // ���� ���� ���� : ������ ���� �Ʒ����� ������
            bool isUpObstacle = Random.value > 0.5f;
            int obstacleHeight = isUpObstacle ? maxHeight - 1 : 0;
            // ���� ����?
            if (spawnedObstacle)
            {
                obstacleSpawnRemainTime = obstacleMinSpawnInterval;
                Spawn(obstacleQueue, obstacleHeight);
                obstacleSpawnCount--;
                remainObjCount++;
            }
            else
            {
                // ���� ���� �� �Ҽ��� ��Ȯ�� ������ �ֱⰡ Ʋ���� �� �����Ƿ� ������ ���ش�
                obstacleSpawnRemainTime -= checkSpawnInterval + INTERVAL_REVISION_BONUS;
            }

            bool spawnedReward = rewardSpawnRemainTime <= 0 &&
                                 rewardSpawnCount > 0 &&
                                 Random.value < rewardSpawnChance;

            // ���� ���� ����
            int halfHeight = maxHeight / 2;
            // ������ ���� ������ ������ �Ʒ��� ������ ����
            int rewardHeight = isUpObstacle ? 
                Random.Range(0, halfHeight) : 
                Random.Range(halfHeight, maxHeight);

            // ���� ����?
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
        // ������ �� �� �ٽ� �޾ƿ��� ����
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
