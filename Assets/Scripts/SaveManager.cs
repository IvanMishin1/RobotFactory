using System;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using System.Text.Json;

public class SaveManager : MonoBehaviour
{
    private RobotManager robotManager;
    private GameContext gameContext;
    private WallManager wallManager;
    private MoneyManager moneyManager;
    
    [Serializable]
    public class RobotData
    {
        public string name;
        public float x;
        public float y;
        public bool paused;
        public bool stopped;
        
        public RobotData() {}
        
        public void SetValues(string name, Vector2 position, bool paused, bool stopped)
        {
            this.name = name;
            this.x = position.x;
            this.y = position.y;
            this.paused = paused;
            this.stopped = stopped;
        }
    }
    
    public class ItemData
    {
        public string name;
        public string type;
        public float x;
        public float y;
        public string robot;
        
        public ItemData() {}
        
        public void SetValues(string name, string type, Vector2 position, string robot = null)
        {
            this.name = name;
            this.type = type;
            this.robot = robot;
            this.x = position.x;
            this.y = position.y;
        }
    }

    [Serializable]
    public class SaveInfoData
    {
        public long money;
        public WallRectInt wallRect;
        public SaveInfoData() {}
        public void SetValues(long money, RectInt wallRect)
        {
            this.money = money;
            this.wallRect = new WallRectInt
            {
                x = wallRect.x,
                y = wallRect.y,
                width = wallRect.width,
                height = wallRect.height
            };
        }
        public class WallRectInt
        {
            public int x;
            public int y;
            public int width;
            public int height;
        }
    }
    public void Awake()
    {
        robotManager = GameObject.Find("RobotManager").GetComponent<RobotManager>();
        gameContext = GameObject.Find("GameContext").GetComponent<GameContext>();
        wallManager = GameObject.Find("WallManager").GetComponent<WallManager>();
        moneyManager = GameObject.Find("MoneyManager").GetComponent<MoneyManager>();
    }
    
    public void SaveGame(string gameName = null)
    {
        if (string.IsNullOrWhiteSpace(gameName))
        {
            gameName = gameContext.gameName;
            if (string.IsNullOrEmpty(gameName))
                throw new ArgumentException("gameName cannot be null, empty, or whitespace.", nameof(gameName));
        }
        if (!Directory.Exists(Application.dataPath + "/Saves/" + gameName))
            Directory.CreateDirectory(Application.dataPath + "/Saves/" + gameName);
        
        // Save robots
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
        string robotsJson = JsonSerializer.Serialize(robotsData, new JsonSerializerOptions { WriteIndented = true, IncludeFields = true });
        string robotsPath = Application.dataPath + "/Saves/" + gameName + "/robots.json";
        File.WriteAllText(robotsPath, robotsJson);
        
        // Save SaveInfo
        SaveInfoData saveInfoData = new SaveInfoData();
        saveInfoData.SetValues(moneyManager.Money, wallManager.wallRect);
        string saveInfoJson = JsonSerializer.Serialize(saveInfoData, new JsonSerializerOptions { WriteIndented = true, IncludeFields = true });
        string saveInfoPath = Application.dataPath + "/Saves/" + gameName + "/saveinfo.json";
        File.WriteAllText(saveInfoPath, saveInfoJson);
        
        // Save Lua scripts
        foreach (var file in new DirectoryInfo(Application.dataPath + "/Saves/Temp").GetFiles("*.lua"))
            file.CopyTo(Application.dataPath + "/Saves/" + gameName + "/" + file.Name, true);        
	}

    public void LoadGame(string gameName)
    {
        if (string.IsNullOrWhiteSpace(gameName))
            throw new ArgumentException("gameName cannot be null, empty, or whitespace.", nameof(gameName));
        
        // Creating missing directories
        if (!Directory.Exists(Application.dataPath + "/Saves/"))
            Directory.CreateDirectory(Application.dataPath + "/Saves/");
        if (!Directory.Exists(Application.dataPath + "/Saves/Temp/"))
            Directory.CreateDirectory(Application.dataPath + "/Saves/Temp/");
        if (!Directory.Exists(Application.dataPath + "/Saves/" + gameName))
            Directory.CreateDirectory(Application.dataPath + "/Saves/" + gameName);
        
        // Load robots
        string robotsJson = File.ReadAllText(Application.dataPath + "/Saves/"+ gameName +"/robots.json"); // TODO: Handle file not found
        List<RobotData> robotsData = JsonSerializer.Deserialize<List<RobotData>>(robotsJson, new JsonSerializerOptions { IncludeFields = true });
        robotManager.DestroyAllRobots();
        foreach (var robot in robotsData)
        {
            Robot robotComponent = robotManager.CreateRobot(new Vector2(robot.x, robot.y), robot.name);
            robotComponent.Pause = robot.paused;
            robotComponent.Stop = robot.stopped;
        }

        // Load SaveInfo
        string saveInfoJson = File.ReadAllText(Application.dataPath + "/Saves/"+ gameName +"/saveinfo.json"); // TODO: Handle file not found
        SaveInfoData saveInfoData = JsonSerializer.Deserialize<SaveInfoData>(saveInfoJson, new JsonSerializerOptions { IncludeFields = true });
        moneyManager.Money = saveInfoData.money;
        wallManager.wallRect = new RectInt(
            saveInfoData.wallRect.x,
            saveInfoData.wallRect.y,
            saveInfoData.wallRect.width,
            saveInfoData.wallRect.height
        );
        
        // Load Lua scripts
        foreach (var file in new DirectoryInfo(Application.dataPath + "/Saves/Temp/").GetFiles("*"))
            file.Delete();
        foreach (var file in new DirectoryInfo(Application.dataPath + "/Saves/" + gameName).GetFiles("*.lua"))
            file.CopyTo(Application.dataPath + "/Saves/Temp/" + file.Name, true);
        
        Time.timeScale = 1f;
    }
    
    public void CreateGame(string gameName)
    {
        if (string.IsNullOrWhiteSpace(gameName))
            throw new ArgumentException("gameName cannot be null, empty, or whitespace.", nameof(gameName));
        
        foreach (var file in new DirectoryInfo(Application.dataPath + "/Saves/Temp/").GetFiles("*"))
            file.Delete();
        
        Directory.CreateDirectory(Application.dataPath + "/Saves/" + gameName);
        File.WriteAllText(Application.dataPath + "/Saves/" + gameName + "/robots.json", "[]");
        File.WriteAllText(Application.dataPath + "/Saves/" + gameName + "/saveinfo.json", "[]");
        robotManager.CreateRobot(new Vector2(0, 0));
        
        var wallManager = GameObject.Find("WallManager").GetComponent<WallManager>();
        wallManager.wallRect = new RectInt(-4,-2,7,5);
        moneyManager.Money = 0;
        SaveGame();
    }
}

