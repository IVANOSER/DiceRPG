using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class CharacterRotateByDrag: MonoBehaviour
{
    [Header("Target")]
    public Transform characterRoot;

    [Header("Settings")]
    [Tooltip("How fast to rotate per pixel drag.")]
    public float rotationSpeed = 0.25f;

    [Tooltip("Ignore drag when pointer is over UI.")]
    public bool blockWhenOverUI = true;

    private bool isDragging;
    private Vector2 lastPos;

    void Update()
    {
        // Works for both mouse and touch in the new Input System
        if (Pointer.current == null) return;

        Vector2 pos = Pointer.current.position.ReadValue();

        // Pointer press/release
        bool pressedThisFrame = Pointer.current.press.wasPressedThisFrame;
        bool releasedThisFrame = Pointer.current.press.wasReleasedThisFrame;
        bool isPressed = Pointer.current.press.isPressed;

        if (pressedThisFrame)
        {
            if (blockWhenOverUI && IsPointerOverUI())
            {
                isDragging = false;
                return;
            }

            isDragging = true;
            lastPos = pos;
        }
        else if (releasedThisFrame)
        {
            isDragging = false;
        }

        if (!isDragging || !isPressed) return;

        float deltaX = pos.x - lastPos.x;
        Rotate(deltaX);
        lastPos = pos;
    }

    private void Rotate(float deltaX)
    {
        if (characterRoot == null) return;
        characterRoot.Rotate(Vector3.up, -deltaX * rotationSpeed, Space.World);
    }

    private bool IsPointerOverUI()
    {
        // Works in most UI setups; requires EventSystem in scene
        if (EventSystem.current == null) return false;
        return EventSystem.current.IsPointerOverGameObject();
    }
}
