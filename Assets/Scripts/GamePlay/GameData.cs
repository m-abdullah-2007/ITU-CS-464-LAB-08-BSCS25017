using UnityEngine;

public static class GameData
{
    public static int Coins { get => PlayerPrefs.GetInt("coins", 250); set => PlayerPrefs.SetInt("coins", value); }
    public static int Best { get => PlayerPrefs.GetInt("best", 0); set => PlayerPrefs.SetInt("best", value); }
    public static int Equipped { get => PlayerPrefs.GetInt("equipped", 0); set => PlayerPrefs.SetInt("equipped", value); }
    public static int TotalMeters { get => PlayerPrefs.GetInt("tMeters", 0); set => PlayerPrefs.SetInt("tMeters", value); }
    public static int TotalCoins { get => PlayerPrefs.GetInt("tCoins", 0); set => PlayerPrefs.SetInt("tCoins", value); }
    public static int Runs { get => PlayerPrefs.GetInt("runs", 0); set => PlayerPrefs.SetInt("runs", value); }
    public static float Volume { get => PlayerPrefs.GetFloat("vol", 0.8f); set => PlayerPrefs.SetFloat("vol", value); }
    public static bool Haptics { get => PlayerPrefs.GetInt("haptics", 1) == 1; set => PlayerPrefs.SetInt("haptics", value ? 1 : 0); }
    public static bool LeftHand { get => PlayerPrefs.GetInt("left", 0) == 1; set => PlayerPrefs.SetInt("left", value ? 1 : 0); }

    public static bool Owns(int jetpack) => jetpack == 0 || PlayerPrefs.GetInt("own" + jetpack, 0) == 1;
    public static void Own(int jetpack) => PlayerPrefs.SetInt("own" + jetpack, 1);

    public static bool Claimed(int mission) => PlayerPrefs.GetInt("claimed" + mission, 0) == 1;
    public static void Claim(int mission) => PlayerPrefs.SetInt("claimed" + mission, 1);
}