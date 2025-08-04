using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using Lua;
using Commands;

public class Robot : MonoBehaviour
{
    public string Name => gameObject.name;
    
    Vector2 movePosition = Vector2.zero;
    public float moveSpeed = 0.5f; 
    
    private LuaState state;
    
    public bool busy = false;
    public bool stop = false;
    public bool pause = false;
    
    public Animator animator;
    public int facingDirection = -1;
    public SpriteRenderer spriteRenderer;
    
    public Item pickedUpItem = null;
    
    async void Start()
    {
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        state = LuaState.Create();
        
        Command.Register("up", state, this, new MoveCommands(Vector2.up));
        Command.Register("down", state, this, new MoveCommands(Vector2.down));
        Command.Register("left", state, this, new MoveCommands(Vector2.left));
        Command.Register("right", state, this, new MoveCommands(Vector2.right));
        Command.Register("pickup", state, this, new PickupCommand());
        Command.Register("drop", state, this, new DropCommand());
        Command.Register("wait", state, this, new WaitCommand());
        Command.Register("stop", state, this, new StopCommand());
        Command.Register("pause", state, this, new PauseCommand());
        
        string path = Application.dataPath + "/Scripts/" + gameObject.name + ".lua";
        if (!System.IO.File.Exists(path))
            System.IO.File.WriteAllText(path, string.Empty);
    }
    
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
}
