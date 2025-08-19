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

    public void Awake()
    {
        robotManager = GameObject.Find("RobotManager").GetComponent<RobotManager>();
    }
    
    public void SaveGame(string gameName)
    {
        if (string.IsNullOrWhiteSpace(gameName))
            throw new ArgumentException("gameName cannot be null, empty, or whitespace.", nameof(gameName));
        
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
				robotsData.Add(robotData);
            }
        }
        
        string json = JsonSerializer.Serialize(robotsData, new JsonSerializerOptions { WriteIndented = true, IncludeFields = true });
        string robotsPath = Application.dataPath + "/Saves/" + gameName + "/robots.json";
        
        if (!Directory.Exists(Application.dataPath + "/Saves/" + gameName))
            Directory.CreateDirectory(Application.dataPath + "/Saves/" + gameName);
        File.WriteAllText(robotsPath, json);
	}

    public void LoadGame(string gameName)
    {
        if (string.IsNullOrWhiteSpace(gameName))
            throw new ArgumentException("gameName cannot be null, empty, or whitespace.", nameof(gameName));
        
        string json = File.ReadAllText(Application.dataPath + "/Saves/"+ gameName +"/robots.json");
        List<RobotData> robotsData = JsonSerializer.Deserialize<List<RobotData>>(json, new JsonSerializerOptions { IncludeFields = true });
        
        robotManager.DestroyAllRobots();
        foreach (var robot in robotsData)
        {
            robotManager.CreateRobot(new Vector2(robot.x, robot.y), robot.name);
        }
    }
    
    public void CreateGame(string gameName)
    {
        if (string.IsNullOrWhiteSpace(gameName))
            throw new ArgumentException("gameName cannot be null, empty, or whitespace.", nameof(gameName));
        
        Directory.CreateDirectory(Application.dataPath + "/Saves/" + gameName);
        File.WriteAllText(Application.dataPath + "/Saves/" + gameName + "/robots.json", "[]");
        File.WriteAllText(Application.dataPath + "/Saves/" + gameName + "/saveinfo.json", "[]");
        robotManager.CreateRobot(new Vector2(0, 0));
    }
}

