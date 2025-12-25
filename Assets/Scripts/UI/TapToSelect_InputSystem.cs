using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class TapToSelect_InputSystem : MonoBehaviour
{
    [SerializeField] private Camera cam;
    [SerializeField] private LayerMask enemyLayer = ~0;

    private void Awake()
    {
        if (cam == null) cam = Camera.main;
    }

    private void Update()
    {
        // 1) якщо тапа/кл≥ка не було Ч вих≥д
        if (Pointer.current == null) return;

        // Ћ ћ або тап по екрану
        if (!Pointer.current.press.wasPressedThisFrame) return;

        // 2) якщо натиснули по UI Ч ≥гноруЇмо (щоб кнопки працювали)
        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
            return;

        // 3) ¬з€ти позиц≥ю pointer (миш/тап)
        Vector2 screenPos = Pointer.current.position.ReadValue();

        // 4) Raycast у св≥т
        Ray ray = cam.ScreenPointToRay(screenPos);
        if (Physics.Raycast(ray, out RaycastHit hit, 1000f, enemyLayer))
        {
            var enemy = hit.collider.GetComponentInParent<Enemy>();
            if (enemy != null)
            {
                Debug.Log("RAY HIT ENEMY: " + enemy.name);
                BattleManager.Instance.SelectEnemy(enemy);
            }
        }
    }
}
