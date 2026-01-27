using UnityEngine;

public class TeleportTrigger : MonoBehaviour
{
    [SerializeField] private Transform target;
    [Tooltip("Визуальная часть портала. Отключается при входе игрока, включается снова в OnEnable.")]
    [SerializeField] private GameObject portalVisual;

    private void OnEnable()
    {
        if (portalVisual != null)
            portalVisual.SetActive(true);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (target == null) return;

        var character = other.GetComponentInParent<RagdollCharacter>();
        if (character != null)
        {
            if (portalVisual != null)
                portalVisual.SetActive(false);
            character.EnterPortal(target);
            return;
        }

        var sphere = other.GetComponent<Sphere>();
        if (sphere == null) return;

        if (portalVisual != null)
            portalVisual.SetActive(false);

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
