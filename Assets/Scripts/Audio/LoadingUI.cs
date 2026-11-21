using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LoadingUI : MonoBehaviour
{
    [SerializeField] Image fill;
    [SerializeField] TMP_Text percent, tip;
    static readonly string[] Tips =
    {
        "Hold anywhere to fly. Let go to fall.",
        "Gold is coins. Coral is danger.",
        "Spend coins in the Shop on faster jetpacks."
    };

    void OnEnable()
    {
        SceneLoader.Progress += OnProgress;
        tip.SetText(Tips[Random.Range(0, Tips.Length)]);
        OnProgress(0f);
    }
    void OnDisable() => SceneLoader.Progress -= OnProgress;

    void OnProgress(float p)
    {
        fill.fillAmount = p;
        percent.SetText("{0}%", Mathf.RoundToInt(p * 100f));
    }
}
