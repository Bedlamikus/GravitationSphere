using UnityEngine;

public class RotatingTransform : MonoBehaviour
{
    [SerializeField] private float speed = 1.0f;
    [SerializeField] private Vector3 axis;

    private void Update()
    {
        transform.Rotate(axis * speed * Time.deltaTime);
    }
}
