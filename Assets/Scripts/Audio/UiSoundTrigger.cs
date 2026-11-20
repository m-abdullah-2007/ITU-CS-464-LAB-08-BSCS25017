using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Selectable))]
public class UiSoundTrigger : MonoBehaviour, IPointerEnterHandler, ISelectHandler, IPointerClickHandler, ISubmitHandler
{
    [SerializeField] UiSound press = UiSound.Click;   // set to Back on BackButton instances
    [SerializeField] bool hover = true;
    Selectable sel;

    void Awake() => sel = GetComponent<Selectable>();

    public void OnPointerEnter(PointerEventData e)
    {
        if (hover && sel.interactable && !Application.isMobilePlatform) UiSounds.Play(UiSound.Hover);  // touch has no hover
    }

    public void OnSelect(BaseEventData e)    // keyboard / gamepad navigation
    {
        if (hover && sel.interactable && Time.unscaledTime - UiSounds.LastPanelChange > 0.2f)
            UiSounds.Play(UiSound.Hover);    // 0.2 s guard: UIManager pre-selects a button on every panel open
    }

    public void OnPointerClick(PointerEventData e) => Press();
    public void OnSubmit(BaseEventData e) => Press();

    // A disabled button (NEED 50, IN PROGRESS, greyed arrows) still gets the click:
    // another component on the same object receives it, and we answer with the error sound.
    void Press() => UiSounds.Play(sel.interactable ? press : UiSound.Error);
}
