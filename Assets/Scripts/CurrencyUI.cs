using TMPro;
using UnityEngine;

public class CurrencyUI : MonoBehaviour
{
    [SerializeField] private TMP_Text currencyText;

    [Header("Interpolation")]
    [SerializeField] private float baseSpeed = 50f;
    [SerializeField] private float speedPerUnitDifference = 2f;

    private float _currentDisplayValue;

    private void Start()
    {
        _currentDisplayValue = Currency.GetCurrency();
        SetCurrencyText(_currentDisplayValue);
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
