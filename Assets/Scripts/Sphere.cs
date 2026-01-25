using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Sphere : MonoBehaviour
{
    [SerializeField] private int defaultMask;
    [SerializeField] private int holeMask;
    [SerializeField] private float timeToFreeze = 10f;
    [SerializeField] private Animator anim;
    [SerializeField] private Transform visualModel;
    [SerializeField] private IsGroundMarker[] groundMarkers;
    [SerializeField] private TMP_Text speedText;
    [SerializeField] private float speedAnimThreshold = 0.25f;
    [SerializeField] private float rotationLerpSpeed = 10f;

    private bool isFrozen = false;
    private Quaternion currentVisualRotation;
    private Vector3 prevPosition;
    private float timer = 0f;
    private Vector3 startPoint;
    private Rigidbody rb;

    private void Start()
    {
        startPoint = transform.position;
        prevPosition = startPoint;
        rb = GetComponent<Rigidbody>();
        if (visualModel != null)
            currentVisualRotation = visualModel.rotation;
        StartCoroutine(FreezeRoutine());
        StartCoroutine(SetFrozen());
        GlobalEvents.StartLevel.AddListener(StartLevel);
    }

    private void Update()
    {
        float speed = rb != null ? rb.velocity.magnitude : 0f;

        if (anim == null) return;

        bool onGround = IsGround;
        int state;

        if (!onGround)
            state = 2;
        else if (speed > speedAnimThreshold)
            state = 1;
        else
            state = 0;

        anim.SetInteger("State", state);
    }

    private void LateUpdate()
    {
        float speed = rb != null ? rb.velocity.magnitude : 0f;

        if (visualModel == null || rb == null) return;

        Vector3 pos = rb.position;
        Vector3 delta = pos - prevPosition;
        delta.z = 0f;
        prevPosition = pos;

        if (speed < speedAnimThreshold) return;
        if (delta.sqrMagnitude <= 0f) return;

        Vector3 forward = new Vector3(delta.x, 0f, delta.y);
        forward.Normalize();
        Quaternion targetRot = Quaternion.LookRotation(forward, Vector3.up);
        float t = Mathf.Clamp01(rotationLerpSpeed * Time.deltaTime);
        currentVisualRotation = Quaternion.Slerp(currentVisualRotation, targetRot, t);
        visualModel.rotation = currentVisualRotation;
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
        prevPosition = startPoint;

        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        Physics.SyncTransforms();

        yield return null;
        rb.constraints = RigidbodyConstraints.FreezeRotation;
        
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

    private bool IsGround
    {
        get
        {
            if (groundMarkers == null || groundMarkers.Length == 0) return false;

            foreach (var marker in groundMarkers)
            {
                if (marker != null && marker.IsGround)
                    return true;
            }

            return false;
        }
    }
}
