
using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;
using System;

public enum GameState
{
    Menu,
    Loadout,
    HowTo,
    Settings,
    Shop,
    Missions,
    Playing,
    Paused,
    Over,
    Confirm,
    Loading
}

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    // Notifies other scripts whenever the game state changes.
    // Parameters: previous state, new state.
    public static event Action<GameState, GameState> StateChanged;

    [Header("Panels")]
    [SerializeField] GameObject menuPanel;
    [SerializeField] GameObject loadoutPanel;
    [SerializeField] GameObject howToPanel;
    [SerializeField] GameObject settingsPanel;
    [SerializeField] GameObject shopPanel;
    [SerializeField] GameObject missionsPanel;
    [SerializeField] GameObject hudPanel;
    [SerializeField] GameObject pausePanel;
    [SerializeField] GameObject overPanel;
    [SerializeField] GameObject confirmPanel;
    [SerializeField] GameObject loadingPanel;

    [Header("First selected button per screen (keyboard / gamepad)")]
    [SerializeField] GameObject menuFirst;
    [SerializeField] GameObject pauseFirst;
    [SerializeField] GameObject overFirst;
    [SerializeField] GameObject confirmFirst;

    [Header("Confirm dialog")]
    [SerializeField] TMP_Text confirmTitle;
    [SerializeField] TMP_Text confirmBody;

    public GameState State { get; private set; }

    GameState previous = GameState.Menu;

    enum Pending
    {
        None,
        QuitApp,
        LeaveRun
    }

    Pending pending;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        SetState(GameState.Menu);
    }

    void Update()
    {
        // Escape on PC = Back on Android.
        if (!Input.GetKeyDown(KeyCode.Escape))
            return;

        // Loading cannot be cancelled by the player.
        if (State == GameState.Loading)
            return;

        // Escape pauses gameplay.
        if (State == GameState.Playing)
        {
            SetState(GameState.Paused);
            return;
        }

        // Escape resumes gameplay.
        if (State == GameState.Paused)
        {
            SetState(GameState.Playing);
            return;
        }

        // Escape closes the confirmation dialog.
        if (State == GameState.Confirm)
        {
            UiSounds.Play(UiSound.Back);
            ConfirmNo();
            return;
        }

        // Escape returns other screens to the main menu.
        if (State != GameState.Menu && State != GameState.Over)
        {
            UiSounds.Play(UiSound.Back);
            SetState(GameState.Menu);
        }
    }

    public void SetState(GameState next)
    {
        GameState prev = State;

        // Remember the screen behind the confirmation dialog.
        if (next == GameState.Confirm && State != GameState.Confirm)
            previous = State;

        State = next;

        // Activate the correct panel.
        menuPanel.SetActive(next == GameState.Menu);
        loadoutPanel.SetActive(next == GameState.Loadout);
        howToPanel.SetActive(next == GameState.HowTo);
        settingsPanel.SetActive(next == GameState.Settings);
        shopPanel.SetActive(next == GameState.Shop);
        missionsPanel.SetActive(next == GameState.Missions);

        hudPanel.SetActive(
            next == GameState.Playing ||
            next == GameState.Paused
        );

        pausePanel.SetActive(next == GameState.Paused);
        overPanel.SetActive(next == GameState.Over);
        confirmPanel.SetActive(next == GameState.Confirm);
        loadingPanel.SetActive(next == GameState.Loading);

        // Time runs during gameplay and loading.
        // All other states stop gameplay time.
        Time.timeScale =
            (next == GameState.Playing || next == GameState.Loading)
            ? 1f
            : 0f;

        // Only the Paused state pauses audio.
        AudioListener.pause = next == GameState.Paused;

        // Select the appropriate button for keyboard/gamepad navigation.
        if (EventSystem.current != null)
        {
            EventSystem.current.SetSelectedGameObject(next switch
            {
                GameState.Menu => menuFirst,
                GameState.Paused => pauseFirst,
                GameState.Over => overFirst,
                GameState.Confirm => confirmFirst,
                _ => null
            });
        }

        // Notify subscribers AFTER the state and panels have been updated.
        StateChanged?.Invoke(prev, next);
    }

    // Inspector buttons can pass one integer:
    // 0 Menu
    // 1 Loadout
    // 2 HowTo
    // 3 Settings
    // 4 Shop
    // 5 Missions
    // 6 Playing
    // 7 Paused
    // 8 Over
    // 9 Confirm
    // 10 Loading

    public void Go(int state)
    {
        SetState((GameState)state);
    }

    // ---------------- CONFIRM DIALOG ----------------

    public void AskQuitApp()
    {
        pending = Pending.QuitApp;

        confirmTitle.SetText("QUIT GAME?");
        confirmBody.SetText("You will return to your device.");

        SetState(GameState.Confirm);
    }

    public void AskLeaveRun()
    {
        pending = Pending.LeaveRun;

        confirmTitle.SetText("LEAVE THIS RUN?");
        confirmBody.SetText(
            "Your distance and coins from this run are lost."
        );

        SetState(GameState.Confirm);
    }

    public void ConfirmYes()
    {
        if (pending == Pending.QuitApp)
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }
        else
        {
            SetState(GameState.Menu);
        }

        pending = Pending.None;
    }

    public void ConfirmNo()
    {
        SetState(previous);
    }
}
