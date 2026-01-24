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

    private void Start()
    {
        startPoint = transform.position;
        rb = GetComponent<Rigidbody>();
        StartCoroutine(MovementRoutine());
    }

    private IEnumerator MovementRoutine()
    {
        Vector3 targetPoint = startPoint + direction;

        yield return new WaitForSeconds(waitingBefore);

        while (true)
        {
            yield return StartCoroutine(MoveToTarget(targetPoint));
            
            yield return new WaitForSeconds(waitingBetweenMovements);

            yield return StartCoroutine(MoveToTarget(startPoint));

            yield return new WaitForSeconds(waitingBetweenMovements);
        }
    }

    private IEnumerator MoveToTarget(Vector3 target)
    {
        Vector3 startPos = rb.position;
        float distance = Vector3.Distance(startPos, target);
        float duration = distance / speed;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            float curveValue = movementCurve.Evaluate(t);
            
            // Используем MovePosition для физического перемещения
            Vector3 newPos = Vector3.Lerp(startPos, target, curveValue);
            rb.MovePosition(newPos);
            
            yield return null;
        }

        rb.MovePosition(target);
    }
}
