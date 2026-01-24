using UnityEngine;

public class TeleportTrigger : MonoBehaviour
{
    [SerializeField] private Transform target;

    private void OnTriggerEnter(Collider other)
    {
        var sphere = other.GetComponent<Sphere>();
        if (sphere == null) return;
        if (target == null) return;

        sphere.MoveToPosition(target.position);
    }
}
