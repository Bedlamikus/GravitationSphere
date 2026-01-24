using UnityEngine;

public class Platform : MonoBehaviour
{
    [SerializeField] private Transform sphereStartPosition;

    public void Hide()
    {
        gameObject.SetActive(false);
    }

    public void Show()
    {
        gameObject.SetActive(true);
    }

    public Transform GetSphereStartPosition()
    {
        return sphereStartPosition;
    }
}
