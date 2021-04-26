using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Follow : MonoBehaviour
{
    public Transform follow;
    public Vector3 offset;

    // Start is called before the first frame update
    void Start()
    {
        // offset = follow.position - transform.position;
    }

    // Update is called once per frame
    void Update()
    {

    }

    void FixedUpdate()
    {
        if (follow == null) return;
        
        var destination = follow.position + offset;
        transform.position = new Vector3(
            transform.position.x.MoveOverTime(destination.x, 0.0001f),
            transform.position.y.MoveOverTime(destination.y, 0.0001f),
            transform.position.z.MoveOverTime(destination.z, 0.0001f)
        );
    }
}
