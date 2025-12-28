using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class DicePreviewRotator : MonoBehaviour
{
    [Header("Pick")]
    [SerializeField] private Camera cam;
    [SerializeField] private LayerMask diceLayer;
    [SerializeField] private bool ignoreWhenPointerOverUI = true;

    [Header("Rotation")]
    [SerializeField] private float rotateSpeed = 0.2f;

    private Transform grabbed;
    private Vector2 lastPos;
    private bool dragging;

    private void Awake()
    {
        if (!cam) cam = Camera.main;
    }

    private void Update()
    {
        if (TryPointerDown(out var pDown))
        {
            if (ignoreWhenPointerOverUI && EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
                return;

            TryGrab(pDown);
        }

        if (TryPointerMove(out var pMove))
        {
            if (dragging && grabbed != null)
            {
                Vector2 delta = pMove - lastPos;

                // крутимо навколо Y і X (як “в руках”)
                float yaw = -delta.x * rotateSpeed;
                float pitch = delta.y * rotateSpeed;

                grabbed.Rotate(Vector3.up, yaw, Space.World);
                grabbed.Rotate(cam.transform.right, pitch, Space.World);

                lastPos = pMove;
            }
        }

        if (TryPointerUp())
        {
            dragging = false;
            grabbed = null;
        }
    }

    private void TryGrab(Vector2 screenPos)
    {
        if (!cam) cam = Camera.main;

        Ray r = cam.ScreenPointToRay(screenPos);
        if (Physics.Raycast(r, out var hit, 1000f, diceLayer))
        {
            grabbed = hit.collider.transform;
            dragging = true;
            lastPos = screenPos;
        }
    }

    private bool TryPointerDown(out Vector2 pos)
    {
        pos = default;

        if (Touchscreen.current != null)
        {
            var t = Touchscreen.current.primaryTouch;
            if (t.press.wasPressedThisFrame)
            {
                pos = t.position.ReadValue();
                return true;
            }
        }

        if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
        {
            pos = Mouse.current.position.ReadValue();
            return true;
        }

        return false;
    }

    private bool TryPointerMove(out Vector2 pos)
    {
        pos = default;

        if (Touchscreen.current != null)
        {
            var t = Touchscreen.current.primaryTouch;
            if (t.press.isPressed)
            {
                pos = t.position.ReadValue();
                return true;
            }
        }

        if (Mouse.current != null && Mouse.current.leftButton.isPressed)
        {
            pos = Mouse.current.position.ReadValue();
            return true;
        }

        return false;
    }

    private bool TryPointerUp()
    {
        if (Touchscreen.current != null)
        {
            var t = Touchscreen.current.primaryTouch;
            return t.press.wasReleasedThisFrame;
        }

        if (Mouse.current != null)
            return Mouse.current.leftButton.wasReleasedThisFrame;

        return false;
    }
}
