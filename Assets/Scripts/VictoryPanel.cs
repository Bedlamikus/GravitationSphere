using UnityEngine;
using UnityEngine.UI;

public class VictoryPanel : MonoBehaviour
{
    [SerializeField] private Button aganeButton;

    private void Start()
    {
        GlobalEvents.RestartLevel.AddListener(Show);
        aganeButton.onClick.AddListener(Agane);
        gameObject.SetActive(false);
    }

    private void Agane()
    {
        GlobalEvents.StartLevel.Invoke();
        gameObject.SetActive(false);
    }

    private void Show()
    {
        gameObject.SetActive(true);
    }
}
