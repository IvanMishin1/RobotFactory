using System.Collections.Generic;
using UnityEngine;

public class AreaDestroy : MonoBehaviour
{
    private GameManager gameManager;
    private MoneyManager moneyManager;
    public Dictionary<string, int> itemValues;
    
    void Awake()
    {
        moneyManager = GameObject.Find("MoneyManager").GetComponent<MoneyManager>();
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
    }
    
    void OnTriggerStay2D(Collider2D itemGameObject)
    {
        if (!itemGameObject.CompareTag("Item"))
            return;
        
        Item item = itemGameObject.GetComponent<Item>();
        if (item != null && !item.transform.parent.CompareTag("Robot") && item.transform.parent != transform)
        {
            itemValues.TryGetValue(item.type, out int value);
            if (value > 0)
            {
                moneyManager.AddMoney(value);
                Destroy(item.gameObject);
            }
        }
        
    }
}
