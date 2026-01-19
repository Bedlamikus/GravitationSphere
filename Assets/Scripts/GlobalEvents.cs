using UnityEngine;
using UnityEngine.Events;

public class GlobalEvents : MonoBehaviour
{
    public static UnityEvent<float> AddTorqueX = new();
    public static UnityEvent<float> AddTorqueZ = new();
}
