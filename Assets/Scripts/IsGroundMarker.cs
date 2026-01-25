using UnityEngine;

/// <summary>
/// Маркер для проверки земли. Размещается в точках проверки (например, снизу сферы).
/// Самостоятельно делает OverlapSphere в своей позиции — если что-то попало, считаем что на земле.
/// Радиус = 1 * scale (относительно lossyScale маркера).
/// </summary>
public class IsGroundMarker : MonoBehaviour
{
    private const float BaseRadius = 1f;
    private static readonly Collider[] OverlapBuffer = new Collider[16];

    /// <summary>
    /// true, если в сфере маркера есть чужой коллайдер (игнорируем свой root и его детей).
    /// </summary>
    public bool IsGround
    {
        get
        {
            Transform root = transform.root;
            float scale = Mathf.Max(transform.lossyScale.x, transform.lossyScale.y, transform.lossyScale.z);
            float radius = BaseRadius * scale;

            int count = Physics.OverlapSphereNonAlloc(
                transform.position,
                radius,
                OverlapBuffer
            );

            for (int i = 0; i < count; i++)
            {
                Collider c = OverlapBuffer[i];
                if (c.transform == root || c.transform.IsChildOf(root))
                    continue;
                return true;
            }

            return false;
        }
    }
}
