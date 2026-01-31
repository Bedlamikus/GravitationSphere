using UnityEngine;
using UnityEngine.UI;

public class VictoryPanel : MonoBehaviour
{
    [SerializeField] private Button aganeButton;

    private void Start()
    {
        GlobalEvents.GameOver.AddListener(Show);
        aganeButton.onClick.AddListener(Agane);
        gameObject.SetActive(false);
    }

    private void Agane()
    {
        // Сбрасываем уровень/платформы на стартовую, а затем "запускаем" уровень.
        GlobalEvents.RestartLevel.Invoke();
        GlobalEvents.StartLevel.Invoke();
        gameObject.SetActive(false);
    }

    private void Show()
    {
        gameObject.SetActive(true);
    }
}
