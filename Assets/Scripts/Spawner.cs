using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class Spawner : MonoBehaviour
{
    [Tooltip("����� ��ֹ� Object ���")] 
    public List<MoveLeft> obstacleObjects;
    private Queue<MoveLeft> obstacleQueue = new Queue<MoveLeft>();
    [Tooltip("����� ���� Object ���")] 
    public List<MoveLeft> rewardObjects;
    private Queue<MoveLeft> rewardQueue = new Queue<MoveLeft>();

    [Header("Spawn Setting")]
    [FormerlySerializedAs("spawnInterval")] 
    public float checkSpawnInterval = 0.5f;
    public float obstacleMinSpawnInterval = 1.5f;
    public float rewardMinSpawnInterval = 0.45f;
    private WaitForSeconds checkWS;

    public float spawnPosX = 20.0f;
    public float initHeight = 0.0f;
    public int maxHeight = 4;

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
        StartCoroutine(IECheckSpawn());
    }

    IEnumerator IECheckSpawn()
    {
        float obstacleSpawnRemainTime = 0.0f;
        float rewardSpawnRemainTime = 0.0f;
        // �̹��� �������� ���� �Ǵ��Ѵ�
        while (true)
        {
            // ���� ���� ���� : ������ ���� �Ʒ����� ������
            bool isUpObstacle = Random.value > 0.5f;
            int obstacleHeight = isUpObstacle ? maxHeight - 1 : 0;
            // ���� ����?
            if (obstacleSpawnRemainTime <= 0)
            {
                obstacleSpawnRemainTime = obstacleMinSpawnInterval;
                Spawn(obstacleQueue, obstacleHeight);
            }
            else
            {
                obstacleSpawnRemainTime -= checkSpawnInterval;
            }

            // ���� ���� ����
            int halfHeight = maxHeight / 2;
            // ������ ���� ������ ������ �Ʒ��� ������ ����
            int rewardHeight = isUpObstacle ? 
                Random.Range(0, halfHeight) : 
                Random.Range(halfHeight, maxHeight);

            // ���� ����?
            if (rewardSpawnRemainTime <= 0)
            {
                rewardSpawnRemainTime = rewardMinSpawnInterval;
                Spawn(rewardQueue, rewardHeight);
            }
            else
            {
                rewardSpawnRemainTime -= checkSpawnInterval;
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
    }
}
