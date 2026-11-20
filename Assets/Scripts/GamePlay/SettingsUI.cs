using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SettingsUI : MonoBehaviour
{
    [SerializeField] Slider[] sliders;       // Master, Music, SFX, UI  (same order as MixerVolumes.Params)
    [SerializeField] TMP_Text[] values;      // 4 percentage labels
    [SerializeField] Toggle muteToggle, hapticsToggle, leftHandToggle;
    [SerializeField] TMP_Text muteState, hapticsState, leftHandState;

    void OnEnable()
    {
        // set values first (without notifying), then listen, so loading does not trigger saves
        for (int i = 0; i < sliders.Length; i++)
        {
            int c = i;
            sliders[i].onValueChanged.RemoveAllListeners();
            sliders[i].minValue = MixerVolumes.MinLinear;
            sliders[i].maxValue = 1f;
            sliders[i].SetValueWithoutNotify(MixerVolumes.Get(i));
            RefreshValue(i);
            sliders[i].onValueChanged.AddListener(v => { MixerVolumes.Set(c, v); RefreshValue(c); });
        }

        muteToggle.onValueChanged.RemoveAllListeners();
        hapticsToggle.onValueChanged.RemoveAllListeners();
        leftHandToggle.onValueChanged.RemoveAllListeners();

        muteToggle.SetIsOnWithoutNotify(MixerVolumes.Muted);
        hapticsToggle.SetIsOnWithoutNotify(GameData.Haptics);
        leftHandToggle.SetIsOnWithoutNotify(GameData.LeftHand);
        RefreshStates();

        muteToggle.onValueChanged.AddListener(on => { MixerVolumes.SetMuted(on); RefreshStates(); Fx.Punch(this, muteState.transform, 0.3f); });
        hapticsToggle.onValueChanged.AddListener(on => { GameData.Haptics = on; RefreshStates(); Fx.Punch(this, hapticsState.transform, 0.3f); if (on) Buzz(); });
        leftHandToggle.onValueChanged.AddListener(on => { GameData.LeftHand = on; RefreshStates(); Fx.Punch(this, leftHandState.transform, 0.3f); });
    }

    void OnDisable() => PlayerPrefs.Save();

    void RefreshValue(int i) => values[i].SetText("{0}%", Mathf.RoundToInt(sliders[i].value * 100f));

    void RefreshStates()
    {
        SetState(muteState, MixerVolumes.Muted);
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
