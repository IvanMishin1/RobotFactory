using System;
using UnityEngine;

public class RobotManager : MonoBehaviour
{
    private int robotId = 0; // TODO: This should be stored
    private GameObject robotPrefab;
    void Awake()
    {
        // Awake to ensure the prefab is loaded before any robots are created
        robotPrefab = Resources.Load<GameObject>("Prefabs/Robot");
        if (robotPrefab == null)
            Debug.LogError("Robot prefab not found in Resources/Prefabs/Robot");
    }
    public Robot CreateRobot(Vector2 position, string robotName = null)
    {
        GameObject robot = Instantiate(robotPrefab, position, Quaternion.identity, transform);
        if (String.IsNullOrEmpty(robotName))
            robot.name = "Robot" + robotId++;
        else
            robot.name = robotName;
        robot.transform.position = position;
        return robot.GetComponent<Robot>();
    }
    public void DestroyRobot(Robot robot)
    {
        if (robot != null)
        {
            Destroy(robot.gameObject);
        }
    }
    public void DestroyAllRobots()
    {
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }
    }
}
