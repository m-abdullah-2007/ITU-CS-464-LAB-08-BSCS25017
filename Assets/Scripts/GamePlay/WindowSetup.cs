using UnityEngine;

public class WindowSetup : MonoBehaviour
{
#if UNITY_STANDALONE
    void Awake() => Screen.SetResolution(540, 960, FullScreenMode.Windowed);
#endif
}