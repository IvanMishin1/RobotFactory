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
    public CameraSize cameraSize;
    public float moveSpeed = 5f;
    public float zoomSpeed = 200f;

    void Awake()
    {
        pixelPerfectCamera = Camera.main.GetComponent<PixelPerfectCamera>();
        input = new CameraInputActions();
        input.Enable();
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
            pixelPerfectCamera.transform.position += new Vector3(move.x, move.y, 0f) * moveSpeed * Time.unscaledDeltaTime;

        float zoomAxis = input.Player.Zoom.ReadValue<float>();
        if (Mathf.Abs(zoomAxis) > 0.0001f)
        {
            cameraSize.height -= (int)(zoomAxis * zoomSpeed * Time.unscaledDeltaTime);
        }

        cameraSize.height &= ~1;
        cameraSize.width = ((int)(cameraSize.height * (16f / 9f))) & ~1;

        pixelPerfectCamera.refResolutionX = cameraSize.width;
        pixelPerfectCamera.refResolutionY = cameraSize.height;
    }

    public void EnableMovement()
    {
        input.Enable();
    }

    public void DisableMovement()
    {
        input.Disable();
    }
}