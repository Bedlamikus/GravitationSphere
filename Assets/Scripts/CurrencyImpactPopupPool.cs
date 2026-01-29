using System.Collections.Generic;
using UnityEngine;

public class CurrencyImpactPopupPool : MonoBehaviour
{
    [Header("Pool")]
    [SerializeField] private CurrencyImpactPopup popupPrefab;
    [SerializeField] private int prewarmCount = 10;
    [SerializeField] private Transform poolParent;

    [Header("Camera")]
    [SerializeField] private Camera targetCamera;

    private readonly List<CurrencyImpactPopup> _pool = new();

    private void Awake()
    {
        if (poolParent == null)
            poolParent = transform;

        Prewarm();
    }

    private void OnEnable()
    {
        GlobalEvents.CurrencyImpactReward.AddListener(OnCurrencyImpactReward);
    }

    private void OnDisable()
    {
        GlobalEvents.CurrencyImpactReward.RemoveListener(OnCurrencyImpactReward);
    }

    private void Prewarm()
    {
        if (popupPrefab == null) return;

        for (int i = _pool.Count; i < prewarmCount; i++)
        {
            var instance = Instantiate(popupPrefab, poolParent);
            instance.gameObject.SetActive(false);
            _pool.Add(instance);
        }
    }

    private void OnCurrencyImpactReward(Vector3 hitPosition, float reward)
    {
        if (popupPrefab == null)
            return;

        if (targetCamera == null)
            targetCamera = Camera.main;

        CurrencyImpactPopup popup = GetFree();
        popup.gameObject.SetActive(true);
        popup.Play(hitPosition, reward, targetCamera);
    }

    private CurrencyImpactPopup GetFree()
    {
        for (int i = 0; i < _pool.Count; i++)
        {
            if (_pool[i] != null && !_pool[i].gameObject.activeSelf)
                return _pool[i];
        }

        var instance = Instantiate(popupPrefab, poolParent);
        instance.gameObject.SetActive(false);
        _pool.Add(instance);
        return instance;
    }
}

