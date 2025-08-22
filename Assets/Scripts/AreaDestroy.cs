using UnityEngine;

public class AreaDestroy : MonoBehaviour
{
    private GameManager gameManager;
    private MoneyManager moneyManager;
    
    void Awake()
    {
        moneyManager = GameObject.Find("MoneyManager").GetComponent<MoneyManager>();
        gameManager = Object.FindFirstObjectByType<GameManager>();
    }
    
    void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Item"))
        {
            Item item = other.GetComponent<Item>();
            if (item != null && !item.transform.parent.CompareTag("Robot") && item.transform.parent != transform)
            {
                ItemExited(item);
                Destroy(other.gameObject);
            }
        }
    }
    
    public void ItemExited(Item item)
    {
        moneyManager.AddMoney((item.itemType switch
        {
            "ore" => 1,
            "ingot" => 2,
            _ => 0
        }));
    }
}
