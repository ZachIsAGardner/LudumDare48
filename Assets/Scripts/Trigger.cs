using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trigger : MonoBehaviour
{
    public Action<Collider> OnEnter;
    public Action<Collider> OnStay;
    public Action<Collider> OnExit;

    private void OnTriggerEnter(Collider other)
    {
        if (OnEnter != null)
            OnEnter(other);
    }

    private void OnTriggerStay(Collider other)
    {
        if (OnStay != null)
            OnStay(other);
    }

    private void OnTriggerExit(Collider other)
    {
        if (OnExit != null)
            OnExit(other);
    }
}
