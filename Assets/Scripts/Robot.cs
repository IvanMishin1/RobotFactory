using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using Lua;

public class Robot : MonoBehaviour
{
    List<RobotTask> actions = new List<RobotTask>();
    Vector2 movePosition = Vector2.zero;
    
    async void Start()
    {
        var state = LuaState.Create();
        
        ValueTask<int> MoveInDirectionAsync(Vector2 direction)
        {
            return MoveInDirectionCoroutine(direction);

            async ValueTask<int> MoveInDirectionCoroutine(Vector2 dir)
            {
                movePosition = (Vector2)transform.position + dir;
                while (Vector2.Distance(transform.position, movePosition) >= 0.01f)
                {
                    await Task.Yield();
                }
                transform.position = movePosition;
                return 0;
            }
        }
        
        state.Environment["up"] = new LuaFunction((context, buffer, ct) => MoveInDirectionAsync(Vector2.up));
        state.Environment["down"] = new LuaFunction((context, buffer, ct) => MoveInDirectionAsync(Vector2.down));
        state.Environment["left"] = new LuaFunction((context, buffer, ct) => MoveInDirectionAsync(Vector2.left));
        state.Environment["right"] = new LuaFunction((context, buffer, ct) => MoveInDirectionAsync(Vector2.right));
        var results = await state.DoStringAsync("up(); down(); left(); right(); up();up(); down(); left(); right(); up();up(); down(); left(); right(); up();up(); down(); left(); right(); up(); testtask();");
    }

    // Update is called once per frame
    async void Update()
    {
        transform.position = Vector2.MoveTowards(transform.position, movePosition, Time.deltaTime);
    }
}
