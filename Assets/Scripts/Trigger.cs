using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trigger : MonoBehaviour
{
    public Action<Collider> OnStay;
    private Liver liver;

    void Start()
    {
        liver = GetComponentInParent<Liver>();
    }

    private void OnTriggerStay(Collider other)
    {
        OnStay(other);
    }
}
