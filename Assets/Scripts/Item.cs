using UnityEngine;

public class Item : MonoBehaviour
{
    public string itemType = "ore";
    public SpriteRenderer spriteRenderer;
    public string[] itemTypes = { "ore", "ingot", "wood" };
    public bool hasBeenProcessed = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        if (spriteRenderer == null)
            spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void SetItemType(string type)
    {
        itemType = type;
        gameObject.name = itemType;
        
        Sprite newSprite = Resources.Load<Sprite>($"Textures/{itemType}");
        if (newSprite != null)
        {
            spriteRenderer.sprite = newSprite;
        }
        else
        {
            Debug.LogWarning($"Could not load sprite for item type: {itemType}");
        }
    }
}
