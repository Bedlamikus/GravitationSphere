using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip notEnoughCurrencyClip;
    
    [Header("Currency Spent")]
    [SerializeField] private AudioClip currencySpentClip;
    
    [Header("Background Music")]
    [SerializeField] private AudioSource backgroundMusicSource;
    [SerializeField] private AudioClip backgroundMusicClip;

    private void Start()
    {
        // Если AudioSource не задан, пытаемся получить его на этом объекте
        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
        }
        
        // Если все еще нет AudioSource, создаем его
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
        
        // Если AudioSource для фоновой музыки не задан, создаем его
        if (backgroundMusicSource == null)
        {
            backgroundMusicSource = gameObject.AddComponent<AudioSource>();
            backgroundMusicSource.loop = true;
        }
        
        // Подписываемся на события
        GlobalEvents.NotEnoughCurrency.AddListener(OnNotEnoughCurrency);
        GlobalEvents.CurrencySpent.AddListener(OnCurrencySpent);
        GlobalEvents.NextPlatform.AddListener(OnNextPlatform);
    }
    
    private void OnDestroy()
    {
        // Отписываемся от событий
        GlobalEvents.NotEnoughCurrency.RemoveListener(OnNotEnoughCurrency);
        GlobalEvents.CurrencySpent.RemoveListener(OnCurrencySpent);
        GlobalEvents.NextPlatform.RemoveListener(OnNextPlatform);
    }
    
    private void OnNotEnoughCurrency()
    {
        if (audioSource != null && notEnoughCurrencyClip != null)
        {
            audioSource.PlayOneShot(notEnoughCurrencyClip);
        }
    }
    
    private void OnCurrencySpent()
    {
        // Воспроизводим клип успешного списания
        if (audioSource != null && currencySpentClip != null)
        {
            audioSource.PlayOneShot(currencySpentClip);
        }
        
        // Запускаем фоновую музыку
        if (backgroundMusicSource != null && backgroundMusicClip != null && !backgroundMusicSource.isPlaying)
        {
            backgroundMusicSource.clip = backgroundMusicClip;
            backgroundMusicSource.Play();
        }
    }
    
    private void OnNextPlatform()
    {
        // Выключаем фоновую музыку при переносе на новую платформу
        if (backgroundMusicSource != null && backgroundMusicSource.isPlaying)
        {
            backgroundMusicSource.Stop();
        }
    }
}
