using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering.Universal;

public class CameraManager : MonoBehaviour
{
    [Serializable]
    public class CameraSize
    {
        public int width;
        public int height;
        public CameraSize(int width, int height)
        {
            this.width = width;
            this.height = height;
        }
    }

    private PixelPerfectCamera pixelPerfectCamera;

    private InputActionMap input;
    private InputAction moveAction;
    private InputAction zoomAction;

    public CameraSize cameraSize = new CameraSize(640, 360);
    public int minHeight = 120;
    public int maxHeight = 1440;
    public float moveSpeed = 5f;
    public float zoomSpeed = 200f; // pixels of height per second per axis unit
    public bool matchScreenAspectOnStart = true;

    private float heightAccum;
    private bool initialized;

    void Awake()
    {
        pixelPerfectCamera = Camera.main?.GetComponent<PixelPerfectCamera>();
        if (pixelPerfectCamera == null)
        {
            Debug.LogError("PixelPerfectCamera not found on main camera.");
            enabled = false;
            return;
        }

        // Derive starting size
        float screenAspect = (float)Screen.width / Screen.height;

        if (cameraSize.height <= 0)
        {
            // Prefer existing ref resolution if valid
            if (pixelPerfectCamera.refResolutionY > 0)
            {
                cameraSize.height = pixelPerfectCamera.refResolutionY;
                cameraSize.width = pixelPerfectCamera.refResolutionX;
            }
            else
            {
                cameraSize.height = Mathf.Clamp(360, minHeight, maxHeight);
                cameraSize.width = Mathf.RoundToInt(cameraSize.height * screenAspect);
            }
        }

        if (matchScreenAspectOnStart)
        {
            cameraSize.height = Mathf.Clamp(cameraSize.height, minHeight, maxHeight);
            cameraSize.width = Mathf.RoundToInt(cameraSize.height * screenAspect);
        }

        // Even dimensions (avoid subpixel artifacts)
        cameraSize.height &= ~1;
        cameraSize.width &= ~1;

        heightAccum = cameraSize.height; // Prevent first zoom jump
        ApplySize();

        initialized = true;

        input = InputSystem.actions.FindActionMap("Camera");
        if (input == null)
        {
            Debug.LogError("[CameraManager] ActionMap 'Camera' not found.");
            enabled = false;
            return;
        }
        moveAction = input.FindAction("Move");
        zoomAction = input.FindAction("Zoom");
        input.Enable();
    }

    void Update()
    {
        if (!initialized) return;

        Vector2 move = moveAction.ReadValue<Vector2>();
        if (move.sqrMagnitude > 0f)
        {
            pixelPerfectCamera.transform.position +=
                new Vector3(move.x, move.y, 0f) * moveSpeed * Time.unscaledDeltaTime;
        }

        float zoomAxis = zoomAction.ReadValue<float>();
        if (Mathf.Abs(zoomAxis) > 0.0001f)
        {
            heightAccum -= zoomAxis * zoomSpeed * Time.unscaledDeltaTime;
            heightAccum = Mathf.Clamp(heightAccum, minHeight, maxHeight);
            UpdateIntResolutionFromAccum();
        }
    }

    private void UpdateIntResolutionFromAccum()
    {
        int newHeight = (int)Mathf.Round(heightAccum) & ~1;
        if (newHeight == cameraSize.height) return;

        cameraSize.height = newHeight;
        float screenAspect = (float)Screen.width / Screen.height;
        cameraSize.width = ((int)Mathf.Round(cameraSize.height * screenAspect)) & ~1;
        ApplySize();
    }

    private void ApplySize()
    {
        pixelPerfectCamera.refResolutionX = cameraSize.width;
        pixelPerfectCamera.refResolutionY = cameraSize.height;
    }

    public void EnableMovement() => input?.Enable();
    public void DisableMovement() => input?.Disable();
    void OnEnable() => input?.Enable();
    void OnDisable() => input?.Disable();
}