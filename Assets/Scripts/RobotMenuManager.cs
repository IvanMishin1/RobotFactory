using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class RobotMenuManager : MonoBehaviour
{
    Robot selectedRobot = null;
    public TMP_Text stopResumeText;
    public TMP_Text pauseUnpauseText;
   CodeEditorManager codeEditorManager;
   public GameObject robotMenu;
   private Camera camera;
   private GameManager gameManager;
   
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        codeEditorManager = GetComponent<CodeEditorManager>();
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        camera = Camera.main;
        robotMenu.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
                return;
            
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction);

            if (hit.collider != null)
            {
                if (hit.collider.gameObject.CompareTag("Robot"))
                {
                    gameManager.onRobotMenuOpened.Invoke();
                    selectedRobot = hit.collider.gameObject.GetComponent<Robot>();
                    robotMenu.transform.position = camera.WorldToScreenPoint(selectedRobot.transform.position);
                    robotMenu.SetActive(true);
                    //codeEditorManager.OpenEditor(selectedRobot);
                }
            }
            else
            {
                robotMenu.SetActive(false); 
                selectedRobot = null;
            }
        }

        if (selectedRobot != null)
        {
            if (selectedRobot.stop)
                stopResumeText.text = "Resume execution";
            else if (!selectedRobot.stop)
                stopResumeText.text = "Stop execution";
            
            if (selectedRobot.pause)
                pauseUnpauseText.text = "Unpause execution";
            else if (!selectedRobot.pause)
                pauseUnpauseText.text = "Pause execution";
        }
    }
    public void OpenEditor()
    {
        if (selectedRobot != null)
        {
            codeEditorManager.OpenEditor(selectedRobot);
            robotMenu.SetActive(false);
        }
    }
    
    public void StopResumeRobot()
    {
        if (selectedRobot != null)
        {
            if (selectedRobot.stop)
                selectedRobot.stop = false;
            else
                selectedRobot.stop = true;
        }
    }

    public void PauseUnpauseRobot()
    {
        if (selectedRobot != null)
        {
            if (selectedRobot.pause)
                selectedRobot.pause = false;
            else
                selectedRobot.pause = true;
        }
    }
}
