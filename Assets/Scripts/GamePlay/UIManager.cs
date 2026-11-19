using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

public enum GameState { Menu, Loadout, HowTo, Settings, Shop, Missions, Playing, Paused, Over, Confirm }

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [Header("Panels")]
    [SerializeField]
    GameObject menuPanel, loadoutPanel, howToPanel, settingsPanel, shopPanel,
                                missionsPanel, hudPanel, pausePanel, overPanel, confirmPanel;

    [Header("First selected button per screen (keyboard / gamepad)")]
    [SerializeField] GameObject menuFirst, pauseFirst, overFirst, confirmFirst;

    [Header("Confirm dialog")]
    [SerializeField] TMP_Text confirmTitle, confirmBody;

    public GameState State { get; private set; }
    GameState previous = GameState.Menu;
    enum Pending { None, QuitApp, LeaveRun }
    Pending pending;

    void Awake() => Instance = this;
    void Start() => SetState(GameState.Menu);

    void Update()
    {
        // Esc on PC = Back on Android (lecture: pause on Escape)
        if (!Input.GetKeyDown(KeyCode.Escape)) return;
        if (State == GameState.Playing) SetState(GameState.Paused);
        else if (State == GameState.Paused) SetState(GameState.Playing);
        else if (State == GameState.Confirm) ConfirmNo();
        else if (State != GameState.Menu && State != GameState.Over) SetState(GameState.Menu);
    }

    public void SetState(GameState next)
    {
        if (next == GameState.Confirm && State != GameState.Confirm) previous = State;
        State = next;

        menuPanel.SetActive(next == GameState.Menu);
        loadoutPanel.SetActive(next == GameState.Loadout);
        howToPanel.SetActive(next == GameState.HowTo);
        settingsPanel.SetActive(next == GameState.Settings);
        shopPanel.SetActive(next == GameState.Shop);
        missionsPanel.SetActive(next == GameState.Missions);
        hudPanel.SetActive(next == GameState.Playing || next == GameState.Paused);
        pausePanel.SetActive(next == GameState.Paused);
        overPanel.SetActive(next == GameState.Over);
        confirmPanel.SetActive(next == GameState.Confirm);

        Time.timeScale = next == GameState.Playing ? 1f : 0f;   // time runs only while Playing
        AudioListener.pause = next == GameState.Paused;

        // a menu that appears mid-game needs a selected button or the keyboard does nothing
        EventSystem.current.SetSelectedGameObject(next switch
        {
            GameState.Menu => menuFirst,
            GameState.Paused => pauseFirst,
            GameState.Over => overFirst,
            GameState.Confirm => confirmFirst,
            _ => null
        });
    }

    // Inspector buttons can pass one int: use the GameState number
    // 0 Menu, 1 Loadout, 2 HowTo, 3 Settings, 4 Shop, 5 Missions, 6 Playing, 7 Paused, 8 Over, 9 Confirm
    public void Go(int state) => SetState((GameState)state);

    // Confirm dialog
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
        confirmBody.SetText("Your distance and coins from this run are lost.");
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
        else SetState(GameState.Menu);
        pending = Pending.None;
    }

    public void ConfirmNo() => SetState(previous);
}