using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShopUI : MonoBehaviour
{
    [SerializeField] TMP_Text[] nameLabels, statLabels, priceLabels, buyLabels;   // 3 each
    [SerializeField] Image[] buyImages;                                           // 3
    [SerializeField] Button[] buyButtons;                                         // 3
    [SerializeField] TMP_Text coinLabel, message;

    static readonly Color32 Cyan = new Color32(0x25, 0xD9, 0xD0, 255);
    static readonly Color32 Coral = new Color32(0xFF, 0x87, 0x5C, 255);
    static readonly Color32 Panel = new Color32(0x10, 0x1D, 0x32, 255);   // deep navy inside the card
    static readonly Color32 Navy = new Color32(0x10, 0x1D, 0x32, 255);
    static readonly Color32 White = new Color32(0xF3, 0xF5, 0xF7, 255);

    void Awake()
    {
        for (int i = 0; i < buyButtons.Length; i++)
        {
            int c = i;                                         // capture for the lambda
            buyButtons[i].onClick.AddListener(() => Buy(c));   // wired in code only, not also in the Inspector
            nameLabels[i].SetText(JetpackInfo.Names[i]);
            statLabels[i].SetText(JetpackInfo.Stats[i]);
        }
    }

    void OnEnable() { message.SetText(""); Refresh(); }

    void Buy(int i)
    {
        if (GameData.Owns(i) || GameData.Coins < JetpackInfo.Prices[i]) return;
        int before = GameData.Coins;
        GameData.Coins -= JetpackInfo.Prices[i];
        GameData.Own(i);
        UiSounds.Play(UiSound.Success);
        GameData.Equipped = i;
        message.SetText(JetpackInfo.Names[i] + " UNLOCKED AND EQUIPPED");
        SettingsUI.Buzz();
        Refresh();
        Fx.Count(this, coinLabel, before, GameData.Coins, "{0}", 0.4f);   // coins count down
        Fx.Punch(this, buyButtons[i].transform, 0.15f);                   // the bought card reacts
        Fx.Punch(this, message.transform, 0.2f);                          // confirmation pops
    }

    void Refresh()
    {
        coinLabel.SetText("{0}", GameData.Coins);
        for (int i = 0; i < buyButtons.Length; i++)
        {
            int price = JetpackInfo.Prices[i];
            if (GameData.Owns(i))
            {
                priceLabels[i].SetText("");
                buyLabels[i].SetText("OWNED");
                buyLabels[i].color = Cyan;
                buyImages[i].color = Panel;
                buyButtons[i].interactable = false;
            }
            else if (GameData.Coins >= price)
            {
                priceLabels[i].SetText("{0} COINS", price);
                buyLabels[i].SetText("BUY");
                buyLabels[i].color = Navy;
                buyImages[i].color = Cyan;
                buyButtons[i].interactable = true;
            }
            else
            {
                priceLabels[i].SetText("{0} COINS", price);
                buyLabels[i].SetText("NEED {0}", price - GameData.Coins);
                buyLabels[i].color = Navy;
                buyImages[i].color = Coral;
                buyButtons[i].interactable = false;
            }
        }
    }
}