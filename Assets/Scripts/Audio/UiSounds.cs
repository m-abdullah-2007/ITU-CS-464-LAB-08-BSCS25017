using UnityEngine;

public enum UiSound { Hover, Click, Back, Open, Close, Pause, Resume, Success, Error }

public class UiSounds : MonoBehaviour
{
    public static UiSounds Instance { get; private set; }
    public static float LastPanelChange { get; private set; }   // lets keyboard "select" stay quiet when a panel opens

    [SerializeField] AudioSource[] sources;     // Ui0, Ui1, Output = UI group
    [SerializeField] AudioClip hover, click, back, open, close, pause, resume, success, error;
    [SerializeField] float pitchRange = 0.5f;   // +/- semitones: UI wants subtle variation
    int turn;
    float lastHover;

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(this); return; }
        Instance = this;
        foreach (var s in sources) s.ignoreListenerPause = true;   // menu clicks must work while paused
    }

    void OnEnable()  => UIManager.StateChanged += OnState;
    void OnDisable() => UIManager.StateChanged -= OnState;
    void OnDestroy() { if (Instance == this) Instance = null; }

    public static void Play(UiSound sound) { if (Instance != null) Instance.PlayClip(sound); }

    void PlayClip(UiSound sound)
    {
        AudioClip clip = sound switch
        {
            UiSound.Hover => hover,   UiSound.Click => click,   UiSound.Back => back,
            UiSound.Open => open,     UiSound.Close => close,   UiSound.Pause => pause,
            UiSound.Resume => resume, UiSound.Success => success, UiSound.Error => error,
            _ => null
        };
        if (clip == null) return;

        if (sound == UiSound.Hover)                                    // never machine-gun hovers
        {
            if (Time.unscaledTime - lastHover < 0.05f) return;
            lastHover = Time.unscaledTime;
        }

        AudioSource s = sources[turn];
        turn = (turn + 1) % sources.Length;
        s.pitch = Mathf.Pow(2f, Random.Range(-pitchRange, pitchRange) / 12f);
        s.PlayOneShot(clip);
    }

    // Panel sounds come from state changes, so no screen script has to remember them.
    void OnState(GameState prev, GameState next)
    {
        LastPanelChange = Time.unscaledTime;
        if (next == GameState.Paused && prev == GameState.Playing)      Play(UiSound.Pause);
        else if (next == GameState.Playing && prev == GameState.Paused) Play(UiSound.Resume);
        else if (next == GameState.Confirm)                             Play(UiSound.Open);    // dialog opens
        else if (prev == GameState.Confirm)                             Play(UiSound.Close);   // dialog closes
        else if (prev == GameState.Menu)                                Play(UiSound.Open);    // menu -> sub-screen
        else if (next == GameState.Menu)                                Play(UiSound.Close);   // sub-screen -> menu
    }
}
