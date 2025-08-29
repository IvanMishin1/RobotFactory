using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System.Text.Json;

public class SaveManager : MonoBehaviour
{
    private RobotManager robotManager;
    private GameContext gameContext;
    private WallManager wallManager;
    private MoneyManager moneyManager;
    private MachineManager machineManager;
    private AreaManager areaManager;
    private TimeManager timeManager;
    
    [Serializable]
    public class RobotData
    {
        public string name;
        public float x;
        public float y;
        public bool paused;
        public bool stopped;
        
        public void SetValues(string name, Vector2 position, bool paused, bool stopped)
        {
            this.name = name;
            this.x = position.x;
            this.y = position.y;
            this.paused = paused;
            this.stopped = stopped;
        }
    }

    public class MachineData
    {
        public string name;
        public float x;
        public float y;
        public Dictionary<string, string> recipes;
        
        public void SetValues(string name, Vector2 position, Dictionary<string, string> recipes)
        {
            this.name = name;
            this.x = position.x;
            this.y = position.y;
            this.recipes = recipes;
        }
    }

    public class AreaData
    {
        public string name;
        public float x;
        public float y;
        public List<Area.InputData> input;
        public List<Area.OutputData> output;
        
        public void SetValues(string name, Vector2 position, List<Area.InputData> input, List<Area.OutputData> output)
        {
            this.name = name;
            this.x = position.x;
            this.y = position.y;
            this.input = input;
            this.output = output;
        }
    }

    [Serializable]
    public class SaveInfoData
    {
        public long money;
        public WallRectInt wallRect;
        public int day;
        public int hour;
        public void SetValues(long money, RectInt wallRect, int day, int hour)
        {
            this.money = money;
            this.day = day;
            this.hour = hour;
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
        machineManager = GameObject.Find("MachineManager").GetComponent<MachineManager>();
        areaManager = GameObject.Find("AreaManager").GetComponent<AreaManager>();
        timeManager = GameObject.Find("TimeManager").GetComponent<TimeManager>();
    }
    
    public void SaveGame(string gameName = null)
    {
        if (string.IsNullOrWhiteSpace(gameName))
        {
            gameName = gameContext.gameName;
            if (string.IsNullOrEmpty(gameName))
                throw new ArgumentException("gameName cannot be null, empty, or whitespace.", nameof(gameName));
        }
        
        CreateDirectories(gameName);
        CreateFiles(gameName);
        
        // Save SaveInfo
        SaveInfoData saveInfoData = new SaveInfoData();
        var (day, hour) = timeManager.GetTime();
        saveInfoData.SetValues(moneyManager.Money, wallManager.wallRect, day, hour);
        string saveInfoJson = JsonSerializer.Serialize(saveInfoData, new JsonSerializerOptions { WriteIndented = true, IncludeFields = true });
        string saveInfoPath = Application.dataPath + "/Saves/" + gameName + "/saveinfo.json";
        File.WriteAllText(saveInfoPath, saveInfoJson);
        
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
        
        // Save machines
        GameObject[] machines = GameObject.FindGameObjectsWithTag("Machine");
        List<MachineData> machinesData = new List<MachineData>();
        foreach (GameObject machineObject in machines)
        {
            Machine machineComponent = machineObject.GetComponent<Machine>();
            if (machineComponent != null)
            {
                MachineData machineData = new MachineData();
                machineData.SetValues(
                    machineComponent.name,
                    machineComponent.transform.position,
                    machineComponent.Recipes
                );
                machinesData.Add(machineData);
            }
        }
        string machinesJson = JsonSerializer.Serialize(machinesData, new JsonSerializerOptions { WriteIndented = true, IncludeFields = true });
        string machinesPath = Application.dataPath + "/Saves/" + gameName + "/machines.json";
        File.WriteAllText(machinesPath, machinesJson);
        
        // Save areas
        GameObject[] areas = GameObject.FindGameObjectsWithTag("Area");
        List<AreaData> areasData = new List<AreaData>();
        foreach (GameObject areaObject in areas)
        {
            Area areaComponent = areaObject.GetComponent<Area>();
            if (areaComponent != null)
            {
                AreaData areaData = new AreaData();
                areaData.SetValues(
                    areaComponent.name,
                    areaComponent.transform.position,
                    areaComponent.Input,
                    areaComponent.Output
                );
                areasData.Add(areaData);
            }
        }
        string areasJson = JsonSerializer.Serialize(areasData, new JsonSerializerOptions { WriteIndented = true, IncludeFields = true });
        string areasPath = Application.dataPath + "/Saves/" + gameName + "/areas.json";
        File.WriteAllText(areasPath, areasJson);
        
        // Save Lua scripts
        foreach (var file in new DirectoryInfo(Application.dataPath + "/Saves/Temp").GetFiles("*.lua"))
            file.CopyTo(Application.dataPath + "/Saves/" + gameName + "/" + file.Name, true);        
	}

