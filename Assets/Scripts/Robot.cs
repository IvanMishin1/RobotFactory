using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using Lua;

public class Robot : MonoBehaviour
{
    public string Name => gameObject.name;
    
    Vector2 movePosition = Vector2.zero;
    float moveSpeed = 0.5f; 
    
    private LuaState state;
    
    public bool busy = false;
    private Animator animator;
    private int facingDirection = -1;
    private SpriteRenderer spriteRenderer;
    
    async void Start()
    {
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        state = LuaState.Create();
        
        state.Environment["up"] = new LuaFunction((context, buffer, ct) => MoveInDirectionAsync(Vector2.up));
        state.Environment["down"] = new LuaFunction((context, buffer, ct) => MoveInDirectionAsync(Vector2.down));
        state.Environment["left"] = new LuaFunction((context, buffer, ct) => MoveInDirectionAsync(Vector2.left));
        state.Environment["right"] = new LuaFunction((context, buffer, ct) => MoveInDirectionAsync(Vector2.right));
        
        string path = Application.dataPath + "/Scripts/" + gameObject.name + ".lua";
        if (!System.IO.File.Exists(path))
            System.IO.File.WriteAllText(path, string.Empty);
    }

    // Update is called once per frame
    async void Update()
    {
        if (busy)
            return;
        busy = true;
        try
        {
            await state.DoFileAsync(Application.dataPath + "/Scripts/" + gameObject.name + ".lua");
        }
        catch (LuaParseException)
        {
            busy = false;
            Debug.LogError("Lua script parsing error for " + gameObject.name);
            return;
        }
        catch (LuaRuntimeException)
        {
            busy = false;
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
                
            
            Debug.Log(dir.x + " " + dir.y);
            
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
}
