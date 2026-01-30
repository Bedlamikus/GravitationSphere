using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private float smoothTime = 0.2f;
    [SerializeField] private float maxSpeed = 50f;
    [SerializeField] private Vector3 offset = new Vector3(0, 0, -10);

    [Header("Настройки камеры при входе в триггер")]
    [Tooltip("Новое смещение камеры (для приближения к игроку)")]
    [SerializeField] private Vector3 triggerCameraOffset = new Vector3(0, 0, -5);
    
    [Tooltip("Новая плавность камеры (больше = плавнее)")]
    [SerializeField] private float triggerCameraSmoothTime = 0.5f;
    
    [Tooltip("Новый поворот камеры (Euler angles)")]
    [SerializeField] private Vector3 triggerCameraRotation = Vector3.zero;

    private Vector3 velocity;
    
    // Исходные значения камеры
    private Vector3 _initialOffset;
    private float _initialSmoothTime;
    private Vector3 _initialRotation;

    public void SetTarget(Transform target)
    {
        this.target = target;
    }
    
    public Vector3 GetOffset() => offset;
    public void SetOffset(Vector3 newOffset) => offset = newOffset;
    
    public float GetSmoothTime() => smoothTime;
    public void SetSmoothTime(float newSmoothTime) => smoothTime = newSmoothTime;
    
    public Vector3 GetRotation() => transform.eulerAngles;
    public void SetRotation(Vector3 newRotation) => transform.rotation = Quaternion.Euler(newRotation);

    private void Awake()
    {
        // Сохраняем исходные значения камеры
        _initialOffset = offset;
        _initialSmoothTime = smoothTime;
        _initialRotation = transform.eulerAngles;
    }
    
    private void Start()
    {
        // Подписываемся на события
        if (GlobalEvents.ShowNextPlatformEvent != null)
        {
            GlobalEvents.ShowNextPlatformEvent.AddListener(OnShowNextPlatform);
        }
        
        if (GlobalEvents.NextPlatform != null)
        {
            GlobalEvents.NextPlatform.AddListener(OnNextPlatform);
        }
    }
    
    private void OnDestroy()
    {
        // Отписываемся от событий
        if (GlobalEvents.ShowNextPlatformEvent != null)
        {
            GlobalEvents.ShowNextPlatformEvent.RemoveListener(OnShowNextPlatform);
        }
        
        if (GlobalEvents.NextPlatform != null)
        {
            GlobalEvents.NextPlatform.RemoveListener(OnNextPlatform);
        }
    }
    
    private void OnShowNextPlatform()
    {
        // Применяем новые параметры камеры
        SetOffset(triggerCameraOffset);
        SetSmoothTime(triggerCameraSmoothTime);
        SetRotation(triggerCameraRotation);
    }
    
    private void OnNextPlatform()
    {
        // Возвращаем исходные параметры камеры
        SetOffset(_initialOffset);
        SetSmoothTime(_initialSmoothTime);
        SetRotation(_initialRotation);
    }

    private const float DistanceThreshold = 20f;

    private void LateUpdate()
    {
        if (target == null) return;

        Vector3 targetPosition = target.position + offset;
        float distance = Vector3.Distance(transform.position, targetPosition);

        float effectiveMaxSpeed = distance > DistanceThreshold
            ? maxSpeed * (distance / DistanceThreshold)
            : maxSpeed;

        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, smoothTime, effectiveMaxSpeed);
    }
}
