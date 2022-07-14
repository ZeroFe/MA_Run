using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class Spawner : MonoBehaviour
{
    [Tooltip("사용할 장애물 Object 목록")] 
    public List<MoveLeft> obstacleObjects;
    private Queue<MoveLeft> obstacleQueue = new Queue<MoveLeft>();
    [Tooltip("사용할 보상 Object 목록")] 
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
        // 이번에 생성할지 말지 판단한다
        while (true)
        {
            // 함정 높이 결정 : 함정은 위나 아래에만 나오게
            bool isUpObstacle = Random.value > 0.5f;
            int obstacleHeight = isUpObstacle ? maxHeight - 1 : 0;
            // 함정 생성?
            if (obstacleSpawnRemainTime <= 0)
            {
                obstacleSpawnRemainTime = obstacleMinSpawnInterval;
                Spawn(obstacleQueue, obstacleHeight);
            }
            else
            {
                obstacleSpawnRemainTime -= checkSpawnInterval;
            }

            // 보상 높이 결정
            int halfHeight = maxHeight / 2;
            // 함정이 위에 있으면 보상은 아래만 나오게 설정
            int rewardHeight = isUpObstacle ? 
                Random.Range(0, halfHeight) : 
                Random.Range(halfHeight, maxHeight);

            // 보상 생성?
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
            obj.gameObject.SetActive(false);
        }
        foreach (var obj in rewardObjects)
        {
            obj.gameObject.SetActive(false);
        }
    }
}
