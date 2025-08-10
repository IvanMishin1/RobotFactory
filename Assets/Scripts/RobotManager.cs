using UnityEngine;

public class RobotManager : MonoBehaviour
{
    private int robotId = 0;
    public GameObject robotPrefab;
    void Start()
    {
        robotPrefab = Resources.Load<GameObject>("Prefabs/Robot");
    }
    public GameObject CreateRobot(Vector2 position)
    {
        GameObject robot = Instantiate(robotPrefab, position, Quaternion.identity);
        robot.name = "Robot" + robotId++;
        robot.transform.position = position;
        robot.transform.parent = transform;
        return robot;
    }
}
