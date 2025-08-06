using UnityEngine;

public class AreaDestroy : MonoBehaviour
{
    private GameManager gameManager;
    private void Start()
    {
        gameManager = Object.FindFirstObjectByType<GameManager>();
    }
    
    void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Item"))
        {
            Item item = other.GetComponent<Item>();
            if (item != null && !item.transform.parent.CompareTag("Robot") && item.transform.parent != transform)
            {
                gameManager.ItemExited(item);
                Destroy(other.gameObject);
            }
        }
    }
}
