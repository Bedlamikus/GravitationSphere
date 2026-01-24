using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obstacle : MonoBehaviour
{
    [SerializeField] private Vector3 direction;
    [SerializeField] private float waitingBefore = 5f;
    [SerializeField] private float waitingBetweenMovements = 1f;
    [SerializeField] private AnimationCurve movementCurve;
    [SerializeField] private float speed;

    private Vector3 startPoint;
    private Rigidbody rb;
    private Platform platform;
    private Coroutine movementCoroutine;

    private void Awake()
    {
        startPoint = transform.position;
        rb = GetComponent<Rigidbody>();
        platform = GetComponentInParent<Platform>();
    }

    private void OnEnable()
    {
        movementCoroutine = StartCoroutine(MovementRoutine());
    }

    private void OnDisable()
    {
        if (movementCoroutine != null)
        {
            StopCoroutine(movementCoroutine);
            movementCoroutine = null;
        }
    }

    private bool IsVisible()
    {
        if (platform == null) return gameObject.activeInHierarchy;
        return platform.gameObject.activeInHierarchy && gameObject.activeInHierarchy;
    }

    private IEnumerator WaitWhileHidden()
    {
        while (!IsVisible())
            yield return null;
    }

    private IEnumerator MovementRoutine()
    {
        Vector3 targetPoint = startPoint + direction;

        yield return new WaitForSeconds(waitingBefore);

        while (true)
        {
            yield return StartCoroutine(WaitWhileHidden());

            yield return StartCoroutine(MoveToTarget(targetPoint));

            yield return StartCoroutine(WaitWhileHidden());
            yield return new WaitForSeconds(waitingBetweenMovements);

            yield return StartCoroutine(MoveToTarget(startPoint));

            yield return StartCoroutine(WaitWhileHidden());
            yield return new WaitForSeconds(waitingBetweenMovements);
        }
    }

    private IEnumerator MoveToTarget(Vector3 target)
    {
        if (rb == null) yield break;

        Vector3 startPos = rb.position;
        float distance = Vector3.Distance(startPos, target);
        float duration = distance / speed;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            yield return StartCoroutine(WaitWhileHidden());

            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            float curveValue = movementCurve.Evaluate(t);

            Vector3 newPos = Vector3.Lerp(startPos, target, curveValue);
            rb.MovePosition(newPos);

            yield return null;
        }

        if (rb != null)
            rb.MovePosition(target);
    }
}
