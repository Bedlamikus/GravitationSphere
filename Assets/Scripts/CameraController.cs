using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private Transform target;
    [SerializeField] private float smoothTime = 0.2f;
    [SerializeField] private float maxSpeed = 50f;
    [SerializeField] private Vector3 offset = new Vector3(0, 0, -10);

    private Vector3 velocity;

    public void SetTarget(Transform target)
    {
        this.target = target;
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
