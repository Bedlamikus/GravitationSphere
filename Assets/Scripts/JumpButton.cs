using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(UnityEngine.UI.Image))]
public class JumpButton : MonoBehaviour, IPointerDownHandler
{
    [SerializeField] private Sphere sphere;

    public void OnPointerDown(PointerEventData eventData)
    {
        if (sphere != null)
            sphere.Jump();
    }
}
