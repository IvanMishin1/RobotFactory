using System;
using System.Collections.Generic;
using UnityEngine;
using System.Text.Json;


public class SaveManager : MonoBehaviour
{
    [Serializable]
    public class RobotData
    {
        public string name;
        public float x;
        public float y;
        public bool paused;
        public bool stopped;

        public RobotData(string name, Vector2 position, bool paused, bool stopped)
        {
            this.name = name;
            this.x = position.x;
            this.y = position.y;
            this.paused = paused;
            this.stopped = stopped;
        }
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
                RobotData robotData = new RobotData(
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
}

