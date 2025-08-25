using UnityEngine;
using UnityEngine.InputSystem;

public class TimeManager : MonoBehaviour
{
    public int Day;
    public int Hour;
    public float decimalHour;

    [SerializeField] private float hoursPerRealSecond = 1f / 30f;

    private InputAction timeScaleAction;
    private int currentScale = 1;

    private void Awake()
    {
        timeScaleAction = InputSystem.actions.FindAction("TimeScale");
        if (timeScaleAction == null)
        {
            Debug.LogError("Missing action 'TimeScale'.");
            enabled = false;
            return;
        }
        timeScaleAction.performed += OnTimeScalePerformed;
    }
    
    private void Update()
    {
        decimalHour += hoursPerRealSecond * Time.deltaTime;
        if (decimalHour >= 24f)
        {
            decimalHour -= 24f;
            Day++;
        }
        Hour = (int)decimalHour;
    }

    private void OnTimeScalePerformed(InputAction.CallbackContext ctx)
    {
        int newScale = Mathf.Max(1, Mathf.RoundToInt(ctx.ReadValue<float>()));
        if (newScale == currentScale) return;
        Time.timeScale = newScale;
        currentScale = newScale;
    }

    public void EnableActions() => timeScaleAction?.Enable();
    public void DisableActions() => timeScaleAction?.Disable();
    private void OnEnable() => timeScaleAction?.Enable();
    private void OnDisable() => timeScaleAction?.Disable();
    private void OnDestroy() { if (timeScaleAction != null) timeScaleAction.performed -= OnTimeScalePerformed; }
}