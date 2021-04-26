using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoreLoader : MonoBehaviour
{
    public GameObject core;
    static GameObject instance;

    void Awake()
    {
        if (instance == null)
        {
            instance = Instantiate(core);
        }
    }
}
