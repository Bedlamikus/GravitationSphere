using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Иконка-кнопка «Следующая платформа». Подписана на ShowNextPlatformEvent — при событии показывается.
/// По нажатию вызывается NextPlatform, скрывается кнопка, у персонажа State = 0.
/// </summary>
public class NextPlatformButton : MonoBehaviour
{
    [SerializeField] private RagdollCharacter character;
    [SerializeField] private Button button;

    private void Start()
    {
        GlobalEvents.ShowNextPlatformEvent.AddListener(OnShowNextPlatform);
        gameObject.SetActive(false);

        button.onClick.AddListener(OnClick);
    }

    private void OnDestroy()
    {
        GlobalEvents.ShowNextPlatformEvent.RemoveListener(OnShowNextPlatform);

        var button = GetComponent<Button>();
        if (button != null)
            button.onClick.RemoveListener(OnClick);
    }

    private void OnShowNextPlatform()
    {
        gameObject.SetActive(true);
    }

    private void OnClick()
    {
        if (character != null)
            character.ExitRagdollWithState0();

        GlobalEvents.NextPlatform.Invoke();
        gameObject.SetActive(false);
    }
}
