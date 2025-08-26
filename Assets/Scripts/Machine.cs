using System;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Machine : MonoBehaviour
{
    public enum MachineRole { In, Out, Machine }
    public MachineRole role;
    public AudioSource audioSource;
    public AudioClip[] clips;
    [SerializeField] private GameObject itemsContainer;
    private ItemManager itemManager;
    public Dictionary<string, string> Recipes = new Dictionary<string, string>()
    {
        {"ore", "ingot"},
        {"ingot", "ore"}
    };
    void Awake()
    {
        itemManager = GameObject.Find("ItemManager").GetComponent<ItemManager>();
        if (itemsContainer == null)
            Debug.LogError("Machine has no items container");
    }

    // Update is called once per frame
    void Update()
    {
        if (role == MachineRole.Machine)
        {
            foreach (Transform child in itemsContainer.transform)
            {
                if (child.CompareTag("Item"))
                {
                    Rigidbody2D rb = child.GetComponent<Rigidbody2D>();
                    rb.AddForce(child.GetComponent<Rigidbody2D>().linearVelocity = new Vector2(1, 0) * Time.deltaTime * 0.1f, ForceMode2D.Force);
                    if (child.localPosition.x > 0f)
                    {
                        Item item = child.GetComponent<Item>();
                        if (!item.hasBeenProcessed)
                        {
                            Recipes.TryGetValue(item.type, out string result);
                            if (!String.IsNullOrEmpty(result))
                                item.SetItemType(result);
                            item.hasBeenProcessed = true;
                        }
                    }

                    if (child.localPosition.x > -0.5f && child.localPosition.x < 0.5f)
                    {
                        if (audioSource != null && !audioSource.isPlaying)
                        {
                            audioSource.pitch = Time.timeScale;
                            audioSource.PlayOneShot(clips[Random.Range(0, clips.Length)]);
                        }
                    }
                }
            }
        }
    }
    
    void OnTriggerStay2D(Collider2D other)
    {
        if (role == MachineRole.In)
        {
            if (other.CompareTag("Item"))
            {
                Item item = other.GetComponent<Item>();
                if (item != null && !item.transform.parent.CompareTag("Robot") && item.transform.parent != itemsContainer.transform)
                {
                    item.transform.SetParent(itemsContainer.transform);
                }
            }
        }
    }
    
    void OnTriggerExit2D(Collider2D other)
    {
        if (role == MachineRole.Out)
        {
            if (other.CompareTag("Item") && other.transform.parent == itemsContainer.transform)
            {
                Item item = other.GetComponent<Item>();
                if (item != null)
                {
                    item.transform.parent = itemManager.transform;
                    Rigidbody2D rb = other.GetComponent<Rigidbody2D>();
                    rb.linearVelocity = Vector2.zero;
                    item.hasBeenProcessed = false;
                }
            }
        }
    }
}