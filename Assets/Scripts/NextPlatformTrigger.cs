using UnityEngine;

public class NextPlatformTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        var sphere = other.GetComponent<Sphere>();
        if (sphere == null) return;

        sphere.SetDefaultMask();

        GlobalEvents.NextPlatform.Invoke();
    }
}
