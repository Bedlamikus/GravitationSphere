using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class NextPlatformTrigger : MonoBehaviour
{
    [Tooltip("Визуальная часть триггера следующей платформы. Отключается после входа персонажа.")]
    [SerializeField] private GameObject visual;
    
    [Tooltip("Точка позиционирования игрока. Если не задана, используется позиция самого триггера.")]
    [SerializeField] private Transform playerPositionPoint;
    
    [Tooltip("Цена прохода через триггер. Если у игрока недостаточно денег, триггер не сработает.")]
    [SerializeField] private int price = 0;

    [SerializeField] private TMP_Text priceText;
    [SerializeField] private Color enoughCurrencyColor = Color.green;
    [SerializeField] private Color notEnoughCurrencyColor = Color.red;

    [Header("Events")]
    [Tooltip("Вызывается, когда персонаж успешно активировал триггер (прошёл проверку цены).")]
    [SerializeField] private UnityEvent onActivated = new UnityEvent();

    public UnityEvent OnActivated => onActivated;

    private bool _rewardGranted;

    private void OnEnable()
    {
        if (visual != null)
            visual.SetActive(true);

        ShowPrice(true);
        if (priceText != null)
            priceText.text = price.ToString();

        UpdatePriceColor();
        _rewardGranted = false;
    }

    private void Update()
    {
        UpdatePriceColor();
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
                    ShowPrice(false);
                }
            }
            
            if (visual != null)
                visual.SetActive(false);

            // Собираем персонажа в заданной точке позиционирования или в позиции триггера
            Transform assemblePoint = playerPositionPoint != null ? playerPositionPoint : transform;
            character.ReturnBonesAndEnableAnimator(assemblePoint);
            GlobalEvents.ShowNextPlatformEvent.Invoke();
            onActivated.Invoke();

            GrantPlatformTriggerReward(character.transform.position);
            return;
        }

        var sphere = other.GetComponent<Sphere>();
        if (sphere == null) return;

        //sphere.SetDefaultMask();

        GrantPlatformTriggerReward(sphere.transform.position);
        GlobalEvents.NextPlatform.Invoke();
    }

    private void ShowPrice(bool show)
    {
        if (priceText == null) return;
        priceText.gameObject.SetActive(show);
    }

    private void UpdatePriceColor()
    {
        if (priceText == null) return;

        bool enough = price <= 0 || Currency.GetCurrency() >= price;
        priceText.color = enough ? enoughCurrencyColor : notEnoughCurrencyColor;
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
