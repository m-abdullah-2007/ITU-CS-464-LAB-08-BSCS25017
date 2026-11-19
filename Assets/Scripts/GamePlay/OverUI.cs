using UnityEngine;
using TMPro;

public class OverUI : MonoBehaviour
{
    [SerializeField] TMP_Text distanceValue, bestLine, coinsLine;
    [SerializeField] GameObject newBestTag;

    void OnEnable()
    {
        Fx.Count(this, distanceValue, 0, GameSession.LastMeters, "{0} m", 0.9f);                 // counts up
        bestLine.SetText("BEST   {0} m", GameData.Best);
        Fx.Count(this, coinsLine, 0, GameSession.LastCoins, "COINS   +{0}", 0.6f, 0.3f);
        newBestTag.SetActive(GameSession.LastWasNewBest);   // second cue for "this is a record"
    }
}