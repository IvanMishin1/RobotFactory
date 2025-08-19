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
    public int tutorialStep = 0;
    public TMP_Text tutorialText;
   
    [Header("Tutorial Events")]
    public UnityEvent onRobotMenuOpened = new UnityEvent();
    public UnityEvent onEditorOpened = new UnityEvent();
    public UnityEvent onGuideOpened = new UnityEvent();
    public UnityEvent onTutorialClicked = new UnityEvent();

    private RobotManager robotManager;
    private GameContext gameContext;
    private SaveManager saveManager;
    
	void Awake()
	{
	    onRobotMenuOpened.AddListener(HandleRobotMenuOpened);
    	onEditorOpened.AddListener(HandleEditorOpened);
    	onGuideOpened.AddListener(HandleGuideOpened);
        robotManager = GameObject.Find("RobotManager").GetComponent<RobotManager>();
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
            tutorialStep = 0;
            tutorialText.text = "Welcome to Robot Factory!\nClick here to continue";
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
        if (tutorialStep == 4 && money >= 6f)
        {
            TutorialStep();
        }
    }
    
    private void HandleRobotMenuOpened()
    {
        if (tutorialStep == 1)
        {
            TutorialStep();
        }
    }

    private void HandleEditorOpened()
    {
        if (tutorialStep == 2)
        {
            TutorialStep();
        }
    }

    private void HandleGuideOpened()
    {
        if (tutorialStep == 3)
        {
            TutorialStep();
        }
    }

    public void HandleTutorialClicked()
    {
        if (tutorialStep == 0 || tutorialStep == 5)
        {
            TutorialStep();
        }
    }
    
    void TutorialStep()
    {
        if (SceneManager.GetActiveScene().buildIndex == 1)
        {
            tutorialStep++;
            switch (tutorialStep)
            {
                case 0:
                    tutorialText.text = "This is your robot factory.\nAnd that orange thing with cute eyes is your robot!";
                    break;
                case 1:
                    tutorialText.text = "Left click on the robot to open its menu.";
                    break;
                case 2:
                    tutorialText.text = "Click on 'Edit Code'";
                    return;
                case 3:
                    tutorialText.text = "Here you can edit the robot's code which is written in Lua.\n" +
                                        "Now open the guide to learn how to control the robot.\n";
                    break;
                case 4:
                    tutorialText.text = "Your task is to earn 6$ by collecting ores and processing them into ingots.\n" +
                                        "You can do this by writing code for your robot.\n" +
                                        "All the documentation is available in the guide.\n" +
                                        "Complete this task to continue the tutorial.\n";
                    break;
                case 5:
                    tutorialText.text = "This was supposed to be the end of the game.\n" + 
                                        "But I decided to add a little extra content in the last hour of the Jam.\n" + 
                                        "Click here to continue.";
                    break;
                case 6:
                    SceneManager.LoadScene("Extra");
                    break;
            }
        }
    }
}
