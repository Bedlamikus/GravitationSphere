using UnityEngine;

public class NextPlatformTrigger : MonoBehaviour
{
    [Tooltip("Визуальная часть триггера следующей платформы. Отключается после входа персонажа.")]
    [SerializeField] private GameObject visual;

    private bool _rewardGranted;

    private void OnEnable()
    {
        if (visual != null)
            visual.SetActive(true);

        _rewardGranted = false;
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

            GrantPlatformTriggerReward(character.transform.position);
            return;
        }

        var sphere = other.GetComponent<Sphere>();
        if (sphere == null) return;

        //sphere.SetDefaultMask();

        GrantPlatformTriggerReward(sphere.transform.position);
        GlobalEvents.NextPlatform.Invoke();
    }

    private void GrantPlatformTriggerReward(Vector3 userPosition)
    {
        if (_rewardGranted) return;
        _rewardGranted = true;

        const float reward = 50f;

        Currency.AddCurrency(Mathf.RoundToInt(reward));

        Camera cam = Camera.main;
        Vector3 spawnPos = cam != null
            ? (userPosition + cam.transform.position) * 0.5f
            : userPosition;

        GlobalEvents.CurrencyImpactReward.Invoke(spawnPos, reward);
    }
}
