using UnityEngine;
using TMPro;

public class CoinLabel : MonoBehaviour
{
    [SerializeField] TMP_Text label;

    void OnEnable()
    {
        label.SetText("{0}", GameData.Coins);
    }
}