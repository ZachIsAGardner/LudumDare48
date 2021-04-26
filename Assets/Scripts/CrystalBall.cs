using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrystalBall : MonoBehaviour
{
    Liver liver;
    Rigidbody rigidbody;
    Animator animator;

    public List<Transform> positions;
    Transform position;

    Boss scene;

    // Start is called before the first frame update
    void Start()
    {
        liver = GetComponent<Liver>();
        rigidbody = GetComponent<Rigidbody>();
        animator = GetComponentInChildren<Animator>();
        scene = FindObjectOfType<Boss>();

        position = positions.Random();
    }

    // Update is called once per frame
    void Update()
    {
        if (liver != null && liver.Health <= 0)
        {
            Instantiate(Prefabs.Get("HitEffect"), transform.transform.position, Quaternion.identity);
            scene.victory = true;
            Destroy(gameObject);
        }

    }

    void FixedUpdate()
    {
        var diff = transform.position - position.position;
        diff = new Vector3(Mathf.Abs(diff.x), Mathf.Abs(diff.y), Mathf.Abs(diff.z));
        if (diff.x < 2 && diff.y < 2 && diff.z < 2)
        {
            position = positions.Random();
        }

        transform.position = Vector3.MoveTowards(transform.position, position.position, 0.1f);
    }

    private void OnTriggerStay(Collider other)
    {
        if (liver == null || liver.IsInvincible) return;

        if (other.CompareTag("HurtEnvironment") || other.CompareTag("HurtPlayer"))
        {
            scene.fight = true;
            
            Player player = other.GetComponentInParent<Player>();
            if (player != null)
            {
                player.LandedHit(gameObject);
            }
            else
            {
                IHurter hurter = other.GetComponent<IHurter>();
                if (hurter != null)
                {
                    hurter.LandedHit(gameObject);
                }
            }

            animator.SetInteger("State", 2);

            liver.TakeDamage(1);
            Instantiate(Prefabs.Get("HitEffect"), transform.transform.position, Quaternion.identity);
            Vector3 direction = other.transform.position - transform.position;
            direction.Normalize();
        }
    }

    void HurtEnd()
    {
        animator.SetInteger("State", 1);
    }
}
