using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class TabButtonView : MonoBehaviour
{
    [Header("Refs")]
    public Button button;
    public RectTransform visual;   // Внутрішній об’єкт, який масштабуємо
    public Image icon;
    public GameObject selectedMark; // optional: підкладка/світіння/крапка

    [Header("Anim")]
    public float selectedScale = 1.18f;
    public float unselectedScale = 1.0f;
    public float animTime = 0.12f;

    [Header("Colors")]
    public Color selectedColor = Color.white;
    public Color unselectedColor = new Color(1f, 1f, 1f, 0.55f);

    Coroutine _co;

    public void SetSelected(bool selected)
    {
        if (selectedMark) selectedMark.SetActive(selected);
        if (icon) icon.color = selected ? selectedColor : unselectedColor;

        float target = selected ? selectedScale : unselectedScale;
        if (_co != null) StopCoroutine(_co);
        _co = StartCoroutine(ScaleTo(target));
    }

    IEnumerator ScaleTo(float target)
    {
        if (!visual) yield break;

        float t = 0f;
        Vector3 start = visual.localScale;
        Vector3 end = Vector3.one * target;

        while (t < animTime)
        {
            t += Time.unscaledDeltaTime;
            float k = Mathf.Clamp01(t / animTime);
            // легкий ease-out
            k = 1f - Mathf.Pow(1f - k, 3f);
            visual.localScale = Vector3.LerpUnclamped(start, end, k);
            yield return null;
        }

        visual.localScale = end;
    }
}
