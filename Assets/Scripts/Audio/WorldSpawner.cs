using UnityEngine;

public class WorldSpawner : MonoBehaviour
{
    [SerializeField] GameObject coinPrefab, zapperPrefab;
    [SerializeField] float interval = 0.9f, speed = 5f, spawnX = 5f, yRange = 5.5f;
    float timer;

    void OnEnable()  => GameSession.GameStarted += Clear;
    void OnDisable() => GameSession.GameStarted -= Clear;

    void Clear() { foreach (Transform c in transform) Destroy(c.gameObject); timer = 0f; }

    void Update()
    {
        if (UIManager.Instance.State != GameState.Playing) return;
        timer += Time.deltaTime;
        if (timer < interval) return;
        timer = 0f;
        Spawn(Random.value < 0.65f ? coinPrefab : zapperPrefab);
    }

    void Spawn(GameObject prefab)
    {
        // parent = this transform, so the item belongs to the Gameplay scene and is unloaded with it
        var go = Instantiate(prefab, new Vector3(spawnX, Random.Range(-yRange, yRange), 0f), Quaternion.identity, transform);
        go.GetComponent<ScrollingItem>().speed = speed;
    }
}
