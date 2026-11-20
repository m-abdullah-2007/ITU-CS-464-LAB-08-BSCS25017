using UnityEngine;

public class CameraShake : MonoBehaviour
{
    [SerializeField] float decay = 1.6f, maxOffset = 0.35f, maxAngle = 4f, frequency = 18f;
    float trauma;

    void OnEnable()  { GameSession.Damaged += OnDamaged; GameSession.GameLost += OnLost; }
    void OnDisable() { GameSession.Damaged -= OnDamaged; GameSession.GameLost -= OnLost; }

    void OnDamaged(int current, int max) => AddTrauma(0.55f);
    void OnLost(int meters, int coins, bool best) => AddTrauma(0.9f);
    void AddTrauma(float amount) => trauma = Mathf.Min(1f, trauma + amount);

    void LateUpdate()
    {
        trauma = Mathf.Max(0f, trauma - decay * Time.unscaledDeltaTime);
        float k = trauma * trauma, t = Time.unscaledTime * frequency;
        transform.localPosition = new Vector3(Noise(0f, t), Noise(1f, t), 0f) * maxOffset * k;
        transform.localRotation = Quaternion.Euler(0f, 0f, Noise(2f, t) * maxAngle * k);
    }
    static float Noise(float seed, float t) => Mathf.PerlinNoise(seed * 10f, t) * 2f - 1f;
}
