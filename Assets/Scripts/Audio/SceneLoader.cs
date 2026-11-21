using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public static SceneLoader Instance { get; private set; }
    public static event Action<float> Progress;           // 0..1, shown on the loading screen

    [SerializeField] string gameplayScene = "Gameplay";
    [SerializeField] float minShowSeconds = 1.2f;         // so a tiny scene still shows the bar
    bool loading;

    public bool GameplayLoaded => SceneManager.GetSceneByName(gameplayScene).isLoaded;

    void Awake() => Instance = this;
    void OnEnable()  => UIManager.StateChanged += OnState;
    void OnDisable() => UIManager.StateChanged -= OnState;

    // Wired from LoadoutUI.StartRun
    public void BeginRun()
    {
        if (loading) return;
        if (GameplayLoaded) { GameSession.Instance.StartRun(); return; }   // e.g. retry: world is already there
        StartCoroutine(Load());
    }

    IEnumerator Load()
    {
        loading = true;
        UIManager.Instance.SetState(GameState.Loading);

        AsyncOperation op = SceneManager.LoadSceneAsync(gameplayScene, LoadSceneMode.Additive);
        op.allowSceneActivation = false;                  // stops at 0.9 until we say go

        float shown = 0f, elapsed = 0f;
        while (shown < 1f)
        {
            elapsed += Time.unscaledDeltaTime;
            float real = Mathf.Clamp01(op.progress / 0.9f);          // real progress, 0.9 means "ready"
            float target = Mathf.Min(real, elapsed / minShowSeconds); // never ahead of reality, never instant
            shown = Mathf.MoveTowards(shown, target, 2f * Time.unscaledDeltaTime);
            Progress?.Invoke(shown);
            yield return null;
        }

        yield return new WaitForSecondsRealtime(0.15f);   // let the player see 100%
        op.allowSceneActivation = true;
        while (!op.isDone) yield return null;             // gameplay objects now exist

        loading = false;
        GameSession.Instance.StartRun();                  // only now do we enter Playing
    }

    // Leaving to the menu unloads the world, so the next START RUN shows loading again
    void OnState(GameState prev, GameState next)
    {
        if (next == GameState.Menu && GameplayLoaded && !loading)
            SceneManager.UnloadSceneAsync(gameplayScene);
    }
}
