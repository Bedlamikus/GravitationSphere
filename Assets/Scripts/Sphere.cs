using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sphere : MonoBehaviour
{
    [SerializeField] private float speed = 5f;
    [SerializeField] private float torqueForce = 1f;
    [SerializeField] private FloatingJoystick joystick;
    [SerializeField] private float fallThreshold = -10f;
    [SerializeField] private PlatformController platformController;

    private Rigidbody rb;
    private Vector3 startPoint;

    private void Start()
    {
        startPoint = transform.position;

        rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.maxAngularVelocity = speed;
        }
        
        GlobalEvents.StartLevel.AddListener(Unfreeze);
    }

    private void OnDestroy()
    {
        GlobalEvents.StartLevel.RemoveListener(Unfreeze);
    }

    private void FixedUpdate()
    {
        if (joystick == null) return;

        if (rb == null || rb.isKinematic) return;

        // Проверяем, не упала ли сфера слишком низко
        if (transform.position.y < fallThreshold)
        {
            ReturnToCurrentPlatform();
            return;
        }

        float verticalInput = joystick.Vertical;
        float horizontalInput = joystick.Horizontal;

        if (Mathf.Abs(verticalInput) > 0.01f)
        {
            rb.AddTorque(Vector3.right * torqueForce * verticalInput, ForceMode.Force);
        }

        if (Mathf.Abs(horizontalInput) > 0.01f)
        {
            rb.AddTorque(-Vector3.forward * torqueForce * horizontalInput, ForceMode.Force);
        }
        
    }

    public void MoveToPosition(Vector3 newPosition)
    {
        transform.position = newPosition;
    }

    public void Freeze()
    {
        rb.isKinematic = true;
    }

    private void Unfreeze()
    {
        if (rb != null)
        {
            rb.isKinematic = false;
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.constraints = RigidbodyConstraints.None;
        }
    }

    private void ReturnToCurrentPlatform()
    {
        if (platformController == null) return;

        Platform currentPlatform = platformController.GetCurrentPlatform();
        if (currentPlatform != null && currentPlatform.GetSphereStartPosition() != null)
        {
            Vector3 platformStartPos = currentPlatform.GetSphereStartPosition().position;
            
            // Сбрасываем физику и перемещаем сферу
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.position = platformStartPos;
            rb.rotation = Quaternion.identity;
            Physics.SyncTransforms();
        }
    }
}
