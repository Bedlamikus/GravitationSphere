using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sphere : MonoBehaviour
{
    [SerializeField] private int defaultMask;
    [SerializeField] private int holeMask;

    public void SetHoleMask()
    {
        gameObject.layer = holeMask;
    }

    public void SetDefaultMask()
    {
        gameObject.layer = defaultMask;
    }
}
