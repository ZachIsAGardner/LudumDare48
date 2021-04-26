using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UiFollow : MonoBehaviour
{
    public Transform target;
    public Vector3 offset;

    void Start()
    {
        
    }

    void Update()
    {
        if (target == null) return;

        Vector3 point = Camera.main.WorldToScreenPoint(target.position) + offset;
        transform.position = point;
    }
}
