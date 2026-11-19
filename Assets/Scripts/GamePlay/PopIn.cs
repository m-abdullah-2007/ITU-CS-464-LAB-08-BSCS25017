using System.Collections;
using UnityEngine;

public class PopIn : MonoBehaviour
{
    [SerializeField] float startScale = 0.85f;
    [SerializeField] float delay = 0f;
    [SerializeField] float duration = 0.4f;
    CanvasGroup group;      // optional: if the object has a Canvas Group it fades in too

    void OnEnable()
    {
        if (group == null) group = GetComponent<CanvasGroup>();
        transform.localScale = Vector3.one * startScale;
        if (group != null) group.alpha = 0f;
        StartCoroutine(Play());
    }

    IEnumerator Play()
    {
        if (delay > 0f) yield return new WaitForSecondsRealtime(delay);
        Tween.Run(this, duration, Ease.OutBack,
            v => transform.localScale = Vector3.one * Mathf.LerpUnclamped(startScale, 1f, v));
        if (group != null) Tween.Run(this, duration * 0.6f, Ease.OutQuad, v => group.alpha = v);
    }

    void OnDisable()
    {
        transform.localScale = Vector3.one;
        if (group != null) group.alpha = 1f;
    }
}