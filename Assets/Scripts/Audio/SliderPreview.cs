using UnityEngine;
using UnityEngine.EventSystems;

public class SliderPreview : MonoBehaviour, IPointerUpHandler
{
    public enum Group { None, Ui, Sfx }
    [SerializeField] Group group = Group.Ui;     // Master + UI = Ui, SFX = Sfx, Music = None (you can already hear it)
    [SerializeField] AudioClip sfxClip;          // e.g. the coin pickup, for the SFX slider

    public void OnPointerUp(PointerEventData e)
    {
        if (group == Group.Ui) UiSounds.Play(UiSound.Click);
        else if (group == Group.Sfx && SfxPlayer.Instance != null) SfxPlayer.Instance.Play(sfxClip);
    }
}
