using UnityEngine;
using TMPro;

public class HudLayout : MonoBehaviour
{
    [SerializeField] RectTransform[] mirrored;   // HealthGroup, CoinsText, PauseButton
    [SerializeField] TMP_Text coinsText;
    bool flipped;

    void OnEnable()
    {
        bool want = GameData.LeftHand;
        if (want != flipped) { Flip(); flipped = want; }
    }

    void Flip()
    {
        foreach (var r in mirrored)
        {
            r.anchorMin = new Vector2(1f - r.anchorMin.x, r.anchorMin.y);
            r.anchorMax = new Vector2(1f - r.anchorMax.x, r.anchorMax.y);
            r.pivot = new Vector2(1f - r.pivot.x, r.pivot.y);
            r.anchoredPosition = new Vector2(-r.anchoredPosition.x, r.anchoredPosition.y);
        }
        coinsText.alignment = coinsText.alignment == TextAlignmentOptions.TopLeft
            ? TextAlignmentOptions.TopRight : TextAlignmentOptions.TopLeft;
    }
}