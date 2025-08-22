using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenuManager : MonoBehaviour
{
    public GameObject pauseMenu;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // TODO: Maybe check if game was saved at pause??
        pauseMenu.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        // TODO: Maybe migrate this to an input manager?
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePauseMenu();
        }
    }
    
    public void TogglePauseMenu()
    {
        if (pauseMenu.activeSelf)
        {
            pauseMenu.SetActive(false);
            Time.timeScale = 1f;
        }
        else
        {
            pauseMenu.SetActive(true);
            Time.timeScale = 0f;
        }
    }
    public void QuitToMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void Load()
    {
        
    }
}
