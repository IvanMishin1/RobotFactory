using System;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private GameContext gameContext;
    private SaveManager saveManager;
    
	void Awake()
	{
        saveManager = GameObject.Find("SaveManager").GetComponent<SaveManager>();
        
        GameObject gameContextObj = GameObject.Find("GameContext");
        if (gameContextObj != null)
            gameContext = gameContextObj.GetComponent<GameContext>();
        else
            gameContext = null;
	}
 
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (gameContext == null)
        {
            Debug.LogWarning("GameContext not found. Loading MainMenu scene.");
            SceneManager.LoadScene("MainMenu");
            return;
        }
        Debug.Log($"Loading {gameContext.gameName}. New Game: {gameContext.isNewGame}");
        if (gameContext.isNewGame)
        {
            saveManager.CreateGame(gameContext.gameName);
        }
        if (!gameContext.isNewGame)
        {
            saveManager.LoadGame(gameContext.gameName);
        }
    }
}