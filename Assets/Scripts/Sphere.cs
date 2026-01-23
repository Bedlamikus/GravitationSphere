using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sphere : MonoBehaviour
{
    [SerializeField] private int defaultMask;
    [SerializeField] private int holeMask;
    [SerializeField] private float timeToFreeze = 10f;
    [SerializeField] private float speed = 5f;
    [SerializeField] private float torqueForce = 1f;
    [SerializeField] private FloatingJoystick joystick;

    private bool isFrozen = false;
    private float timer = 0f;
    private Vector3 startPoint;

    private void Start()
    {
        startPoint = transform.position;
        StartCoroutine(FreezeRoutine());
        StartCoroutine(SetFrozen());
        GlobalEvents.StartLevel.AddListener(StartLevel);
    }

    private void OnCollisionEnter(Collision collision)
    {
        isFrozen = false;
        timer = 0f;
    }

    private void OnCollisionExit(Collision collision)
    {
        isFrozen = false;
        timer = 0f;
    }

    private void OnCollisionStay(Collision collision)
    {
        isFrozen = false;
        timer = 0f;
    }

    public void SetHoleMask()
    {
        gameObject.layer = holeMask;
    }

    public void SetDefaultMask()
    {
        gameObject.layer = defaultMask;
    }

    private IEnumerator FreezeRoutine()
    {
        while (timer < timeToFreeze)
        {
            timer += Time.deltaTime;
            yield return null;
        }
        
        if (isFrozen)
        {
            var rb = GetComponent<Rigidbody>();
            rb.Sleep();
            yield return null;
            rb.constraints = RigidbodyConstraints.FreezeAll;
            rb.velocity = Vector3.zero;
            rb.isKinematic = true;

            GlobalEvents.RestartLevel.Invoke();
            //rb.constraints = RigidbodyConstraints.None;
        }
    }

    private void StartLevel()
    {
        StartCoroutine(StartAganeRoutine());
    }

    private IEnumerator StartAganeRoutine()
    {
        var rb = GetComponent<Rigidbody>();
        rb.isKinematic = false;
        rb.position = startPoint;
        rb.rotation = Quaternion.identity;

        // —брос скоростей
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        
        Physics.SyncTransforms(); 
        
        yield return null;
        rb.constraints = RigidbodyConstraints.None;
        
        timer = 0;

        isFrozen = false;
        StartCoroutine(FreezeRoutine());
    }

    private IEnumerator SetFrozen()
    {
        while (true)
        {
            isFrozen = true;
            yield return new WaitForSeconds(1f);
        }
    }
}
