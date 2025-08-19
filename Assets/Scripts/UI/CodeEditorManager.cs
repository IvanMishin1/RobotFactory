using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class CodeEditorManager : MonoBehaviour
    {
        public TMP_InputField codeText;

        public GameObject codeEditor;

        public Robot selectedRobot = null;
        private GameManager gameManager;

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Awake()
        {
            codeEditor.gameObject.SetActive(false);
            if (!Directory.Exists(Application.dataPath + "/Saves"))
            {
                Directory.CreateDirectory(Application.dataPath + "/Saves");
            }

            gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
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
            selectedRobot.Stop = false;
            selectedRobot.DisplayToggleStatus(null);
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
                return File.ReadAllText(path);
            }

            File.WriteAllText(path, string.Empty);
            return string.Empty;
        }

        private void SaveCode()
        {
            string path = Application.dataPath + "/Saves/" + selectedRobot.gameObject.name + ".lua";
            File.WriteAllText(path, codeText.text);
        }
    }
}