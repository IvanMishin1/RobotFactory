using System;
using UnityEngine;
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
    [NonSerialized] public CameraInputActions input;

    public CameraSize cameraSize = new CameraSize(640, 360);
    public int minHeight = 120;
    public int maxHeight = 1440;
    public float moveSpeed = 5f;
    public float zoomSpeed = 200f; // units (pixels of height) per second per axis unit

    // Float accumulator to avoid truncation jitter
    private float heightAccum;

    void Awake()
    {
        pixelPerfectCamera = Camera.main.GetComponent<PixelPerfectCamera>();
        input = new CameraInputActions();
        input.Enable();
        if (cameraSize == null) cameraSize = new CameraSize(640, 360);
        heightAccum = cameraSize.height;
        ApplySize();
    }

    void OnEnable()
    {
        input.Enable();
    }

    void OnDisable()
    {
        input.Disable();
    }

    void Update()
    {
        Vector2 move = input.Player.Move.ReadValue<Vector2>();
        if (move.sqrMagnitude > 0f)
        {
            pixelPerfectCamera.transform.position +=
                new Vector3(move.x, move.y, 0f) * moveSpeed * Time.unscaledDeltaTime;
        }

        float zoomAxis = input.Player.Zoom.ReadValue<float>();
        if (Mathf.Abs(zoomAxis) > 0.0001f)
        {
            heightAccum -= zoomAxis * zoomSpeed * Time.unscaledDeltaTime;
            heightAccum = Mathf.Clamp(heightAccum, minHeight, maxHeight);
            UpdateIntResolutionFromAccum();
        }
    }

    private void UpdateIntResolutionFromAccum()
    {
        cameraSize.height = (int)Mathf.Round(heightAccum) & ~1;
        cameraSize.width = ((int)Mathf.Round(cameraSize.height * (16f / 9f))) & ~1;
        ApplySize();
    }

    private void ApplySize()
    {
        pixelPerfectCamera.refResolutionX = cameraSize.width;
        pixelPerfectCamera.refResolutionY = cameraSize.height;
    }

    public void EnableMovement() => input.Enable();
    public void DisableMovement() => input.Disable();
}