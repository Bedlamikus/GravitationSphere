using UnityEngine;

public class NextPlatformTrigger : MonoBehaviour
{
    [Tooltip("Визуальная часть триггера следующей платформы. Отключается после входа персонажа.")]
    [SerializeField] private GameObject visual;
    
    [Tooltip("Точка позиционирования игрока. Если не задана, используется позиция самого триггера.")]
    [SerializeField] private Transform playerPositionPoint;
    
    [Tooltip("Цена прохода через триггер. Если у игрока недостаточно денег, триггер не сработает.")]
    [SerializeField] private int price = 0;

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
            // Проверяем, достаточно ли у игрока денег (только для персонажа)
            if (price > 0)
            {
                if (!Currency.TrySpendCurrency(price))
                {
                    // Недостаточно денег - триггер не срабатывает
                    GlobalEvents.NotEnoughCurrency.Invoke();
                    return;
                }
                else
                {
                    // Успешное списание денег
                    GlobalEvents.CurrencySpent.Invoke();
                }
            }
            
            if (visual != null)
                visual.SetActive(false);

            // Собираем персонажа в заданной точке позиционирования или в позиции триггера
            Transform assemblePoint = playerPositionPoint != null ? playerPositionPoint : transform;
            character.ReturnBonesAndEnableAnimator(assemblePoint);
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
