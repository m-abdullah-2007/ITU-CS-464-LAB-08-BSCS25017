using UnityEngine;
using UnityEngine.UI;

public class HitFlash : MonoBehaviour
{
    [SerializeField] Image overlay;
    [SerializeField] float alpha = 0.35f, fadeSpeed = 2.5f;

    void OnEnable()  => GameSession.Damaged += Flash;
    void OnDisable() => GameSession.Damaged -= Flash;     // without this, a restart leaves a dead subscriber

    void Flash(int current, int max) => SetAlpha(alpha);
    void Update() => SetAlpha(Mathf.MoveTowards(overlay.color.a, 0f, fadeSpeed * Time.unscaledDeltaTime));

    void SetAlpha(float a) { Color c = overlay.color; c.a = a; overlay.color = c; }
}
