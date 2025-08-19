using System;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    float money = 0f;
    public TMP_Text moneyText;
    
    private GameContext gameContext;
    private SaveManager saveManager;
    
	void Awake()
	{
        saveManager = GameObject.Find("GameManager").GetComponent<SaveManager>();
        gameContext = GameObject.Find("GameContext").GetComponent<GameContext>();
	}
 
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Debug.Log($"Loading {gameContext.gameName}. New Game: {gameContext.isNewGame}");
        if (gameContext.isNewGame)
        {
            saveManager.CreateGame(gameContext.gameName);
        }
        if (String.IsNullOrEmpty(gameContext.gameName) && Application.isEditor) // TODO: For testing without menu
            gameContext.gameName = "testsave1";
        if (!gameContext.isNewGame)
        {
            saveManager.LoadGame(gameContext.gameName);
        }
        moneyText.text = $"Net Gain: 0$";
    }

    public void ItemExited(Item item)
    {
        money += item.itemType switch
        {
            "ore" => 1f,
            "ingot" => 2f,
            _ => 0f
        };
        moneyText.text = $"Net Gain: {money}$";
    }
}