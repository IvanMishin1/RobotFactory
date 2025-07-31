using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CodeEditorManager : MonoBehaviour
{
    public TMP_Text codeText;
    public TMP_Dropdown dropdown;
    public GameObject codeEditor;
    
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
        Debug.Log("Opened Editor");
        codeEditor.gameObject.SetActive(true);
        
        // Updating the dropdown
        GameObject[] robots = GameObject.FindGameObjectsWithTag("Robot");
        dropdown.options.Clear();
        for (int i = 0; i < robots.Length; i++)
        {
            dropdown.options.Add(new TMP_Dropdown.OptionData(robots[i].name));
        }
        dropdown.value = dropdown.options.FindIndex(option => option.text == robot.name);
        dropdown.RefreshShownValue();
        
        // Loading code
        LoadCode(robot.transform.name);
        string selectedRobot = dropdown.options[dropdown.value].text;
        LoadCode(selectedRobot);
    }
    
    public void LoadCode(string title)
    {
        codeText.text = File.ReadAllText(Application.dataPath + "/" + title + ".txt");
    }
    
    public void SaveCode(string title)
    {
        File.WriteAllText(Application.dataPath + "/" + title + ".lua", codeText.text);
    }
}
