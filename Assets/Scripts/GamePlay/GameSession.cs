using System;
using UnityEngine;

public class GameSession : MonoBehaviour
{
    public static GameSession Instance { get; private set; }

    // ---- C# events (publisher). Only this class can raise them. ----
    public static event Action GameStarted;
    public static event Action<int, int> Damaged;          // current health, max health
    public static event Action<int> CoinCollected;         // coins this run
    public static event Action<int, int, bool> GameLost;   // meters, coins, new best?

    // read by the Game Over screen (unchanged from Lab 07)
    public static int LastMeters, LastCoins;
    public static bool LastWasNewBest;

    public const int MaxHealth = 5;
    const float MetersPerSecond = 10f;
    int health, runCoins;
    float meters;

    public int Health => health;
    public int RunCoins => runCoins;
    public int Meters => (int)meters;

    void Awake() => Instance = this;

    void Update()
    {
        if (UIManager.Instance.State != GameState.Playing) return;
        meters += MetersPerSecond * Time.deltaTime;       // 0 while timeScale = 0
    }

    public void StartRun()
    {
        health = MaxHealth; runCoins = 0; meters = 0f;
        UIManager.Instance.SetState(GameState.Playing);
        GameStarted?.Invoke();
    }

    public void TakeHit()
    {
        if (UIManager.Instance.State != GameState.Playing) return;
        health = Mathf.Max(0, health - 1);
        Damaged?.Invoke(health, MaxHealth);               // the bar, sound, flash, shake, haptics all listen
        if (health == 0) EndRun();
    }

    public void AddCoin()
    {
        if (UIManager.Instance.State != GameState.Playing) return;
        runCoins++;
        CoinCollected?.Invoke(runCoins);
    }

    void EndRun()
    {
        int m = (int)meters;
        LastMeters = m;
        LastCoins = runCoins;
        LastWasNewBest = m > GameData.Best;
        if (LastWasNewBest) GameData.Best = m;

        GameData.Coins       += runCoins;
        GameData.TotalMeters += m;
        GameData.TotalCoins  += runCoins;
        GameData.Runs++;

        GameLost?.Invoke(m, runCoins, LastWasNewBest);    // raise first, while still Playing
        UIManager.Instance.SetState(GameState.Over);
    }
}
