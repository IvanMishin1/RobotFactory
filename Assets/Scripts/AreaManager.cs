using System;
using System.Collections.Generic;
using UnityEngine;

public class AreaManager : MonoBehaviour
{
    private int areaId = 0; // TODO: This should be stored
    private GameObject areaPrefab;
    void Awake()
    {
        // Awake to ensure the prefab is loaded before any areas are created
        areaPrefab = Resources.Load<GameObject>("Prefabs/Area");
        if (areaPrefab == null)
            Debug.LogError("Area prefab not found in Resources/Prefabs/Area");
    }
    public GameObject CreateArea(Vector2 position, Dictionary<string,int> Destroy, string areaName = null)
    {
        GameObject area = Instantiate(areaPrefab, position, Quaternion.identity);
        if (String.IsNullOrEmpty(areaName))
            area.name = "Area" + areaId++;
        else
            area.name = areaName;
        area.transform.parent = transform;
        Area areaComponent = area.GetComponent<Area>();
        return area;
    }
    public void DestroyAllAreas()
    {
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }
    }
}