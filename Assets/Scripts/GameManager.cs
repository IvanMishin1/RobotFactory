using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    string gameType = "demo";
    float money = 0f;
    public TMP_Text moneyText;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        var r = Object.Instantiate(Resources.Load("Prefabs/Robot"), new Vector3(0, 0, 0), Quaternion.identity);
        r.name = "Robot1";
        r.GetComponent<Transform>().position = GameObject.Find("Robots").transform.position;
        r.GetComponent<Transform>().parent = GameObject.Find("Robots").transform;
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
