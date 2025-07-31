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
    
    async void Start()
    {
        state = LuaState.Create();
        
        state.Environment["up"] = new LuaFunction((context, buffer, ct) => MoveInDirectionAsync(Vector2.up));
        state.Environment["down"] = new LuaFunction((context, buffer, ct) => MoveInDirectionAsync(Vector2.down));
        state.Environment["left"] = new LuaFunction((context, buffer, ct) => MoveInDirectionAsync(Vector2.left));
        state.Environment["right"] = new LuaFunction((context, buffer, ct) => MoveInDirectionAsync(Vector2.right));
        
    }

    // Update is called once per frame
    async void Update()
    {
        if (busy)
            return;
        busy = true;
        await state.DoFileAsync(Application.dataPath + "/Saves/" + gameObject.name + ".lua");
        
        busy = false;
    }
    
    ValueTask<int> MoveInDirectionAsync(Vector2 direction)
    {
        return MoveInDirectionCoroutine(direction);
        async ValueTask<int> MoveInDirectionCoroutine(Vector2 dir)
        {
            movePosition = (Vector2)transform.position + dir;
            while (Vector2.Distance(transform.position, movePosition) >= 0.01f)
            {
                // Move towards the target position
                transform.position = Vector2.MoveTowards(transform.position, movePosition, Time.deltaTime * moveSpeed); // 5f is speed
                await Task.Yield();
            }
            transform.position = movePosition;
            return 0;
        }
    }
}
