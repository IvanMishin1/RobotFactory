using UnityEngine;

public class Machine : MonoBehaviour
{
    public string role;
    public AudioSource audioSource;
    public AudioClip[] clips;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (role == "machine")
        {
            foreach (Transform child in transform)
            {
                if (child.CompareTag("Item"))
                {
                    Rigidbody2D rb = child.GetComponent<Rigidbody2D>();
                    rb.AddForce(child.GetComponent<Rigidbody2D>().linearVelocity = new Vector2(1, 0) * Time.deltaTime * 0.1f, ForceMode2D.Force);
                    if (child.position.x > 0f)
                    {
                        Item item = child.GetComponent<Item>();
                        if (!item.hasBeenProcessed)
                        {
                            if (item.itemType == "ore")
                                item.SetItemType("ingot");
                            else if (item.itemType == "ingot")
                                item.SetItemType("ore");
            
                            item.hasBeenProcessed = true;
                        }
                    }

                    if (child.position.x > -0.5f && child.position.x < 0.5f)
                    {
                        if (audioSource != null && !audioSource.isPlaying)
                        {
                            audioSource.PlayOneShot(clips[Random.Range(0, clips.Length)]);
                        }
                    }
                }
            }
        }
    }
    
    void OnTriggerStay2D(Collider2D other)
    {
        if (role == "in")
        {
            if (other.CompareTag("Item"))
            {
                Item item = other.GetComponent<Item>();
                if (item != null && !item.transform.parent.CompareTag("Robot") && item.transform.parent != transform.parent)
                {
                    item.transform.SetParent(transform.parent);
                }
            }
        }
    }
    
    void OnTriggerExit2D(Collider2D other)
    {
        if (role == "out")
        {
            if (other.CompareTag("Item") && other.transform.parent == transform.parent)
            {
                Item item = other.GetComponent<Item>();
                if (item != null)
                {
                    item.transform.parent = GameObject.Find("Items").transform;
                    Rigidbody2D rb = other.GetComponent<Rigidbody2D>();
                    rb.linearVelocity = Vector2.zero;
                    item.hasBeenProcessed = false;
                }
            }
        }
    }
}