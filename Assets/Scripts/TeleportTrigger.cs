using UnityEngine;

public class TeleportTrigger : MonoBehaviour
{
    [SerializeField] private Transform target;

    private void OnTriggerEnter(Collider other)
    {
        var sphere = other.GetComponent<Sphere>();
        if (sphere == null) return;
        if (target == null) return;

        var rb = sphere.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.position = target.position;
            Physics.SyncTransforms();
        }
        else
        {
            sphere.MoveToPosition(target.position);
        }
    }
}
