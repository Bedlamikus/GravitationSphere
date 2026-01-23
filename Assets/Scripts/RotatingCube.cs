using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotatingCube : MonoBehaviour
{
    [SerializeField] private Rigidbody cubeRigidbody;
    [SerializeField] private float maxAngleX = 10f;
    [SerializeField] private float minAngleX = -10f;
    [SerializeField] private float maxAngleZ = 10f;
    [SerializeField] private float minAngleZ = -10f;
    [SerializeField] private float torqueForce = 10f;
    [SerializeField] private FloatingJoystick joystick;
    [SerializeField] private Sphere sphere;
    [SerializeField] private bool sphereTarget;

    private float currentAngleX;
    private float currentAngleZ;
    private Vector3 startPosition;

    private void Start()
    {
        startPosition = transform.position;
    }

    private void AddTorqueX(float sign)
    {
        currentAngleX = transform.eulerAngles.x;
        if (currentAngleX > 180)
        {
            currentAngleX -= 360;
        }

        if (sign > 0 && currentAngleX < maxAngleX)
        {
            cubeRigidbody.AddTorque(transform.right * torqueForce * sign, ForceMode.Acceleration);
        }

        if (sign < 0 && currentAngleX > minAngleX)
        {
            cubeRigidbody.AddTorque(transform.right * torqueForce * sign, ForceMode.Acceleration);
        }
    }

    private void AddTorqueZ(float sign)
    {
        currentAngleZ = transform.eulerAngles.z;
        if (currentAngleZ > 180)
        {
            currentAngleZ -= 360;
        }

        if (sign < 0 && currentAngleZ > minAngleZ)
        {
            cubeRigidbody.AddTorque(transform.forward * torqueForce * sign, ForceMode.Acceleration);
        }

        if (sign > 0 && currentAngleZ < maxAngleZ)
        {
            cubeRigidbody.AddTorque(transform.forward * torqueForce * sign, ForceMode.Acceleration);
        }

        var localEulers = transform.localEulerAngles;
        localEulers.y = 0f;
        transform.localEulerAngles = localEulers;
    }

    private void Update()
    {
        if (cubeRigidbody == null) return;

        //if (joystick.Vertical > 0)
        //{
        //    AddTorqueX(1);
        //}
        //else if (joystick.Vertical < 0)
        //{
        //    AddTorqueX(-1);
        //}

        if (joystick.Horizontal > 0) {
            AddTorqueZ(-1);
        }
        else if (joystick.Horizontal < 0)
        {
            AddTorqueZ(1);
        }

        transform.position = startPosition;
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    public void Show()
    {
        gameObject.SetActive(true);
    }

    public Transform GetTarget()
    {
        if (sphereTarget)
        {
            return sphere.transform;
        }
        return transform;
    }
}
