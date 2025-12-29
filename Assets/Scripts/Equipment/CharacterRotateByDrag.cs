using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class CharacterRotateByDrag : MonoBehaviour
{
    [Header("Pick")]
    [SerializeField] private Camera cam;
    [SerializeField] private LayerMask characterLayer;
    [SerializeField] private bool ignoreWhenPointerOverUI = true;

    [Header("Rotation")]
    [SerializeField] private float rotateSpeed = 0.25f;

    [Header("Debug")]
    [SerializeField] private bool log = true;
    [SerializeField] private float rayDistance = 2000f;

    private Transform grabbed;
    private Vector2 lastPos;
    private bool dragging;

    private void Awake()
    {
        if (!cam) cam = Camera.main;
        if (log) Debug.Log($"[CharacterRot] Awake. cam={(cam ? cam.name : "NULL")} mask={characterLayer.value}");
    }

    private void Update()
    {
        if (TryPointerDown(out var downPos, out var pointerId))
        {
            if (ignoreWhenPointerOverUI && IsPointerOverUI(pointerId))
            {
                if (log) Debug.Log("[CharacterRot] Pointer over UI -> ignored");
                return;
            }

            TryGrab(downPos);
        }

        if (TryPointerMove(out var movePos))
        {
            if (dragging && grabbed != null)
            {
                Vector2 delta = movePos - lastPos;
                float yaw = -delta.x * rotateSpeed;

                grabbed.Rotate(Vector3.up, yaw, Space.World);
                lastPos = movePos;
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
        if (!cam)
        {
            cam = Camera.main;
            if (log) Debug.Log($"[CharacterRot] cam was null, now={(cam ? cam.name : "NULL")}");
        }

        Ray ray = cam.ScreenPointToRay(screenPos);

        // Намалює промінь у Scene view
        Debug.DrawRay(ray.origin, ray.direction * 5f, Color.white, 0.2f);

        if (Physics.Raycast(ray, out var hit, rayDistance, characterLayer))
        {
            grabbed = hit.collider.transform; // тимчасово без marker
            dragging = true;
            lastPos = screenPos;

            if (log) Debug.Log($"[CharacterRot] HIT: {hit.collider.name} (layer {hit.collider.gameObject.layer}) -> grabbed={grabbed.name}");
        }
        else
        {
            if (log) Debug.Log("[CharacterRot] Raycast MISS (mask mismatch / no collider / wrong camera)");
        }
    }

    private bool IsPointerOverUI(int pointerId)
    {
        if (EventSystem.current == null) return false;
        return EventSystem.current.IsPointerOverGameObject(pointerId);
    }

    private bool TryPointerDown(out Vector2 pos, out int pointerId)
    {
        pos = default;
        pointerId = -1;

        if (Touchscreen.current != null)
        {
            var t = Touchscreen.current.primaryTouch;
            if (t.press.wasPressedThisFrame)
            {
                pos = t.position.ReadValue();
                pointerId = t.touchId.ReadValue(); // важливо для UI check на touch
                return true;
            }
        }

        if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
        {
            pos = Mouse.current.position.ReadValue();
            pointerId = -1; // миша
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
            return Touchscreen.current.primaryTouch.press.wasReleasedThisFrame;

        if (Mouse.current != null)
            return Mouse.current.leftButton.wasReleasedThisFrame;

        return false;
    }
}
