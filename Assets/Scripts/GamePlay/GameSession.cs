using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameSession : MonoBehaviour
{
    public static GameSession Instance { get; private set; }

    // read by the Game Over screen
    public static int LastMeters, LastCoins;
    public static bool LastWasNewBest;

    [SerializeField] TMP_Text distanceText, healthText, coinsText;
    [SerializeField] Image healthFill, lagFill;   // lagFill = pale bar behind
    [SerializeField] Transform healthGroup;
    [SerializeField] Gradient colour;

    const int MaxHealth = 5;
    const float MetersPerSecond = 10f;
    int health, runCoins;
    Coroutine lagRun;
    float meters;

    void Awake() => Instance = this;

    void Update()
    {
        if (UIManager.Instance.State != GameState.Playing) return;
        meters += MetersPerSecond * Time.deltaTime;      // Time.deltaTime is 0 while timeScale = 0
        distanceText.SetText("{0} m", (int)meters);
    }

    public void StartRun()
    {
        health = MaxHealth; runCoins = 0; meters = 0f;
        RefreshHud();
        UIManager.Instance.SetState(GameState.Playing);
    }

    public void TakeHit()
    {
        if (UIManager.Instance.State != GameState.Playing) return;
        health = Mathf.Max(0, health - 1);
        SettingsUI.Buzz();
        RefreshHud(true);                 // true = we lost health
        if (health == 0) EndRun();
    }

    public void AddCoin()
    {
        if (UIManager.Instance.State != GameState.Playing) return;
        runCoins++;
        RefreshHud();
        Fx.Punch(this, coinsText.transform, 0.15f);   // gold counter pops on pickup
    }

    void EndRun()
    {
        int m = (int)meters;
        LastMeters = m;
        LastCoins = runCoins;
        LastWasNewBest = m > GameData.Best;
        if (LastWasNewBest) GameData.Best = m;

        GameData.Coins += runCoins;
        GameData.TotalMeters += m;
        GameData.TotalCoins += runCoins;
        GameData.Runs++;

        UIManager.Instance.SetState(GameState.Over);
    }

    void RefreshHud(bool lost = false)
    {
        float fraction = Mathf.Clamp01((float)health / MaxHealth);   // (float) cast, as in the lecture
        healthFill.fillAmount = fraction;                  // the truth drops at once
        healthFill.color = colour.Evaluate(fraction);

        if (lost)
        {
            if (lagRun != null) StopCoroutine(lagRun);
            lagRun = Fx.Fill(this, lagFill, fraction, 0.35f, 0.4f);   // pale bar waits 0.4 s, then slides down
            Fx.Punch(this, healthGroup, 0.1f);
        }
        else lagFill.fillAmount = fraction;                // new run / no damage: snap
        healthText.SetText("HP {0}/{1}", health, MaxHealth);
        coinsText.SetText("COINS {0}", runCoins);
        distanceText.SetText("{0} m", (int)meters);
    }
}