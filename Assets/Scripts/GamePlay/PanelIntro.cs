using UnityEngine;

[RequireComponent(typeof(CanvasGroup))]
public class PanelIntro : MonoBehaviour
{
    public enum Mode { FadeSlide, FadeScale }

    [SerializeField] Mode mode = Mode.FadeSlide;
    [SerializeField] RectTransform target;     // what moves or scales (empty = this panel; or the Card)
    [SerializeField] float duration = 0.3f;

    CanvasGroup group;
    Vector2 home;

    void Awake()
    {
        group = GetComponent<CanvasGroup>();
        if (target == null) target = (RectTransform)transform;
        home = target.anchoredPosition;
    }

    void OnEnable()    // UIManager.SetActive(true) triggers this = the transition plays on every entry
    {
        group.alpha = 0f;
        Tween.Run(this, duration * 0.7f, Ease.OutQuad, v => group.alpha = v);

        if (mode == Mode.FadeSlide)
        {
            target.anchoredPosition = home + Vector2.down * 80f;
            Tween.Run(this, duration, Ease.OutCubic,
                v => target.anchoredPosition = Vector2.LerpUnclamped(home + Vector2.down * 80f, home, v));
        }
        else
        {
            target.localScale = Vector3.one * 0.9f;
            Tween.Run(this, duration, Ease.OutBack,
                v => target.localScale = Vector3.one * Mathf.LerpUnclamped(0.9f, 1f, v));
        }
    }

    void OnDisable()   // always leave things at rest
    {
        group.alpha = 1f;
        target.anchoredPosition = home;
        target.localScale = Vector3.one;
    }
}