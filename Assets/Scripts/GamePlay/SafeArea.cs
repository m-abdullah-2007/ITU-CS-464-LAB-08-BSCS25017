using UnityEngine;

public class SafeArea : MonoBehaviour
{
    void Start()
    {
        var rt = (RectTransform)transform;
        Rect s = Screen.safeArea;
        Vector2 min = s.position, max = s.position + s.size;
        min.x /= Screen.width; min.y /= Screen.height;
        max.x /= Screen.width; max.y /= Screen.height;
        rt.anchorMin = min; rt.anchorMax = max;
        rt.offsetMin = rt.offsetMax = Vector2.zero;
    }
}