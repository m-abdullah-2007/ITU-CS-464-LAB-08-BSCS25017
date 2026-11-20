using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class HoldZone : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public static event Action<bool> HoldChanged;     // true = finger down
    bool held;

    public void OnPointerDown(PointerEventData e) => Set(true);
    public void OnPointerUp(PointerEventData e)   => Set(false);
    void OnDisable() => Set(false);

    void Set(bool h) { if (held == h) return; held = h; HoldChanged?.Invoke(h); }
}
