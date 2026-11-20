using UnityEngine;

public class PilotController : MonoBehaviour
{
    public static PilotController Instance { get; private set; }

    [SerializeField] float lift = 22f, gravity = 18f, maxUp = 9f, maxDown = 12f, top = 6f, bottom = -6f;
    float vy;
    bool held;

    void Awake() => Instance = this;
    void OnDestroy() { if (Instance == this) Instance = null; }

    void OnEnable()  { HoldZone.HoldChanged += OnHold; GameSession.GameStarted += ResetPilot; }
    void OnDisable() { HoldZone.HoldChanged -= OnHold; GameSession.GameStarted -= ResetPilot; }

    void OnHold(bool h) => held = h;
    void ResetPilot() { vy = 0f; Vector3 p = transform.position; p.y = 0f; transform.position = p; }

    void Update()
    {
        if (UIManager.Instance.State != GameState.Playing) return;
        vy += (held ? lift : -gravity) * Time.deltaTime;
        vy = Mathf.Clamp(vy, -maxDown, maxUp);
        Vector3 p = transform.position;
        p.y += vy * Time.deltaTime;
        if (p.y > top)    { p.y = top;    vy = 0f; }
        if (p.y < bottom) { p.y = bottom; vy = 0f; }
        transform.position = p;
    }
}
