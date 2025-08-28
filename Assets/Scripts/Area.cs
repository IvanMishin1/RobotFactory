using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using UnityEngine;

public class Area : MonoBehaviour
{
    [Serializable]
    public class OutputData
    {
        public string itemType;
        public int amount;
        [NonSerialized][JsonIgnore] public float nextSpawnTime;
        [NonSerialized][JsonIgnore] public float spawnInterval;
    }
    
    [Serializable]
    public class InputData
    {
        public string itemType;
        public int value;
    }
    
    public List<OutputData> Output;
    public List<InputData> Input;
    private Dictionary<string, int> inputDict // TODO: This is inefficient
    {
        get
        {
            var dict = new Dictionary<string, int>();
            foreach (var item in Input)
            {
                dict[item.itemType] = item.value;
            }
            return dict;
        }
    }
    
    private GameObject itemPrefab;
    private Bounds areaBounds;
    private TimeManager timeManager;
    private int currentDay;
    private ItemManager itemManager;
    private GameManager gameManager;
    private MoneyManager moneyManager;
    

    void Awake()
    {
        timeManager = GameObject.Find("TimeManager").GetComponent<TimeManager>();
        itemManager = GameObject.Find("ItemManager").GetComponent<ItemManager>();
        moneyManager = GameObject.Find("MoneyManager").GetComponent<MoneyManager>();
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        itemPrefab = Resources.Load<GameObject>("Prefabs/Item");
    }
    void Start()
    {
        CalculateSpawnTimes();
        CalculateAreaBounds();
    }

    void Update()
    {
        if (Output.Count == 0)
            return;
        if (timeManager.Day != currentDay)
        {
            currentDay = timeManager.Day;
            CalculateSpawnTimes();
        }

        foreach (var item in Output)
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
        foreach (var item in Output)
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
    
    void OnTriggerStay2D(Collider2D itemGameObject)
    {
        // This handles input into the area
        if (!itemGameObject.CompareTag("Item"))
            return;
        
        Item item = itemGameObject.GetComponent<Item>();
        if (item != null && !item.transform.parent.CompareTag("Robot") && item.transform.parent != transform)
        {
            inputDict.TryGetValue(item.type, out int value);
            if (value > 0)
            {
                moneyManager.AddMoney(value);
                Destroy(item.gameObject);
            }
        }
    }
}