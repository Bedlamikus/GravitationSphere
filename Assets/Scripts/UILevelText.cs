using TMPro;
using UnityEngine;

public class UILevelText : MonoBehaviour
{
    [SerializeField] private TMP_Text lvlText;

    private int lvlIndex = 0;

    private void Awake()
    {
        GlobalEvents.NextPlatform.AddListener(SetLvlText);
        SetLvlText();
    }

    private void SetLvlText()
    {
        lvlIndex++;
        lvlText.text = lvlIndex.ToString();
    }
}
