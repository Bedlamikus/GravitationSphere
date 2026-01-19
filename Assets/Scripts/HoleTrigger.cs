using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HoleTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        var sphere = other.GetComponent<Sphere>();
        if (sphere == null) return;

        sphere.SetHoleMask();
    }

    private void OnTriggerExit(Collider other)
    {
        var sphere = other.GetComponent<Sphere>();
        if (sphere == null) return;

        sphere.SetDefaultMask();
    }
}
