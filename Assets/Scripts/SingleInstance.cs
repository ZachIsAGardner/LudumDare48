using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SingleInstance<T> : MonoBehaviour
{
    public static T Instance; 

    protected virtual void Awake()
    {
        Instance = GetComponent<T>();
    }
}
