using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HudListener : MonoBehaviour
{
    [SerializeField] TMP_Text distanceText, healthText, coinsText;
    [SerializeField] Image healthFill, lagFill;
    [SerializeField] Transform healthGroup;
    [SerializeField] Gradient colour;          // Coral 0-0.35, Cyan 0.36-1 (Lab 07)
    Coroutine lagRun;

    void OnEnable()
    {
        GameSession.GameStarted  += OnStarted;
        GameSession.Damaged      += OnDamaged;
        GameSession.CoinCollected += OnCoin;
        Refresh(false);                        // the panel is enabled just after a run starts: catch up
    }
    void OnDisable()
    {
        GameSession.GameStarted  -= OnStarted;
        GameSession.Damaged      -= OnDamaged;
        GameSession.CoinCollected -= OnCoin;
    }

    void OnStarted() => Refresh(false);
    void OnDamaged(int current, int max) => Refresh(true);
    void OnCoin(int runCoins) { Refresh(false); Fx.Punch(this, coinsText.transform, 0.15f); }

    void Update()   // distance changes every frame: poll it, do not raise an event 60 times a second
    {
        if (GameSession.Instance != null) distanceText.SetText("{0} m", GameSession.Instance.Meters);
    }

    void Refresh(bool lost)
    {
        GameSession s = GameSession.Instance;
        if (s == null) return;
        float fraction = Mathf.Clamp01((float)s.Health / GameSession.MaxHealth);   // (float) cast, as in the lecture
        healthFill.fillAmount = fraction;
        healthFill.color = colour.Evaluate(fraction);
        if (lost)
        {
            if (lagRun != null) StopCoroutine(lagRun);
            lagRun = Fx.Fill(this, lagFill, fraction, 0.35f, 0.4f);
            Fx.Punch(this, healthGroup, 0.1f);
        }
        else lagFill.fillAmount = fraction;
        healthText.SetText("HP {0}/{1}", s.Health, GameSession.MaxHealth);
        coinsText.SetText("COINS {0}", s.RunCoins);
    }
}
