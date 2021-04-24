using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    private Rigidbody rigidbody;
    private CapsuleCollider collider;
    private Transform camera;
    private float startGravity;
    private int slopeCount = 0;
    private Vector2 angle;

    public float Speed = 10;
    public float Acceleration = 0.001f;
    public float Gravity = 1f;

    // Start is called before the first frame update
    void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
        collider = GetComponent<CapsuleCollider>();
        camera = Camera.main.transform;
        startGravity = Gravity;
    }

    // Update is called once per frame
    void Update()
    {
        collider.material.dynamicFriction = 1;
        Gravity = startGravity;

        Vector2 input = new Vector2();
        input.x = Input.GetAxisRaw("Horizontal");
        input.y = Input.GetAxisRaw("Vertical");
        input.Normalize();

        Vector2 forward = camera.forward;
        Vector2 right = camera.transform.right;

        forward.y = 0f;
        right.y = 0f;
        forward.Normalize();
        right.Normalize();

        if (!IsGrounded())
        {
            input = Vector3.zero;
        }

        Vector3 movementX = Camera.main.transform.right * input.x;
        Vector3 movementZ = Camera.main.transform.forward * input.y;
        Vector3 movement = movementX + movementZ;

        Vector3 look = Vector3.RotateTowards(transform.forward, transform.position + new Vector3(input.x, 0, input.y), 1 * Time.deltaTime, 0.0f);
        
        rigidbody.velocity = new Vector3(
            rigidbody.velocity.x.MoveOverTime(movement.x * Speed, Acceleration),
            rigidbody.velocity.y - (Gravity * Time.deltaTime),
            rigidbody.velocity.z.MoveOverTime(movement.z * Speed, Acceleration)
        );

        if (input != Vector2.zero) 
        {
            angle = input;
        }

        transform.rotation = Quaternion.LookRotation(new Vector3(-angle.x, 0, -angle.y));
    }

    private bool IsGrounded()
    {
        // return Physics.Raycast(transform.position, -Vector3.up, collider.bounds.extents.y + 0.5f);
        bool didHit = Physics.SphereCast(transform.position + new Vector3(0, collider.height / 2f, 0), collider.radius * 1.1f, Vector3.down, out RaycastHit hit);

        if (Math.Abs(hit.normal.x) > 0.5f || Math.Abs(hit.normal.z) > 0.5f)
        {
            slopeCount++;

            if (slopeCount > 30)
            {
                print("SLOPE");
                collider.material.dynamicFriction = 0;
                Gravity = startGravity * 5;
                return false;
            }
        }
        else
        {
            slopeCount = 0;
        }

        return didHit;
    }
}
