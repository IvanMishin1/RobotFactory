using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using Lua;
using Commands;

public class Robot : MonoBehaviour
{
    Vector2 movePosition = Vector2.zero;
    public float moveSpeed = 0.5f; 
    
    private LuaState state;
    
    private bool _stop;
    private bool _pause;
    
    public bool busy = false;
    public bool Stop { get { return _stop; } set { if (_stop != value) { DisplayToggleStatus("stopped"); } _stop = value; } } 
    public bool Pause { get { return _pause; } set { if (_pause != value) { DisplayToggleStatus("paused"); }_pause = value; } }
    
    private Coroutine statusResetCoroutine;

    public Animator animator;
    public int facingDirection = -1;
    public SpriteRenderer spriteRenderer;
    public SpriteRenderer statusSpriteRenderer;
	public Vector2 startingPosition;
    
    public Item pickedUpItem = null;
    
    void Start()
    {
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        statusSpriteRenderer = transform.Find("Status").GetComponent<SpriteRenderer>();

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
        if (busy || Stop)
            return;
        busy = true;
        try
        {
			startingPosition = transform.position;
            await state.DoFileAsync(Application.dataPath + "/Scripts/" + gameObject.name + ".lua");
        }
        catch (LuaParseException)
        {
            busy = false;
            Stop = true;
            Debug.LogError("Lua script parsing error for " + gameObject.name);
            DisplayToggleStatus("lua_error");
            return;
        }
        catch (LuaRuntimeException)
        {
            busy = false;
            Stop = true;
            Debug.LogError("Lua script parsing error for " + gameObject.name);
            DisplayToggleStatus("lua_error");
            return;
        }
        busy = false;
    }

    public void DisplayToggleStatus(string status)
    {
        statusSpriteRenderer.sprite = null;
        if (statusResetCoroutine != null)
            StopCoroutine(statusResetCoroutine);
        if (statusSpriteRenderer.sprite != null && statusSpriteRenderer.sprite.name == status)
            statusSpriteRenderer.sprite = null;
        if (status != null)
            statusSpriteRenderer.sprite = Resources.Load<Sprite>("Textures/Status/" + status);
        else 
            statusSpriteRenderer.sprite = null;

        statusResetCoroutine = StartCoroutine(ResetStatusAfterDelay());
    }
    private IEnumerator ResetStatusAfterDelay()
    {
        yield return new WaitForSeconds(1f);
        statusSpriteRenderer.sprite = null;
        if (Pause)
            DisplayToggleStatus("paused");
        else if (Stop)
            DisplayToggleStatus("stopped");
    }
}
