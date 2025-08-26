using System;
using System.Collections.Generic;
using UnityEngine;

public class MachineManager : MonoBehaviour
{
    private int machineId = 0; // TODO: This should be stored
    private GameObject machinePrefab;
    void Awake()
    {
        // Awake to ensure the prefab is loaded before any machines are created
        machinePrefab = Resources.Load<GameObject>("Prefabs/Machine");
        if (machinePrefab == null)
            Debug.LogError("Machine prefab not found in Resources/Prefabs/Machine");
    }
    public Machine CreateMachine(Vector2 position, Dictionary<string, string> Recipes, string machineName = null)
    {
        GameObject machine = Instantiate(machinePrefab, position, Quaternion.identity);
        if (String.IsNullOrEmpty(machineName))
            machine.name = "Machine" + machineId++;
        else
            machine.name = machineName;
        machine.transform.parent = transform;
        Machine machineComponent = machine.GetComponent<Machine>();
        machineComponent.Recipes = Recipes;
        return machineComponent;
    }
    public void DestroyAllMachines()
    {
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }
    }
}
