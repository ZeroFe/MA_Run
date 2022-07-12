using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    [Tooltip("생성할 Object 목록")] 
    public List<GameObject> spawnObjects;
    private Queue<GameObject> spawnQueue = new Queue<GameObject>();

    public float spawnInterval = 3.0f;
    private float currentTime = 0.0f;

    public float spawnPosX = 20.0f;
    public float initHeight = -0.5f;
    public float maxHeight = 7.0f;

    void Start()
    {
        for (int i = 0; i < spawnObjects.Count; i++)
        {
            spawnQueue.Enqueue(spawnObjects[i]);
        }
    }

    void Update()
    {
        if (currentTime > spawnInterval)
        {
            currentTime -= spawnInterval;
            Spawn();
        }
        currentTime += Time.deltaTime;
    }

    public void Spawn()
    {
        // 밖으로 뺀 후 다시 받아오는 구조
        var obj = spawnQueue.Dequeue();
        spawnQueue.Enqueue(obj);
        obj.gameObject.SetActive(true);
        var y = initHeight + UnityEngine.Random.Range(0, maxHeight);
        obj.transform.localPosition = new Vector3(spawnPosX, y, 0);
    }
}
