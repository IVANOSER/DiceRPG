using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class DiceRaycastSelector : MonoBehaviour
{
    [SerializeField] private Camera cam;
    [SerializeField] private LayerMask diceLayer = ~0;

    [Header("Optional: ignore clicks over UI")]
    [SerializeField] private bool ignoreWhenPointerOverUI = true;

    private void Awake()
    {
        if (!cam) cam = Camera.main;
    }

    private void Update()
    {
        // Один код і для мишки, і для тачу (Input System)
        if (TryGetPointerDown(out Vector2 screenPos))
        {
            if (ignoreWhenPointerOverUI && IsPointerOverUI())
                return;

            TryPick(screenPos);
        }
    }

    private bool TryGetPointerDown(out Vector2 screenPos)
    {
        screenPos = default;

        // Touch (mobile)
        if (Touchscreen.current != null)
        {
            var touch = Touchscreen.current.primaryTouch;
            if (touch.press.wasPressedThisFrame)
            {
                screenPos = touch.position.ReadValue();
                return true;
            }
        }

        // Mouse (desktop)
        if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame)
        {
            screenPos = Mouse.current.position.ReadValue();
            return true;
        }

        return false;
    }

    private bool IsPointerOverUI()
    {
        // працює якщо в сцені є EventSystem
        if (EventSystem.current == null) return false;
        return EventSystem.current.IsPointerOverGameObject();
    }

    private void TryPick(Vector2 screenPos)
    {
        if (!cam) cam = Camera.main;

        Ray ray = cam.ScreenPointToRay(screenPos);
        if (Physics.Raycast(ray, out RaycastHit hit, 1000f, diceLayer))
        {
            var selectable = hit.collider.GetComponentInParent<DieSelectable3D>();
            if (selectable != null)
                selectable.Select();
        }
    }
}
