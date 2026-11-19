using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MissionsUI : MonoBehaviour
{
    [SerializeField] TMP_Text[] titles, progressLabels, claimLabels;   // 3 each
    [SerializeField] Image[] fills, claimImages;                       // 3 each
    [SerializeField] Button[] claimButtons;                            // 3
    [SerializeField] TMP_Text coinLabel;

    const int Reward = 100;
    static readonly string[] Names = { "FLY 500 METERS", "COLLECT 50 COINS", "FINISH 3 RUNS" };
    static readonly int[] Goals = { 500, 50, 3 };
    static readonly string[] Units = { "m", "coins", "runs" };

    static readonly Color32 Cyan = new Color32(0x25, 0xD9, 0xD0, 255);
    static readonly Color32 Navy = new Color32(0x10, 0x1D, 0x32, 255);
    static readonly Color32 Muted = new Color32(0x9F, 0xB0, 0xC8, 255);

    int Value(int i) => i == 0 ? GameData.TotalMeters : i == 1 ? GameData.TotalCoins : GameData.Runs;

    void Awake()
    {
        for (int i = 0; i < claimButtons.Length; i++)
        {
            int c = i;
            claimButtons[i].onClick.AddListener(() => Claim(c));
            titles[i].SetText(Names[i]);
        }
    }

    void OnEnable() => Refresh(true);   // true = play the bar-fill tween

    void Claim(int i)
    {
        if (GameData.Claimed(i) || Value(i) < Goals[i]) return;
        int before = GameData.Coins;
        GameData.Claim(i);
        GameData.Coins += Reward;
        SettingsUI.Buzz();
        Refresh(false);
        Fx.Count(this, coinLabel, before, GameData.Coins, "{0}", 0.5f);
        Fx.Punch(this, claimButtons[i].transform, 0.15f);
    }

    void Refresh(bool animate)
    {
        coinLabel.SetText("{0}", GameData.Coins);
        for (int i = 0; i < claimButtons.Length; i++)
        {
            int value = Mathf.Min(Value(i), Goals[i]);
            float target = Mathf.Clamp01((float)value / Goals[i]);   // cast to float, or ints give 0 (lecture bug)
            if (animate) { fills[i].fillAmount = 0f; Fx.Fill(this, fills[i], target, 0.6f, 0.2f + 0.12f * i); }
            else fills[i].fillAmount = target;
            progressLabels[i].SetText(value + " / " + Goals[i] + " " + Units[i]);

            if (GameData.Claimed(i))
            {
                claimLabels[i].SetText("DONE"); claimLabels[i].color = Muted;
                claimImages[i].color = new Color32(0x10, 0x1D, 0x32, 255);
                claimButtons[i].interactable = false;
            }
            else if (value >= Goals[i])
            {
                claimLabels[i].SetText("CLAIM"); claimLabels[i].color = Navy;
                claimImages[i].color = Cyan;
                claimButtons[i].interactable = true;
            }
            else
            {
                claimLabels[i].SetText("IN PROGRESS"); claimLabels[i].color = Muted;
                claimImages[i].color = new Color32(0x10, 0x1D, 0x32, 255);
                claimButtons[i].interactable = false;
            }
        }
    }
}