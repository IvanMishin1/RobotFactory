using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CodeEditorManager : MonoBehaviour
{
    public TMP_InputField codeText;

    public GameObject codeEditor;

    public Robot selectedRobot = null;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        codeEditor.gameObject.SetActive(false);
    }

    public void OpenEditor(Robot robot)
    {
        selectedRobot = robot;
        codeEditor.gameObject.SetActive(true);
        codeText.text = LoadCode(selectedRobot.gameObject.name);
    }

    public void SaveAndCloseEditor()
    {
        if (selectedRobot == null)
        {
            Debug.LogWarning("No robot selected to save code.");
            return;
        }
        SaveCode();
        codeEditor.gameObject.SetActive(false);
        selectedRobot = null;
    }
    
    public void RevertToSavedCode()
    {
        if (selectedRobot == null)
        {
            Debug.LogWarning("No robot selected to revert code.");
            return;
        }
        codeText.text = LoadCode(selectedRobot.gameObject.name);
    }
    
    private string LoadCode(string title)
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
    
    private void SaveCode()
    {
        string path = Application.dataPath + "/Saves/" + selectedRobot.gameObject.name + ".lua";
        File.WriteAllText(path, codeText.text);
        Debug.Log("Saved code : " + codeText.text + " to " + path);
    }
}
