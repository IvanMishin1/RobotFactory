using System;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using System.Text.Json;

public class SaveManager : MonoBehaviour
{
    private RobotManager robotManager;
    
    [Serializable]
    public class RobotData
    {
        public string name;
        public float x;
        public float y;
        public bool paused;
        public bool stopped;
        
        public RobotData()
        {
        }
        
        public void SetValues(string name, Vector2 position, bool paused, bool stopped)
        {
            this.name = name;
            this.x = position.x;
            this.y = position.y;
            this.paused = paused;
            this.stopped = stopped;
        }
    }

    public void Start()
    {
        robotManager = GameObject.Find("RobotManager").GetComponent<RobotManager>();
    }
    
    public void SaveGame()
    {
        GameObject[] robots = GameObject.FindGameObjectsWithTag("Robot");
        List<RobotData> robotsData = new List<RobotData>();
        
        foreach (GameObject robotObject in robots)
        {
            Robot robotComponent = robotObject.GetComponent<Robot>();
            if (robotComponent != null)
            {
                RobotData robotData = new RobotData();
                robotData.SetValues(
                    robotComponent.name,
                    robotComponent.startingPosition,
                    robotComponent.Pause,
                    robotComponent.Stop
                );
                Debug.Log("Saving robot: " + robotData.name);
				robotsData.Add(robotData);
            }
        }
        string json = JsonSerializer.Serialize(robotsData, new JsonSerializerOptions { WriteIndented = true, IncludeFields = true });
        string path = Application.dataPath + "/Saves/robots.json";

        if (!System.IO.Directory.Exists(Application.dataPath + "/Saves/"))
			System.IO.Directory.CreateDirectory(Application.dataPath + "/Saves/");
		else
		{
			System.IO.Directory.CreateDirectory(Application.dataPath + "/Saves/");
        	System.IO.File.WriteAllText(path, json);
		}
	}

    public void LoadGame(bool viewOnly = false)
    {
        string json = File.ReadAllText(Application.dataPath + "/Saves/robots.json");
        List<RobotData> robotsData = JsonSerializer.Deserialize<List<RobotData>>(json);

        if (!viewOnly)
        {
            robotManager.DestroyAllRobots();
            foreach (var robot in robotsData)
            {
                robotManager.CreateRobot(new Vector2(robot.x, robot.y));
            }
        }
    }
}

