using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

namespace UI
{
    public class LoadSaveMenuManager : MonoBehaviour
    {
        public GameObject newGameMenu, loadGameMenu;

        public TMP_InputField createGameNameInput;
        public TMP_Dropdown loadGameDropdown;
        public TMP_Text errorText;
        private GameContext gameContext;

        public void Awake()
        {
            gameContext = GameObject.Find("GameContext").GetComponent<GameContext>();
        }
        
        public void OpenNewGameMenu()
        {
            newGameMenu.SetActive(true);
        }

        public void OpenLoadGameMenu()
        {
            loadGameMenu.SetActive(true);
            loadGameDropdown.ClearOptions();
            List<string> gameNames = GetGameNames();
            loadGameDropdown.AddOptions(gameNames);
        }

        public void CreateGame()
        {
            string gameName = createGameNameInput.text.Trim();
            if (string.IsNullOrEmpty(gameName))
            {
                errorText.text = "Game name cannot be empty.";
                return;
            }

            if (GetGameNames().Contains(gameName))
            {
                errorText.text = "Game name already exists. Please choose a different name.";
                return;
            }
            
            if (!Regex.IsMatch(gameName, @"^[a-zA-Z0-9 ]+$"))
            {
                errorText.text = "Game name may contain only letters, digits, and spaces.";
                return;
            }
            
            gameContext.gameName = gameName;
            gameContext.isNewGame = true;
            gameContext.isTutorial = false;
            SceneManager.LoadScene("Game");
        }

        public void LoadGame()
        {
            if (!GetGameNames().Contains(loadGameDropdown.options[loadGameDropdown.value].text))
            {
                Debug.LogWarning("Selected game does not exist.");
                return;
            }
            gameContext.gameName = loadGameDropdown.options[loadGameDropdown.value].text;
            gameContext.isNewGame = false;
            gameContext.isTutorial = false;
            SceneManager.LoadScene("Game"); 
        }

        public void PlayTutorial()
        {
            gameContext.gameName = "Tutorial";
            gameContext.isNewGame = false;
            gameContext.isTutorial = true;
            SceneManager.LoadScene("Game");
        }
    
        public List<string> GetGameNames()
        {
            if (!System.IO.Directory.Exists(Application.dataPath + "/Saves"))
            {
                Debug.LogWarning("Saves directory does not exist, creating it.");
                System.IO.Directory.CreateDirectory(Application.dataPath + "/Saves");
            }

            return Directory.EnumerateDirectories(Application.dataPath + "/Saves/", "*", SearchOption.TopDirectoryOnly)
                .Select(Path.GetFileName)
                .Where(name => !string.Equals(name, "Temp", StringComparison.OrdinalIgnoreCase))
                .ToList();
        }
    }
}