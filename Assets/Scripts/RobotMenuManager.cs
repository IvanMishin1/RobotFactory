using UnityEngine;
using UnityEngine.EventSystems;

public class RobotMenuManager : MonoBehaviour
{
    Robot selectedRobot = null;
   CodeEditorManager codeEditorManager;
   public GameObject robotMenu;
   private Camera camera;
   
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        codeEditorManager = GetComponent<CodeEditorManager>();
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
                Debug.Log("1");
                if (hit.collider.gameObject.CompareTag("Robot"))
                {
                    Debug.Log("2");
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
    }
    public void OpenEditor()
    {
        if (selectedRobot != null)
        {
            codeEditorManager.OpenEditor(selectedRobot);
            robotMenu.SetActive(false);
        }
    }
}
