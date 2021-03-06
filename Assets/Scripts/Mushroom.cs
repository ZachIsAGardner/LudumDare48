using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mushroom : MonoBehaviour
{
    Liver liver;
    Rigidbody rigidbody;
    Animator animator;

    float shootTime = 0;
    float shootDuration = 2f;
    int animationState = 1;

    public bool Diagonol = false;
    public GameObject hpTarget;
    GameObject healthUi;
    Timer healthUiTimer;

    void Start()
    {
        liver = GetComponent<Liver>();
        rigidbody = GetComponent<Rigidbody>();
        animator = GetComponentInChildren<Animator>();

        healthUi = Instantiate(Prefabs.Get("EnemyHP"), GameObject.FindGameObjectWithTag("Canvas").transform);
        healthUi.GetComponent<UiFollow>().target = hpTarget.transform;
        healthUi.SetActive(false);
        liver.SetUi(healthUi.GetComponentInChildren<HpBar>().GetComponent<RectTransform>());
    }

    void Update()
    {
        animator.SetInteger("State", animationState);

        if (animationState == 1)
        {
            shootTime -= Time.deltaTime;

            if (shootTime <= 0)
            {
                shootTime = shootDuration;
                animationState = 2;
            }
        }

        if (liver != null && liver.Health <= 0)
        {
            Instantiate(Prefabs.Get("HitEffect"), transform.transform.position, Quaternion.identity);
            if (healthUi != null) Destroy(healthUi);
            Destroy(gameObject);
        }
    }

    public void Shoot()
    {
        shootTime = shootDuration;

        List<Vector3> directions = !Diagonol ? new List<Vector3>()
        {
            new Vector3(1, 0, 0),
            new Vector3(-1, 0, 0),
            new Vector3(0, 0, 1),
            new Vector3(0, 0, -1),
        } : new List<Vector3>()
        {
            new Vector3(1, 0, 1),
            new Vector3(1, 0, -1),
            new Vector3(-1, 0, 1),
            new Vector3(-1, 0, -1),
        };

        Sounds.Play("Cough", transform.position);

        foreach (Vector3 direction in directions)
        {
            Rigidbody instance = Instantiate(Prefabs.Get("Bullet"), transform.position, Quaternion.identity).GetComponent<Rigidbody>();
            instance.velocity = direction * 10;
        }
    }

    public void HurtEnd()
    {
        animationState = 1;
    }

    public void AttackEnd()
    {
        animationState = 1;
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
            else
            {
                IHurter hurter = other.GetComponent<IHurter>();
                if (hurter != null)
                {
                    hurter.LandedHit(gameObject);
                }
            }
            animationState = 3;

            liver.TakeDamage(1);
            Instantiate(Prefabs.Get("HitEffect"), transform.transform.position, Quaternion.identity);
            Vector3 direction = other.transform.position - transform.position;
            direction.Normalize();
            rigidbody.AddForce(-direction * 450);

            if (healthUi != null)
            {
                healthUi.SetActive(true);
                if (healthUiTimer != null) Destroy(healthUiTimer.gameObject);
                healthUiTimer = Timer.Create(1f, () =>
                {
                    if (healthUi != null)
                        healthUi.SetActive(false);
                });
            }
        }
    }
}
