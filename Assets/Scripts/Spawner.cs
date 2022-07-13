using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class Spawner : MonoBehaviour
{
    [Tooltip("����� ��ֹ� Object ���")] 
    public List<GameObject> obstacleObjects;
    private Queue<GameObject> obstacleQueue = new Queue<GameObject>();
    [Tooltip("����� ���� Object ���")] 
    public List<GameObject> rewardObjects;
    private Queue<GameObject> rewardQueue = new Queue<GameObject>();

    [Header("Spawn Setting")]
    [FormerlySerializedAs("spawnInterval")] 
    public float checkSpawnInterval = 0.5f;
    public float obstacleMinSpawnInterval = 1.5f;
    public float rewardMinSpawnInterval = 0.45f;
    private WaitForSeconds checkWS;

    public float spawnPosX = 20.0f;
    public float initHeight = 0.0f;
    public int maxHeight = 5;

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
        // �̹��� �������� ���� �Ǵ��Ѵ�
        while (true)
        {
            bool obstacleSpawned = false;
            // ���� ���� ����
            int obstacleHeight = UnityEngine.Random.Range(0, maxHeight);
            // ���� ����?
            if (obstacleSpawnRemainTime <= 0)
            {
                obstacleSpawned = true;
                obstacleSpawnRemainTime = obstacleMinSpawnInterval;
                Spawn(obstacleQueue, obstacleHeight);
            }
            else
            {
                obstacleSpawnRemainTime -= checkSpawnInterval;
            }

            // ���� ���� ����
            int rewardHeight = UnityEngine.Random.Range(0, maxHeight);
            // �����ϰ� ���� ��ġ ��ġ�� Ȯ��
            while (obstacleSpawned && obstacleHeight == rewardHeight)
            {
                // ��ģ�ٸ� �缳��
                rewardHeight = UnityEngine.Random.Range(0, maxHeight);
            }
            // ���� ����
            Spawn(rewardQueue, rewardHeight);

            yield return checkWS;
        }
    }

    public void Spawn(Queue<GameObject> spawnQueue, int height)
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
            obj.SetActive(false);
        }
        foreach (var obj in rewardObjects)
        {
            obj.SetActive(false);
        }
    }
}
