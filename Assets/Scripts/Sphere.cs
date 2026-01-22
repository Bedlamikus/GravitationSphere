using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sphere : MonoBehaviour
{
    [SerializeField] private int defaultMask;
    [SerializeField] private int holeMask;
    [SerializeField] private float timeToFreeze = 10f;

    private bool isFrozen = false;
    private float timer = 0f;
    private Vector3 startPoint;

    private void Start()
    {
        startPoint = transform.position;
        StartCoroutine(FreezeRoutine());
        StartCoroutine(SetFrozen());
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
            rb.constraints = RigidbodyConstraints.FreezeAll;
            transform.position = startPoint;
            transform.rotation = Quaternion.identity;
            
            GlobalEvents.RestartLevel.Invoke();
            rb.constraints = RigidbodyConstraints.None;
        }
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
