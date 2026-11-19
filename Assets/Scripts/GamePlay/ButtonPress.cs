using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ButtonPress : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler
{
    Selectable sel;
    Coroutine run;

    void Awake() => sel = GetComponent<Selectable>();

    public void OnPointerDown(PointerEventData e) { if (sel.interactable) To(0.94f, 0.08f, Ease.InQuad); }
    public void OnPointerUp(PointerEventData e) => Release();
    public void OnPointerExit(PointerEventData e) => Release();

    void Release() { if (transform.localScale.x < 0.999f) To(1f, 0.2f, Ease.OutBack); }

    void To(float scale, float duration, Func<float, float> ease)
    {
        if (run != null) StopCoroutine(run);
        float from = transform.localScale.x;
        run = Tween.Run(this, duration, ease,
            v => transform.localScale = Vector3.one * Mathf.LerpUnclamped(from, scale, v));
    }

    void OnDisable() { transform.localScale = Vector3.one; run = null; }
}