using System.Collections;
using UnityEngine;

public class MusicPlayer : MonoBehaviour
{
    [SerializeField] AudioSource a, b;
    [SerializeField] float fadeSeconds = 1.5f;
    AudioSource current;

    void Awake()
    {
        current = a;
        a.ignoreListenerPause = true;     // music keeps playing while paused (ducked by the snapshot)
        b.ignoreListenerPause = true;
    }

    public void CrossfadeTo(AudioClip clip)
    {
        if (clip == null || (current.clip == clip && current.isPlaying)) return;
        AudioSource next = current == a ? b : a;
        next.clip = clip;
        next.volume = 0f;
        next.Play();
        StopAllCoroutines();
        StartCoroutine(Fade(current, next));
        current = next;
    }

    IEnumerator Fade(AudioSource from, AudioSource to)
    {
        float start = from.volume;
        for (float t = 0f; t < fadeSeconds; t += Time.unscaledDeltaTime)   // unscaled: works while paused
        {
            float u = t / fadeSeconds;
            from.volume = start * Mathf.Cos(u * Mathf.PI / 2f);            // equal power: no dip in the middle
            to.volume = Mathf.Sin(u * Mathf.PI / 2f);
            yield return null;
        }
        from.Stop();
        to.volume = 1f;
    }
}