    public void LoadGame(string gameName)
    {
        if (string.IsNullOrWhiteSpace(gameName))
            throw new ArgumentException("gameName cannot be null, empty, or whitespace.", nameof(gameName));
        
        CreateDirectories(gameName);
        CreateFiles(gameName);
        
        // Load SaveInfo
        string saveInfoJson = File.ReadAllText(Application.dataPath + "/Saves/"+ gameName +"/saveinfo.json");
        SaveInfoData saveInfoData = JsonSerializer.Deserialize<SaveInfoData>(saveInfoJson, new JsonSerializerOptions { IncludeFields = true });
        moneyManager.Money = saveInfoData.money;
        timeManager.SetTime(saveInfoData.day, saveInfoData.hour); // TODO: Validate this
        wallManager.wallRect = new RectInt(
            saveInfoData.wallRect.x,
            saveInfoData.wallRect.y,
            saveInfoData.wallRect.width,
            saveInfoData.wallRect.height
        );
        
        // Load robots
        string robotsJson = File.ReadAllText(Application.dataPath + "/Saves/"+ gameName +"/robots.json");
        List<RobotData> robotsData = JsonSerializer.Deserialize<List<RobotData>>(robotsJson, new JsonSerializerOptions { IncludeFields = true });
        robotManager.DestroyAllRobots();
        foreach (var robot in robotsData)
        {
            Robot robotComponent = robotManager.CreateRobot(new Vector2(robot.x, robot.y), robot.name);
            robotComponent.Pause = robot.paused;
            robotComponent.Stop = robot.stopped;
        }
        
        // Load machines
        string machinesJson = File.ReadAllText(Application.dataPath + "/Saves/"+ gameName +"/machines.json");
        List<MachineData> machinesData = JsonSerializer.Deserialize<List<MachineData>>(machinesJson, new JsonSerializerOptions { IncludeFields = true });
        machineManager.DestroyAllMachines();
        foreach (var machine in machinesData)
        {
            machineManager.CreateMachine(new Vector2(machine.x, machine.y), machine.recipes, machine.name);
        }
        
        // Load areas
        string areasJson = File.ReadAllText(Application.dataPath + "/Saves/"+ gameName +"/areas.json");
        List<AreaData> areasData = JsonSerializer.Deserialize<List<AreaData>>(areasJson, new JsonSerializerOptions { IncludeFields = true });
        areaManager.DestroyAllAreas();
        foreach (var area in areasData)
        {
            areaManager.CreateArea(new Vector2(area.x, area.y), area.input, area.output, area.name);
        }
        
        // Load Lua scripts
        ClearTempDirectory();
        foreach (var file in new DirectoryInfo(Application.dataPath + "/Saves/" + gameName).GetFiles("*.lua"))
            file.CopyTo(Application.dataPath + "/Saves/Temp/" + file.Name, true);
        
        Time.timeScale = 1f;
    }
    
    public void CreateGame(string gameName)
    {
        if (string.IsNullOrWhiteSpace(gameName))
            throw new ArgumentException("gameName cannot be null, empty, or whitespace.", nameof(gameName));
        
        CreateDirectories(gameName);
        CreateFiles(gameName);
        ClearTempDirectory();
        
        wallManager.wallRect = new RectInt(-4,-2,7,5);
        moneyManager.Money = 0;
        robotManager.CreateRobot(new Vector2(0, 0));
        machineManager.CreateMachine(new Vector2(0, 2), new Dictionary<string, string>()
        {
            {"ore", "ingot"},
            {"ingot", "ore"}
        });
        
        areaManager.CreateArea(new Vector2(2, 0), new List<Area.InputData> { new Area.InputData { itemType = "ingot", value = 2 } }, null);
        areaManager.CreateArea(new Vector2(-2, 0), null, new List<Area.OutputData> { new Area.OutputData { itemType = "ore", amount = 64 } });
        
        SaveGame();
    }
    
    private void CreateDirectories(string gameName)
    {
        if (!Directory.Exists(Application.dataPath + "/Saves/"))
            Directory.CreateDirectory(Application.dataPath + "/Saves/");
        if (!Directory.Exists(Application.dataPath + "/Saves/Temp/"))
            Directory.CreateDirectory(Application.dataPath + "/Saves/Temp/");
        if (!Directory.Exists(Application.dataPath + "/Saves/" + gameName))
            Directory.CreateDirectory(Application.dataPath + "/Saves/" + gameName);
    }

    private void CreateFiles(string gameName)
    {
        if (!File.Exists(Application.dataPath + "/Saves/" + gameName + "/robots.json"))
            File.WriteAllText(Application.dataPath + "/Saves/" + gameName + "/robots.json", "[]");
        if (!File.Exists(Application.dataPath + "/Saves/" + gameName + "/machines.json"))
            File.WriteAllText(Application.dataPath + "/Saves/" + gameName + "/machines.json", "[]");
        if (!File.Exists(Application.dataPath + "/Saves/" + gameName + "/areas.json"))
            File.WriteAllText(Application.dataPath + "/Saves/" + gameName + "/areas.json", "[]");
        if (!File.Exists(Application.dataPath + "/Saves/" + gameName + "/saveinfo.json"))
            File.WriteAllText(Application.dataPath + "/Saves/" + gameName + "/saveinfo.json", "[]");
    }
    
    private void ClearTempDirectory()
    {
        if (!Directory.Exists(Application.dataPath + "/Saves/Temp/"))
            return;
        foreach (var file in new DirectoryInfo(Application.dataPath + "/Saves/Temp/").GetFiles("*"))
            file.Delete();
    }
}