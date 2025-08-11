using UnityEngine;

public class RobotManager : MonoBehaviour
{
    private int robotId = 0;
    private GameObject robotPrefab;
    void Start()
    {
        robotPrefab = Resources.Load<GameObject>("Prefabs/Robot");
    }
    public GameObject CreateRobot(Vector2 position)
    {
        // TODO: Add more arguments
        GameObject robot = Instantiate(robotPrefab, position, Quaternion.identity);
        robot.name = "Robot" + robotId++;
        robot.transform.position = position;
        robot.transform.parent = transform;
        return robot;
    }
    public void DestroyRobot(Robot robot)
    {
        if (robot != null)
        {
            Destroy(robot);
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
