using UnityEngine;
using UnityEngine.Events;

public class GlobalEvents : MonoBehaviour
{
    public static UnityEvent<float> AddTorqueX = new();
    public static UnityEvent<float> AddTorqueZ = new();

    public static UnityEvent<Vector3, float> CurrencyImpactReward = new();

    public static UnityEvent NextPlatform = new();
    public static UnityEvent ShowNextPlatformEvent = new();
    public static UnityEvent RestartLevel = new();
    public static UnityEvent StartLevel = new();
    public static UnityEvent GameOver = new();
    public static UnityEvent CharacterInPortal = new();
}
