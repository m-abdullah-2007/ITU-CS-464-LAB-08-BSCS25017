using System.Collections;
using UnityEngine;
using UnityEngine.Audio;

public class AudioBootstrap : MonoBehaviour
{
    [SerializeField] AudioMixer mixer;
    [SerializeField] AudioMixerSnapshot normal, paused;
    [SerializeField] float blend = 0.2f;
    bool ducked;

    void OnEnable() => UIManager.StateChanged += OnState;
    void OnDisable() => UIManager.StateChanged -= OnState;

    IEnumerator Start()
    {
        MixerVolumes.Init(mixer);     // saved volumes apply before Settings is ever opened
        yield return null;
        MixerVolumes.ApplyAll();      // one frame later, in case the first call was too early
    }

    void OnState(GameState prev, GameState next)
    {
        // Confirm opened from Pause keeps the paused sound
        bool want = next == GameState.Paused || (next == GameState.Confirm && prev == GameState.Paused);
        if (want == ducked) return;
        ducked = want;
        (want ? paused : normal).TransitionTo(blend);
    }

    void OnApplicationPause(bool p) { if (p) PlayerPrefs.Save(); }   // mobile: app may be killed
    void OnApplicationQuit() => PlayerPrefs.Save();
}