using UnityEngine;
using UnityEngine.Audio;

public static class MixerVolumes
{
    public const float MinLinear = 0.0001f;   // Log10(0) = -infinity, so sliders never reach 0
    public static readonly string[] Params = { "MasterVolume", "MusicVolume", "SfxVolume", "UiVolume" };
    static readonly float[] Defaults = { 1f, 0.7f, 1f, 0.8f };
    const string MuteKey = "audio_muted";

    static AudioMixer mixer;

    public static bool Muted => PlayerPrefs.GetInt(MuteKey, 0) == 1;

    public static float Get(int i) =>
        Mathf.Clamp(PlayerPrefs.GetFloat("vol_" + Params[i], Defaults[i]), MinLinear, 1f);

    public static void Init(AudioMixer m) { mixer = m; ApplyAll(); }

    public static void Set(int i, float linear)
    {
        PlayerPrefs.SetFloat("vol_" + Params[i], Mathf.Clamp(linear, MinLinear, 1f));
        Apply(i);
    }

    public static void SetMuted(bool on)
    {
        PlayerPrefs.SetInt(MuteKey, on ? 1 : 0);
        Apply(0);                                  // mute lives on Master only
    }

    public static void ApplyAll() { for (int i = 0; i < Params.Length; i++) Apply(i); }

    // 1.0 -> 0 dB, 0.5 -> -6 dB, 0.1 -> -20 dB, 0.0001 -> -80 dB
    public static float ToDecibels(float linear) =>
        Mathf.Log10(Mathf.Clamp(linear, MinLinear, 1f)) * 20f;

    public static bool Ready => mixer != null;

    static void Apply(int i)
    {
        if (mixer == null)
        {
            Debug.LogError("MixerVolumes: no mixer. Is AudioBootstrap in the scene with its Mixer slot filled?");
            return;
        }
        float db = (i == 0 && Muted) ? -80f : ToDecibels(Get(i));
        if (!mixer.SetFloat(Params[i], db))
            Debug.LogWarning("Mixer has no exposed parameter named " + Params[i]);
        Debug.Log(Params[i] + " -> " + db.ToString("F1") + " dB");   // delete once it works
    }
}