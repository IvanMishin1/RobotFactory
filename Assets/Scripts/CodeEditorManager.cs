using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CodeEditorManager : MonoBehaviour
{
    public TMP_InputField codeText;
    public TMP_Dropdown dropdown;
    public GameObject codeEditor;

    public Robot selectedRobot = null;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        codeEditor.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {

    }
    
    public void OpenEditor(Robot robot)
    {
        codeText.text = string.Empty;
        selectedRobot = robot;
        codeEditor.gameObject.SetActive(true);
        
        // Updating the dropdown
        GameObject[] robots = GameObject.FindGameObjectsWithTag("Robot");
        dropdown.options.Clear();
        for (int i = 0; i < robots.Length; i++)
        {
            dropdown.options.Add(new TMP_Dropdown.OptionData(robots[i].name));
        }
        
        dropdown.value = dropdown.options.FindIndex(option => option.text == selectedRobot.name);
        dropdown.RefreshShownValue();

        //selectedRobot = dropdown.options[dropdown.value].text;
        Debug.LogWarning(codeText.text = LoadCode(selectedRobot.name));
        Debug.Log("Opened editor for: " + selectedRobot.name);
    }

    public void CloseEditor()
    {
        SaveCode();
        codeEditor.gameObject.SetActive(false);

    }

    public void ChangeRobot()
    {
        SaveCode();

        string selectedName = dropdown.options[dropdown.value].text;
        GameObject[] robots = GameObject.FindGameObjectsWithTag("Robot");
        GameObject foundRobot = null;
        foreach (GameObject robot in robots)
        {
            if (robot.gameObject.name == selectedName)
            {
                foundRobot = robot;
                break;
            }
        }
        if (foundRobot == null)
        {
            Debug.LogError("Robot not found: " + selectedName);
            codeText.text = string.Empty;
            return;
        }
        selectedRobot = foundRobot.GetComponent<Robot>();

        string loadedCode = LoadCode(selectedRobot.gameObject.name);
        Debug.Log($"Loaded code for {selectedRobot.gameObject.name}: '{loadedCode}'");
        codeText.text = loadedCode;
        Debug.Log("Changed text to: " + codeText.text);
    }

    
    public string LoadCode(string title)
    {
        string path = Application.dataPath + "/Saves/" + title + ".lua";
        if (File.Exists(path))
        {
            Debug.Log("Loaded Code : " + File.ReadAllText(path) + " from " + path);
            return File.ReadAllText(path);
        }
        File.WriteAllText(path, string.Empty);
        return string.Empty;
    }
    
    public void SaveCode()
    {
        string path = Application.dataPath + "/Saves/" + selectedRobot.gameObject.name + ".lua";
        File.WriteAllText(path, codeText.text);
        Debug.Log("Saved code : " + codeText.text + " to " + path);
    }
}
