using UnityEngine;

public class HapticsListener : MonoBehaviour
{
    void OnEnable()  => GameSession.Damaged += Buzz;
    void OnDisable() => GameSession.Damaged -= Buzz;
    void Buzz(int current, int max) => SettingsUI.Buzz();
}
