using UnityEngine;

/// <summary>
/// Контроллер двери. Открывает дверь при событии NextPlatform, вращая вокруг указанной локальной оси
/// с плавной анимацией по кривой. При OnEnable возвращает исходные углы.
/// </summary>
public class DoorController : MonoBehaviour
{
    [Header("Настройки вращения")]
    [Tooltip("Локальная ось вращения (X, Y или Z)")]
    [SerializeField] private Axis rotationAxis = Axis.Y;
    
    [Tooltip("Угол открытия двери в градусах")]
    [SerializeField] private float openAngle = 90f;
    
    [Tooltip("Скорость анимации (чем больше значение, тем быстрее)")]
    [SerializeField] private float animationSpeed = 1f;
    
    [Tooltip("Кривая анимации открытия (0 = закрыто, 1 = открыто)")]
    [SerializeField] private AnimationCurve openCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);
    
    [Header("Настройки закрытия")]
    [Tooltip("Кривая анимации закрытия (0 = открыто, 1 = закрыто)")]
    [SerializeField] private AnimationCurve closeCurve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);
    
    private Vector3 _initialRotation;
    private bool _isOpen;
    private bool _isAnimating;
    private float _currentAnimationTime;
    
    private enum Axis
    {
        X,
        Y,
        Z
    }
    
    private void Awake()
    {
        // Сохраняем исходные углы при первом создании
        _initialRotation = transform.localEulerAngles;
    }
    
    private void Start()
    {
        // Подписываемся на событие показа кнопки следующей платформы
        if (GlobalEvents.ShowNextPlatformEvent != null)
        {
            GlobalEvents.ShowNextPlatformEvent.AddListener(OnShowNextPlatform);
        }
    }
    
    private void OnEnable()
    {
        // Возвращаем исходные углы
        transform.localEulerAngles = _initialRotation;
        _isOpen = false;
        _isAnimating = false;
        _currentAnimationTime = 0f;
    }
    
    private void OnDestroy()
    {
        // Отписываемся от события
        if (GlobalEvents.ShowNextPlatformEvent != null)
        {
            GlobalEvents.ShowNextPlatformEvent.RemoveListener(OnShowNextPlatform);
        }
    }
    
    private void Update()
    {
        if (!_isAnimating) return;
        
        if (animationSpeed <= 0f)
        {
            _isAnimating = false;
            return;
        }
        
        _currentAnimationTime += Time.deltaTime * animationSpeed;
        
        if (_currentAnimationTime >= 1f)
        {
            _currentAnimationTime = 1f;
            _isAnimating = false;
        }
        
        // Выбираем кривую в зависимости от направления анимации
        AnimationCurve curve = _isOpen ? openCurve : closeCurve;
        float curveValue = curve.Evaluate(_currentAnimationTime);
        
        // Вычисляем текущий угол
        // При открытии: от 0 до openAngle
        // При закрытии: от openAngle до 0
        float currentAngle = _isOpen 
            ? Mathf.Lerp(0f, openAngle, curveValue)
            : Mathf.Lerp(openAngle, 0f, curveValue);
        
        // Применяем вращение вокруг выбранной оси
        Vector3 newRotation = _initialRotation;
        switch (rotationAxis)
        {
            case Axis.X:
                newRotation.x += currentAngle;
                break;
            case Axis.Y:
                newRotation.y += currentAngle;
                break;
            case Axis.Z:
                newRotation.z += currentAngle;
                break;
        }
        
        transform.localEulerAngles = newRotation;
    }
    
    private void OnShowNextPlatform()
    {
        if (_isAnimating || _isOpen) return;
        
        _isOpen = true;
        _isAnimating = true;
        _currentAnimationTime = 0f;
    }
    
    /// <summary>
    /// Закрыть дверь вручную (опционально)
    /// </summary>
    public void CloseDoor()
    {
        if (_isAnimating || !_isOpen) return;
        
        _isOpen = false;
        _isAnimating = true;
        _currentAnimationTime = 0f;
        
        // Для закрытия используем обратную кривую
        // Переворачиваем время для правильной анимации закрытия
    }
    
    /// <summary>
    /// Открыть дверь вручную (опционально)
    /// </summary>
    public void OpenDoor()
    {
        if (_isAnimating || _isOpen) return;
        
        _isOpen = true;
        _isAnimating = true;
        _currentAnimationTime = 0f;
    }
}
