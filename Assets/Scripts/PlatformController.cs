using UnityEngine;

public class PlatformController : MonoBehaviour
{
    [SerializeField] private Transform[] targets;
    [SerializeField] private CameraController cameraController;

    private int currentTargetIndex = 0;

    private void Start()
    {
        SetNextTarget();
        GlobalEvents.NextPlatform.AddListener(SetNextTarget);
    }

    private void SetNextTarget()
    {
        if (currentTargetIndex < targets.Length)
        {
            if (currentTargetIndex != 0)
            {
                targets[currentTargetIndex - 1].gameObject.SetActive(false);
            }
            cameraController.SetTarget(targets[currentTargetIndex]);
            currentTargetIndex++;
        }
    }
}
