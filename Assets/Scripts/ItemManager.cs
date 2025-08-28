using UnityEngine;
using System;

public class ItemManager : MonoBehaviour
{
    private GameObject itemPrefab;
    private int itemId = 0;
    void Awake()
    {
        // Awake to ensure the prefab is loaded before any items are created
        itemPrefab = Resources.Load<GameObject>("Prefabs/Item");
        if (itemPrefab == null)
            Debug.LogError("Item prefab not found in Resources/Prefabs/Item");
    }
    
    public Item CreateItem(Vector2 position, string itemType, string itemName)
    {
        GameObject item = Instantiate(itemPrefab, position, Quaternion.identity, transform);
        Item itemComponent = item.GetComponent<Item>();
        itemComponent.SetItemType(itemType);
        if (String.IsNullOrEmpty(itemName))
            item.name = "Item" + itemId++;
        else
            item.name = itemName;
        return itemComponent;
    }

    public void DestroyAllItems()
    {
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }
    }
}
