using UnityEngine;

public class EntryArea : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        InvokeRepeating("SpawnOre", 1f, 5f);

    }

    void SpawnOre()
    {
        GameObject orePrefab = Resources.Load<GameObject>("Prefabs/Ore");
        if (orePrefab != null)
        {
            // Generate random position within 0.4f radius
            Vector2 randomOffset = Random.insideUnitCircle * 0.4f;
            Vector3 spawnPosition = transform.position + new Vector3(randomOffset.x, randomOffset.y, 0f);
        
            GameObject ore = Instantiate(orePrefab, spawnPosition, Quaternion.identity);
            ore.transform.SetParent(GameObject.Find("Items").transform);
            ore.GetComponent<Item>().SetItemType("ore");
        }
        else
        {
            Debug.LogError("Ore prefab not found!");
        }
    }
}
