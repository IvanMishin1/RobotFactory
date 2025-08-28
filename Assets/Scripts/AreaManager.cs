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
    public Area CreateArea(Vector2 position, List<Area.InputData> input, List<Area.OutputData> output, string areaName = null)
    {
        GameObject area = Instantiate(areaPrefab, position, Quaternion.identity, transform);
        if (String.IsNullOrEmpty(areaName))
            area.name = "Area" + areaId++;
        else
            area.name = areaName;
        Area areaComponent = area.GetComponent<Area>();
        input ??= new List<Area.InputData>();
        output ??= new List<Area.OutputData>();
        areaComponent.Input = input;
        areaComponent.Output = output;
        return areaComponent;
    }
    public void DestroyAllAreas()
    {
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }
    }
}