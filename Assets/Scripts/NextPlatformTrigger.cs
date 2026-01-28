using UnityEngine;

public class NextPlatformTrigger : MonoBehaviour
{
    [Tooltip("Визуальная часть триггера следующей платформы. Отключается после входа персонажа.")]
    [SerializeField] private GameObject visual;

    private void OnEnable()
    {
        if (visual != null)
            visual.SetActive(true);
    }

    private void OnTriggerEnter(Collider other)
    {
        var character = other.GetComponentInParent<RagdollCharacter>();
        if (character != null)
        {
            if (visual != null)
                visual.SetActive(false);

            // Собираем персонажа в позиции самого триггера (assemblePoint = transform)
            character.ReturnBonesAndEnableAnimator(transform);
            GlobalEvents.ShowNextPlatformEvent.Invoke();
            return;
        }

        var sphere = other.GetComponent<Sphere>();
        if (sphere == null) return;

        //sphere.SetDefaultMask();

        GlobalEvents.NextPlatform.Invoke();
    }
}
