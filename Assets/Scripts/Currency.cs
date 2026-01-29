using UnityEngine;

public static class Currency
{
    private const string CurrencyKey = "PlayerCurrency";

    public static void AddCurrency(int amount)
    {
        int current = GetCurrency();
        current += amount;
        PlayerPrefs.SetInt(CurrencyKey, current);
        PlayerPrefs.Save();
    }

    public static int GetCurrency()
    {
        return PlayerPrefs.GetInt(CurrencyKey, 0);
    }
}
