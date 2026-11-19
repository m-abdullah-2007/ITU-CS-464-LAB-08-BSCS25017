using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SettingsUI : MonoBehaviour
{
    [SerializeField] Slider volumeSlider;
    [SerializeField] TMP_Text volumeValue;
    [SerializeField] Toggle hapticsToggle, leftHandToggle;
    [SerializeField] TMP_Text hapticsState, leftHandState;

    void OnEnable()
    {
        // set values first, then listen, so loading does not trigger saves
        volumeSlider.onValueChanged.RemoveAllListeners();
        hapticsToggle.onValueChanged.RemoveAllListeners();
        leftHandToggle.onValueChanged.RemoveAllListeners();

        volumeSlider.SetValueWithoutNotify(GameData.Volume);
        hapticsToggle.SetIsOnWithoutNotify(GameData.Haptics);
        leftHandToggle.SetIsOnWithoutNotify(GameData.LeftHand);
        Refresh();

        volumeSlider.onValueChanged.AddListener(v => { GameData.Volume = v; Refresh(); });
        hapticsToggle.onValueChanged.AddListener(on => { GameData.Haptics = on; Refresh(); Fx.Punch(this, hapticsState.transform, 0.3f); if (on) Buzz(); });
        leftHandToggle.onValueChanged.AddListener(on => { GameData.LeftHand = on; Refresh(); Fx.Punch(this, leftHandState.transform, 0.3f); });
    }

    void Refresh()
    {
        AudioListener.volume = GameData.Volume;
        volumeValue.SetText("{0}%", Mathf.RoundToInt(GameData.Volume * 100f));
        SetState(hapticsState, GameData.Haptics);
        SetState(leftHandState, GameData.LeftHand);
    }

    static void SetState(TMP_Text t, bool on)
    {
        t.SetText(on ? "ON" : "OFF");
        t.color = on ? new Color32(0x25, 0xD9, 0xD0, 255) : new Color32(0x9F, 0xB0, 0xC8, 255);
    }

    public static void Buzz()
    {
#if UNITY_ANDROID || UNITY_IOS
        if (GameData.Haptics) Handheld.Vibrate();
#endif
    }
}