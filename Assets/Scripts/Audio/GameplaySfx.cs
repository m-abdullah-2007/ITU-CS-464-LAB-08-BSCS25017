using UnityEngine;

public class GameplaySfx : MonoBehaviour
{
    [SerializeField] AudioClip pickup, hurt, death, boost, runStart;
    float lastBoost;

    void OnEnable()
    {
        GameSession.GameStarted   += OnStarted;
        GameSession.CoinCollected += OnCoin;
        GameSession.Damaged       += OnDamaged;
        GameSession.GameLost      += OnLost;
        HoldZone.HoldChanged      += OnHold;
    }
    void OnDisable()
    {
        GameSession.GameStarted   -= OnStarted;
        GameSession.CoinCollected -= OnCoin;
        GameSession.Damaged       -= OnDamaged;
        GameSession.GameLost      -= OnLost;
        HoldZone.HoldChanged      -= OnHold;
    }

    void OnStarted() => Play(runStart);
    void OnCoin(int coins) => Play(pickup);
    void OnDamaged(int current, int max) { if (current > 0) Play(hurt); }       // the last hit plays "death" instead
    void OnLost(int meters, int coins, bool best) => Play(death);

    void OnHold(bool down)
    {
        if (!down || UIManager.Instance.State != GameState.Playing) return;
        if (Time.unscaledTime - lastBoost < 0.25f) return;                       // no spam from rapid taps
        lastBoost = Time.unscaledTime;
        Play(boost, 0.7f);
    }

    static void Play(AudioClip c, float v = 1f) { if (SfxPlayer.Instance != null) SfxPlayer.Instance.Play(c, v); }
}
