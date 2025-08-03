using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using Lua;

public class Robot : MonoBehaviour
{
    public string Name => gameObject.name;
    
    Vector2 movePosition = Vector2.zero;
    public float moveSpeed = 0.5f; 
    
    private LuaState state;
    
    public bool busy = false;
    public bool stop = false;
    
    private Animator animator;
    private int facingDirection = -1;
    private SpriteRenderer spriteRenderer;
    
    public Item pickedUpItem = null;
    
    async void Start()
    {
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        state = LuaState.Create();
        
        state.Environment["up"] = new LuaFunction((context, buffer, ct) => MoveInDirectionAsync(Vector2.up));
        state.Environment["down"] = new LuaFunction((context, buffer, ct) => MoveInDirectionAsync(Vector2.down));
        state.Environment["left"] = new LuaFunction((context, buffer, ct) => MoveInDirectionAsync(Vector2.left));
        state.Environment["right"] = new LuaFunction((context, buffer, ct) => MoveInDirectionAsync(Vector2.right));
        state.Environment["pickup"] = new LuaFunction((context, buffer, ct) => Pickup(context));
        state.Environment["drop"] = new LuaFunction((context, buffer, ct) => Drop());
        state.Environment["wait"] = new LuaFunction((context, buffer, ct) => Wait(context));
        state.Environment["stop"] = new LuaFunction((context, buffer, ct) => Stop());
        
        string path = Application.dataPath + "/Scripts/" + gameObject.name + ".lua";
        if (!System.IO.File.Exists(path))
            System.IO.File.WriteAllText(path, string.Empty);
    }

    // Update is called once per frame
    async void Update()
    {
        if (busy || stop)
            return;
        busy = true;
        try
        {
            await state.DoFileAsync(Application.dataPath + "/Scripts/" + gameObject.name + ".lua");
        }
        catch (LuaParseException)
        {
            busy = false;
            stop = true;
            Debug.LogError("Lua script parsing error for " + gameObject.name);
            return;
        }
        catch (LuaRuntimeException)
        {
            busy = false;
            stop = true;
            Debug.LogError("Lua script parsing error for " + gameObject.name);
            return;
        }
        
        busy = false;
    }
    
    ValueTask<int> MoveInDirectionAsync(Vector2 direction)
    {
        return MoveInDirectionCoroutine(direction);
        async ValueTask<int> MoveInDirectionCoroutine(Vector2 dir)
        {
            movePosition = (Vector2)transform.position + dir;
            Collider2D collision = Physics2D.OverlapCircle(movePosition, 0.4f, LayerMask.GetMask("Walls", "Machines"));
            if (collision != null)
            {
                
                return 0;
            }
            animator.SetInteger("x_movement", (int)dir.x);
            animator.SetInteger("y_movement", (int)dir.y);
            
            if (dir.y == 1f && facingDirection == -1)
                facingDirection = 1;
            else if (dir.y == -1f && facingDirection == 1)
                facingDirection = -1;

            if (dir.x == 1f && facingDirection == -1)
                spriteRenderer.flipX = true;
            else if (dir.x == -1f && facingDirection == -1)
                spriteRenderer.flipX = false;
            else if (dir.x == -1f && facingDirection == 1)
                spriteRenderer.flipX = true;
            else if (dir.x == 1f && facingDirection == 1)
                spriteRenderer.flipX = false;
            
            if (dir.y == 1f)
                spriteRenderer.sortingOrder = 3;
            else if (dir.y == -1f)
                spriteRenderer.sortingOrder = 1;
            
            while (Vector2.Distance(transform.position, movePosition) >= 0.01f)
            {
                // Move towards the target position
                transform.position = Vector2.MoveTowards(transform.position, movePosition, Time.deltaTime * moveSpeed); // 5f is speed
                
                await Task.Yield();
            }
            transform.position = movePosition;
            animator.SetInteger("x_movement", 0);
            animator.SetInteger("y_movement", 0);
            return 0;
        }
    }

    private ValueTask<int> Pickup(Lua.LuaFunctionExecutionContext context)
    {
        string itemType = null;
        if (pickedUpItem != null)
            return new ValueTask<int>(0);
        if (context.ArgumentCount > 0)
            itemType = context.GetArgument<string>(0);
        if (Physics2D.OverlapCircle(transform.position, 0.5f, LayerMask.GetMask("Items")) is Collider2D itemCollider)
        {
            Item item = itemCollider.GetComponent<Item>();
            if (!String.IsNullOrEmpty(itemType) && item.itemType != itemType && !item.transform.parent.CompareTag("Machine"))
            {
                Debug.Log($"Skipping item: expected type '{itemType}' but found '{item.itemType}'");
                return new ValueTask<int>(0);
            }
            if (item != null)
            {
                pickedUpItem = item;
                pickedUpItem.transform.SetParent(transform);
                pickedUpItem.transform.position = transform.position;
            }
        }
        return new ValueTask<int>(0);
    }

    private ValueTask<int> Drop()
    {
        if (pickedUpItem != null)
        {
            Debug.Log("Dropping " + pickedUpItem.name);
            pickedUpItem.transform.SetParent(GameObject.Find("Items").transform);
            pickedUpItem = null;
        }
        return new ValueTask<int>(0);
    }

    private ValueTask<int> Wait(LuaFunctionExecutionContext context)
    {
        return WaitCoroutine(context);
    
        async ValueTask<int> WaitCoroutine(LuaFunctionExecutionContext ctx)
        {
            float seconds = 1f;
        
            if (ctx.ArgumentCount > 0)
                seconds = (float)ctx.GetArgument<double>(0);
            
        
            await Task.Delay((int)(seconds * 1000));
        
            return 0;
        }
    }

    private ValueTask<int> Stop()
    {
        busy = false;
        stop = true;
        return new ValueTask<int>(0);
    }
}
