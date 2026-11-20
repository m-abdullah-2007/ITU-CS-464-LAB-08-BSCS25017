using UnityEngine;

public class MusicDirector : MonoBehaviour
{
    [SerializeField] MusicPlayer music;
    [SerializeField] AudioClip menuTrack, gameTrack, resultsTrack;

    void OnEnable()  => UIManager.StateChanged += OnState;
    void OnDisable() => UIManager.StateChanged -= OnState;
    void Start() => music.CrossfadeTo(menuTrack);

    void OnState(GameState prev, GameState next)
    {
        switch (next)
        {
            case GameState.Playing: music.CrossfadeTo(gameTrack); break;      // no-op if already playing (Resume)
            case GameState.Over:    music.CrossfadeTo(resultsTrack); break;
            case GameState.Paused:
            case GameState.Confirm:
            case GameState.Loading: break;                                    // keep whatever is playing
            default:                music.CrossfadeTo(menuTrack); break;      // Menu and every sub-screen
        }
    }
}
