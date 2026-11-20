using UnityEngine;

public class ScrollingItem : MonoBehaviour
{
    public enum Kind { Coin, Zapper }
    [SerializeField] Kind kind;
    [SerializeField] float hitX = 0.6f, hitY = 0.7f;
    [HideInInspector] public float speed = 5f;

    void Update()
    {
        if (UIManager.Instance.State != GameState.Playing) return;
        transform.Translate(Vector3.left * speed * Time.deltaTime, Space.World);
        if (transform.position.x < -6f) { Destroy(gameObject); return; }

        PilotController p = PilotController.Instance;
        if (p == null) return;
        Vector3 d = transform.position - p.transform.position;
        if (Mathf.Abs(d.x) > hitX || Mathf.Abs(d.y) > hitY) return;

        if (kind == Kind.Coin) GameSession.Instance.AddCoin(); else GameSession.Instance.TakeHit();
        Destroy(gameObject);       // safe: the sound is played by a listener that outlives this object
    }
}
