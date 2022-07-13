using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class Spawner : MonoBehaviour
{
    [Tooltip("사용할 장애물 Object 목록")] 
    public List<GameObject> obstacleObjects;
    private Queue<GameObject> obstacleQueue = new Queue<GameObject>();
    [Tooltip("사용할 보상 Object 목록")] 
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
        // 이번에 생성할지 말지 판단한다
        while (true)
        {
            bool obstacleSpawned = false;
            // 함정 높이 결정
            int obstacleHeight = UnityEngine.Random.Range(0, maxHeight);
            // 함정 생성?
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

            // 보상 높이 결정
            int rewardHeight = UnityEngine.Random.Range(0, maxHeight);
            // 보상하고 함정 위치 겹치나 확인
            while (obstacleSpawned && obstacleHeight == rewardHeight)
            {
                // 겹친다면 재설정
                rewardHeight = UnityEngine.Random.Range(0, maxHeight);
            }
            // 보상 생성
            Spawn(rewardQueue, rewardHeight);

            yield return checkWS;
        }
    }

    public void Spawn(Queue<GameObject> spawnQueue, int height)
    {
        // 밖으로 뺀 후 다시 받아오는 구조
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
