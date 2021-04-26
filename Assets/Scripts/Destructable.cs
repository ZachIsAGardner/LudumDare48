using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Destructable : MonoBehaviour
{
    Liver liver;
    Rigidbody rigidbody;

    void Start()
    {
        liver = GetComponent<Liver>();
        rigidbody = GetComponent<Rigidbody>();
    }

    void Update()
    {
        if (liver != null && liver.Health <= 0)
        {
            Instantiate(Prefabs.Get("HitEffect"), transform.transform.position, Quaternion.identity);
            Destroy(gameObject);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (liver == null || liver.IsInvincible) return;

        if (other.CompareTag("HurtEnvironment") || other.CompareTag("HurtPlayer"))
        {
            Player player = other.GetComponentInParent<Player>();
            if (player != null)
            {
                player.LandedHit(gameObject);
            }
            liver.TakeDamage(1);
            Instantiate(Prefabs.Get("HitEffect"), transform.transform.position, Quaternion.identity);
            Vector3 direction = other.transform.position - transform.position;
            direction.Normalize();
            rigidbody.AddForce(-direction * 450);
        }
    }
}
