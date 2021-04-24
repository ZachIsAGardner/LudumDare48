using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Follow : MonoBehaviour
{
    public Transform follow;
    private Vector3 offset;

    // Start is called before the first frame update
    void Start()
    {
        offset = follow.position - transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = follow.position - offset;
    }
}
