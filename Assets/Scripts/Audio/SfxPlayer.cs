using UnityEngine;

public class SfxPlayer : MonoBehaviour
{
    public static SfxPlayer Instance { get; private set; }

    [SerializeField] AudioSource[] sources;      // Src0..Src3, Output = SFX
    [SerializeField] float pitchRange = 2f;      // +/- semitones
    [SerializeField] float minVolume = 0.85f;
    int next;

    void Awake() => Instance = this;
    void OnDestroy() { if (Instance == this) Instance = null; }

    public void Play(AudioClip clip, float volume = 1f)
    {
        if (clip == null) return;
        AudioSource s = sources[next];
        next = (next + 1) % sources.Length;               // pitch belongs to the source: rotate
        s.pitch = Mathf.Pow(2f, Random.Range(-pitchRange, pitchRange) / 12f);
        s.PlayOneShot(clip, volume * Random.Range(minVolume, 1f));
    }
}