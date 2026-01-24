using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private float smoothTime = 0.2f;
    [SerializeField] private float maxSpeed = 50f;
    [SerializeField] private Vector3 offset = new Vector3(0, 0, -10);
    [SerializeField] private float leadAmount = 2f;
    [SerializeField] [Tooltip("Время сглаживания прироста lead-смещения (меньше = быстрее отклик)")]
    private float leadSmoothTime = 0.15f;
    [SerializeField] [Tooltip("Макс. скорость прироста lead-смещения (единиц/сек)")]
    private float leadGrowthSpeed = 20f;
    [SerializeField] [Tooltip("При расстоянии больше этого значения - мгновенный телепорт")]
    private float maxTeleportDistance = 50f;

    private Vector3 velocity;
    private Vector3 currentLeadOffset;
    private Vector3 leadVelocity;
    private Rigidbody targetRb;

    public void SetTarget(Transform target)
    {
        this.target = target;
        targetRb = target != null ? target.GetComponent<Rigidbody>() : null;
    }

    private const float DistanceThreshold = 20f;
    private const float VelocityEpsilon = 0.01f;

    private void LateUpdate()
    {
        if (target == null) return;

        if (targetRb == null)
            targetRb = target.GetComponent<Rigidbody>();

        Vector3 targetLead = Vector3.zero;
        if (leadAmount > 0f && targetRb != null)
        {
            Vector3 vel = targetRb.velocity;
            vel.z = 0f;
            if (vel.sqrMagnitude > VelocityEpsilon * VelocityEpsilon)
            {
                targetLead = vel.normalized * leadAmount;
            }
        }

        currentLeadOffset = Vector3.SmoothDamp(currentLeadOffset, targetLead, ref leadVelocity, leadSmoothTime, leadGrowthSpeed);

        Vector3 targetPosition = target.position + offset + currentLeadOffset;
        float distance = Vector3.Distance(transform.position, targetPosition);
        
        // При очень большом расстоянии - мгновенный телепорт ближе к цели
        if (distance > maxTeleportDistance)
        {
            Debug.Log($"Camera teleport: расстояние {distance:F1} > {maxTeleportDistance} - мгновенный телепорт на 85%");
            Vector3 newPos = Vector3.Lerp(transform.position, targetPosition, 0.85f);
            transform.position = newPos;
            
            // Сбрасываем скорости
            velocity = Vector3.zero;
            currentLeadOffset = Vector3.zero;
            leadVelocity = Vector3.zero;
            
            // Пересчитываем расстояние
            distance = Vector3.Distance(transform.position, targetPosition);
            Debug.Log($"Camera teleport: камера перемещена, новое расстояние = {distance:F1}");
        }

        float effectiveMaxSpeed = distance > DistanceThreshold
            ? maxSpeed * (distance / DistanceThreshold)
            : maxSpeed;

        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, smoothTime, effectiveMaxSpeed);
    }
}
