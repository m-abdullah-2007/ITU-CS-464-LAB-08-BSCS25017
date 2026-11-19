using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LoadoutUI : MonoBehaviour
{
    [SerializeField] GameObject[] jetpacks;          // J0, J1, J2 in the preview stage
    [SerializeField] TMP_Text nameLabel, statLabel, equippedTag;
    [SerializeField] Button prevButton, nextButton;
    [SerializeField] Transform previewImage;         // the RawImage, for the punch
    int index;

    void OnEnable() { index = GameData.Equipped; Show(); }

    public void Next() => Step(1);
    public void Prev() => Step(-1);

    void Step(int dir)
    {
        int n = jetpacks.Length;
        for (int i = 1; i <= n; i++)
        {
            int c = ((index + dir * i) % n + n) % n;
            if (GameData.Owns(c)) { index = c; break; }   // skip jetpacks you do not own
        }
        Show();                                       // browsing only: does NOT change what is equipped
        Fx.Punch(this, previewImage, 0.06f);          // the picture reacts to the change
        Fx.Punch(this, nameLabel.transform, 0.12f);
    }

    // START RUN: equip what is on screen, then begin the run
    public void StartRun()
    {
        GameData.Equipped = index;
        GameSession.Instance.StartRun();
    }

    void Show()
    {
        for (int i = 0; i < jetpacks.Length; i++) jetpacks[i].SetActive(i == index);
        nameLabel.SetText(JetpackInfo.Names[index]);
        statLabel.SetText(JetpackInfo.Stats[index]);
        bool isEquipped = index == GameData.Equipped;
        equippedTag.SetText(isEquipped ? "EQUIPPED" : "OWNED");
        equippedTag.color = isEquipped ? new Color32(0x25, 0xD9, 0xD0, 255) : new Color32(0x9F, 0xB0, 0xC8, 255);

        int owned = 0;
        for (int i = 0; i < jetpacks.Length; i++) if (GameData.Owns(i)) owned++;
        prevButton.interactable = nextButton.interactable = owned > 1;   // disabled = clearly greyed
    }
}