using System;
using UnityEngine;

public class Machine : MonoBehaviour
{
    public Item[] items;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        foreach (Transform child in transform)
        {
            if (child.CompareTag("Item"))
            {
                Rigidbody2D rb = child.GetComponent<Rigidbody2D>();
                rb.AddForce(child.GetComponent<Rigidbody2D>().linearVelocity = new Vector2(1, 0) * Time.deltaTime * 0.1f, ForceMode2D.Force);
                if (child.position.x > 0f)
                {
                    // TODO: Make this based on a file
                    if (child.GetComponent<Item>().itemType == "ore")
                        child.GetComponent<Item>().SetItemType("ingot");
                    else if (child.GetComponent<Item>().itemType == "ingot")
                        child.GetComponent<Item>().SetItemType("ore");
                }
            }
        }
    }
    
    void OnTriggerStay2D(Collider2D other)
    {
        Debug.Log("OnTriggerStay2D");
        if (other.CompareTag("Item"))
        {
            Item item = other.GetComponent<Item>();
            if (item != null && !item.transform.parent.CompareTag("Robot") && item.transform.parent != transform)
            {
                item.transform.SetParent(transform);
            }
        }
    }
    
    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Item") && other.transform.parent == transform)
        {
            Item item = other.GetComponent<Item>();
            if (item != null)
            {
                item.transform.parent = GameObject.Find("Items").transform;
                Rigidbody2D rb = other.GetComponent<Rigidbody2D>();
                rb.linearVelocity = Vector2.zero;
            }
        }
    }
}