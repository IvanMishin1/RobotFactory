using UnityEngine;
using UnityEngine.EventSystems;

public class RobotMenuManager : MonoBehaviour
{
    Robot selectedRobot = null;
   CodeEditorManager codeEditorManager;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        codeEditorManager = GetComponent<CodeEditorManager>();
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
                if (hit.collider.CompareTag("Robot"))
                {
                    selectedRobot = hit.collider.gameObject.GetComponent<Robot>();
                    Debug.Log($"Robot clicked: {selectedRobot.Name}");
                    codeEditorManager.OpenEditor(selectedRobot);
                }
            }
        }
    }
}
