using UnityEngine;

public class CurrencyDebugHotkeys : MonoBehaviour
{
    [SerializeField] private KeyCode addKey = KeyCode.M;
    [SerializeField] private int addAmount = 100;

    private void Update()
    {
        if (Input.GetKeyDown(addKey))
            Currency.AddCurrency(addAmount);
    }
}

