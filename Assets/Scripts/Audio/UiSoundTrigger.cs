using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Selectable))]
public class UiSoundTrigger : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler,
                              ISelectHandler, IPointerClickHandler, ISubmitHandler
{
    [SerializeField] UiSound press = UiSound.Click;   // set to Back on BackButton instances
    [SerializeField] bool hover = true;
    Selectable sel;
    bool pointerOver;

    void Awake() => sel = GetComponent<Selectable>();
    void OnDisable() => pointerOver = false;

    public void OnPointerEnter(PointerEventData e)
    {
        pointerOver = true;
        if (hover && sel.interactable && !Application.isMobilePlatform) UiSounds.Play(UiSound.Hover);
    }

    public void OnPointerExit(PointerEventData e) => pointerOver = false;

    // Keyboard / gamepad only. A mouse press or a touch also "selects" the button, so skip those.
    public void OnSelect(BaseEventData e)
    {
        if (pointerOver || Application.isMobilePlatform) return;
        if (hover && sel.interactable && Time.unscaledTime - UiSounds.LastPanelChange > 0.2f)
            UiSounds.Play(UiSound.Hover);
    }

    public void OnPointerClick(PointerEventData e) => Press();
    public void OnSubmit(BaseEventData e) => Press();

    void Press() => UiSounds.Play(sel.interactable ? press : UiSound.Error);
}