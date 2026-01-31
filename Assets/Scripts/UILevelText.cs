using TMPro;
using UnityEngine;

public class UILevelText : MonoBehaviour
{
    [SerializeField] private TMP_Text lvlText;

    private int lvlIndex = 0;

    private void Awake()
    {
        GlobalEvents.NextPlatform.AddListener(SetLvlText);
        GlobalEvents.GameOver.AddListener(RestartLevel);
        GlobalEvents.RestartLevel.AddListener(RestartLevel);
        SetLvlText();
    }

    private void OnDestroy()
    {
        GlobalEvents.NextPlatform.RemoveListener(SetLvlText);
        GlobalEvents.GameOver.RemoveListener(RestartLevel);
        GlobalEvents.RestartLevel.RemoveListener(RestartLevel);
    }

    private void SetLvlText()
    {
        lvlIndex++;
        lvlText.text = lvlIndex.ToString();
    }

    private void RestartLevel()
    {
        lvlIndex = 0;
        SetLvlText(); // сразу показать "1" при возврате на первую платформу
    }
}
