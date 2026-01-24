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
    [Header("Прыжок")]
    [SerializeField] private float jumpForce = 8f;
    [SerializeField] private float groundCheckDistance = 0.6f;
    [Header("Управление в воздухе")]
    [SerializeField] private float airControlForce = 3f;

    private Rigidbody rb;
    private Vector3 startPoint;
    private float referenceZ;

    private void Start()
    {
        startPoint = transform.position;
        referenceZ = startPoint.z;

        rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.maxAngularVelocity = speed;
            rb.constraints = RigidbodyConstraints.FreezePositionZ;
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

        bool grounded = IsGrounded();
        if (!grounded && airControlForce > 0f && (Mathf.Abs(verticalInput) > 0.01f || Mathf.Abs(horizontalInput) > 0.01f))
        {
            Vector3 airForce = new Vector3(horizontalInput * airControlForce, verticalInput * airControlForce, 0f);
            rb.AddForce(airForce, ForceMode.Force);
        }

        ClampZToReference();
    }

    private void ClampZToReference()
    {
        if (rb != null)
        {
            Vector3 p = rb.position;
            if (Mathf.Abs(p.z - referenceZ) > 0.0001f)
            {
                p.z = referenceZ;
                rb.position = p;
                Physics.SyncTransforms();
            }
        }
        else
        {
            Vector3 p = transform.position;
            p.z = referenceZ;
            transform.position = p;
        }
    }

    public void MoveToPosition(Vector3 newPosition)
    {
        referenceZ = newPosition.z;
        if (rb != null)
        {
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.position = newPosition;
            Physics.SyncTransforms();
        }
        else
        {
            transform.position = newPosition;
        }
    }

    public void Freeze()
    {
        rb.isKinematic = true;
    }

    public void Jump()
    {
        if (rb == null || rb.isKinematic) return;
        if (!IsGrounded()) return;

        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
    }

    private bool IsGrounded()
    {
        return Physics.Raycast(transform.position, Vector3.down, groundCheckDistance);
    }

    private void Unfreeze()
    {
        if (rb != null)
        {
            rb.isKinematic = false;
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.constraints = RigidbodyConstraints.FreezePositionZ;
        }
    }

    private void ReturnToCurrentPlatform()
    {
        if (platformController == null) return;

        Platform currentPlatform = platformController.GetCurrentPlatform();
        if (currentPlatform != null && currentPlatform.GetSphereStartPosition() != null)
        {
            Vector3 platformStartPos = currentPlatform.GetSphereStartPosition().position;
            referenceZ = platformStartPos.z;

            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.position = platformStartPos;
            rb.rotation = Quaternion.identity;
            Physics.SyncTransforms();
        }
    }
}
