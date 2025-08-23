using System;
using System.Collections.Generic;
using UnityEngine;

public class AreaSpawn : MonoBehaviour
{
    [Serializable]
    public class Quantity
    {
        public string itemType;
        public int amount;
        [NonSerialized] public float nextSpawnTime;
        [NonSerialized] public float spawnInterval;
    }
    
    public List<Quantity> output;
    private GameObject itemPrefab;
    private Bounds areaBounds;
    private TimeManager timeManager;
    private int currentDay;
    private ItemManager itemManager;
    

    void Start()
    {
        timeManager = GameObject.Find("TimeManager").GetComponent<TimeManager>();
        itemManager = GameObject.Find("ItemManager").GetComponent<ItemManager>();
        itemPrefab = Resources.Load<GameObject>("Prefabs/Item");
        CalculateSpawnTimes();
        CalculateAreaBounds();
    }

    void Update()
    {
        if (timeManager.Day != currentDay)
        {
            currentDay = timeManager.Day;
            CalculateSpawnTimes();
        }

        foreach (var item in output)
        {
            if (timeManager.Hour >= item.nextSpawnTime)
            {
                SpawnItem(item.itemType);
                item.nextSpawnTime += item.spawnInterval;
            }
        }
    }

    private void CalculateSpawnTimes()
    {
        foreach (var item in output)
        {
            item.spawnInterval = 24f / Math.Max(1, item.amount);
            item.nextSpawnTime = item.spawnInterval / 2f;
        }
    }

    private void SpawnItem(string itemType)
    {
        if (itemPrefab != null)
        {
            Vector3 spawnPosition = new Vector3(UnityEngine.Random.Range(areaBounds.min.x, areaBounds.max.x), 
                                                UnityEngine.Random.Range(areaBounds.min.y, areaBounds.max.y), 
                                                transform.position.z);
    
            itemManager.CreateItem(spawnPosition, itemType, null);
        }
    }

    private void CalculateAreaBounds()
    {
        areaBounds = new Bounds(transform.position, new Vector3(transform.localScale.x, transform.localScale.y, 0));
    }
}