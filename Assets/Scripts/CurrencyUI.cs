using TMPro;
using UnityEngine;

public class CurrencyUI : MonoBehaviour
{
    [SerializeField] private TMP_Text currencyText;

    [Header("Interpolation")]
    [SerializeField] private float baseSpeed = 50f;
    [SerializeField] private float speedPerUnitDifference = 2f;
    
    [Header("Not Enough Currency Effect")]
    [SerializeField] private Color redColor = Color.red;
    [SerializeField] private float blinkDuration = 0.3f;
    [SerializeField] private int blinkCount = 2;

    private float _currentDisplayValue;
    private Color _originalColor;
    private bool _isBlinking;

    private void Start()
    {
        _currentDisplayValue = Currency.GetCurrency();
        SetCurrencyText(_currentDisplayValue);
        
        if (currencyText != null)
        {
            _originalColor = currencyText.color;
        }
        
        // Подписываемся на событие недостатка валюты
        GlobalEvents.NotEnoughCurrency.AddListener(OnNotEnoughCurrency);
    }
    
    private void OnDestroy()
    {
        // Отписываемся от события
        GlobalEvents.NotEnoughCurrency.RemoveListener(OnNotEnoughCurrency);
    }
    
    private void OnNotEnoughCurrency()
    {
        if (!_isBlinking && currencyText != null)
        {
            StartCoroutine(BlinkRed());
        }
    }
    
    private System.Collections.IEnumerator BlinkRed()
    {
        _isBlinking = true;
        
        for (int i = 0; i < blinkCount; i++)
        {
            currencyText.color = redColor;
            yield return new WaitForSeconds(blinkDuration);
            currencyText.color = _originalColor;
            yield return new WaitForSeconds(blinkDuration);
        }
        
        _isBlinking = false;
    }

    private void Update()
    {
        int targetValue = Currency.GetCurrency();
        float difference = Mathf.Abs(targetValue - _currentDisplayValue);

        if (difference > 0.01f)
        {
            float speed = baseSpeed + difference * speedPerUnitDifference;
            _currentDisplayValue = Mathf.MoveTowards(_currentDisplayValue, targetValue, speed * Time.deltaTime);
        }
        SetCurrencyText(_currentDisplayValue);
    }

    private void SetCurrencyText(float currency)
    {
        currencyText.text = $"{Mathf.RoundToInt(_currentDisplayValue).ToString()}$";
    }
}
