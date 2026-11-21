using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public static class Ease
{
    public static float Linear(float t) => t;
    public static float InQuad(float t) => t * t;
    public static float OutQuad(float t) => 1f - (1f - t) * (1f - t);
    public static float OutCubic(float t) { float u = 1f - t; return 1f - u * u * u; }
    public static float OutBack(float t)
    {
        const float c1 = 1.70158f, c3 = c1 + 1f;
        float u = t - 1f;
        return 1f + c3 * u * u * u + c1 * u * u;      // goes slightly past 1, then settles at 1
    }
}

public static class Tween
{
    // runs apply(0..1 eased) over duration seconds; unscaled time = works while paused
    public static Coroutine Run(MonoBehaviour owner, float duration, Func<float, float> ease,
                                Action<float> apply, Action done = null)
        => owner.StartCoroutine(Routine(duration, ease, apply, done));

    public static IEnumerator Routine(float duration, Func<float, float> ease,
                                      Action<float> apply, Action done = null)
    {
        float t = 0f;
        while (t < duration)
        {
            t += Time.unscaledDeltaTime;
            apply(ease(Mathf.Clamp01(t / duration)));
            yield return null;
        }
        apply(ease(1f));
        done?.Invoke();
    }
}

public static class Fx
{
    // quick scale "pop" on a value change (grow fast, spring back)
    public static void Punch(MonoBehaviour owner, Transform t, float amount = 0.12f)
    {
        Tween.Run(owner, 0.08f, Ease.OutQuad,
            v => t.localScale = Vector3.one * (1f + amount * v),
            () => Tween.Run(owner, 0.25f, Ease.OutBack,
                v => t.localScale = Vector3.one * Mathf.LerpUnclamped(1f + amount, 1f, v)));
    }

    // number counts from -> to
    public static Coroutine Count(MonoBehaviour owner, TMP_Text label, int from, int to,
                                  string format, float duration = 0.5f, float delay = 0f)
        => owner.StartCoroutine(CountRoutine(label, from, to, format, duration, delay));

    static IEnumerator CountRoutine(TMP_Text label, int from, int to, string format, float duration, float delay)
    {
        if (delay > 0f) yield return new WaitForSecondsRealtime(delay);
        yield return Tween.Routine(duration, Ease.OutCubic,
            v => label.SetText(format, Mathf.RoundToInt(Mathf.Lerp(from, to, v))));
    }

    // Image (Filled) fillAmount moves to a target
    public static Coroutine Fill(MonoBehaviour owner, Image img, float to, float duration = 0.35f, float delay = 0f)
        => owner.StartCoroutine(FillRoutine(img, to, duration, delay));

    static IEnumerator FillRoutine(Image img, float to, float duration, float delay)
    {
        if (delay > 0f) yield return new WaitForSecondsRealtime(delay);
        float from = img.fillAmount;
        yield return Tween.Routine(duration, Ease.OutCubic, v => img.fillAmount = Mathf.Lerp(from, to, v));
    }
}