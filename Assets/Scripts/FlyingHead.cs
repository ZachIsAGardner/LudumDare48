using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum FlyingHeadState
{
    Idle = 1,
    Follow = 2,
    Attack = 3,
    Hurt = 4
}

public enum FlyingHeadAnimationState
{
    Idle = 1,
    Follow = 2,
    Attack = 3,
    Hurt = 4
}

public class FlyingHead : MonoBehaviour
{
    public float Speed = 10;

    public GameObject hpTarget;
    GameObject healthUi;
    Timer healthUiTimer;

    float attackCooldown = 0;
    float retreatTime = 0;

    Animator animator;
    Liver liver;
    Player player;
    Rigidbody rigidbody;

    FlyingHeadAnimationState animationState = FlyingHeadAnimationState.Idle;
    FlyingHeadState state = FlyingHeadState.Idle;

    Action start;
    Action update;
    Action exit;

    // IDLE

    void IdleStart()
    {

    }

    void IdleUpdate()
    {
        animationState = FlyingHeadAnimationState.Idle;

        Vector3 distance = transform.position - player.transform.position;

        distance = new Vector3(Math.Abs(distance.x), Math.Abs(distance.y), Math.Abs(distance.z));

        if (distance.x <= 12 && distance.z <= 12)
        {
            ChangeState(FlyingHeadState.Follow);
        }
    }

    void IdleExit()
    {

    }

    // FOLLOW

    void FollowStart()
    {

    }

    void FollowUpdate()
    {
        attackCooldown -= Time.deltaTime;
        retreatTime -= Time.deltaTime;

        animationState = FlyingHeadAnimationState.Follow;

        Vector3 distance = player.transform.position - transform.position;
        Vector3 normalDistance = distance;
        normalDistance.Normalize();
        Vector3 destination = (normalDistance * Speed);

        Vector3 absDistance = new Vector3(Math.Abs(distance.x), Math.Abs(distance.y), Math.Abs(distance.z));

        retreatTime = 0;

        if (absDistance.x < 5 && absDistance.z < 5)
        {
            if (attackCooldown <= 0)
            {
                ChangeState(FlyingHeadState.Attack);
            }

            retreatTime = new List<float>() { 1f, 2f, 4f }.Random();
        }

        transform.rotation = Quaternion.LookRotation(new Vector3(normalDistance.x, 0, normalDistance.z));

        if (retreatTime > 0) destination = -destination * 0.5f;

        rigidbody.velocity = new Vector3(
            rigidbody.velocity.x.MoveOverTime(destination.x, 0.1f),
            0,
            rigidbody.velocity.z.MoveOverTime(destination.z, 0.1f)
        );
    }

    void FollowExit()
    {

    }

    // ATTACK

    void AttackStart()
    {
        animationState = FlyingHeadAnimationState.Attack;
        rigidbody.velocity /= 10;
    }

    void AttackUpdate()
    {
        Vector3 distance = player.transform.position - transform.position;
        distance.Normalize();
        Vector3 destination = (distance * (Speed / 2f));
        rigidbody.velocity = new Vector3(
            rigidbody.velocity.x.MoveOverTime(destination.x, 0.1f),
            0,
            rigidbody.velocity.z.MoveOverTime(destination.z, 0.1f)
        );

        transform.rotation = Quaternion.LookRotation(new Vector3(distance.x, 0, distance.z));
    }

    void AttackExit()
    {

    }

    // HURT

    void HurtStart()
    {
        animationState = FlyingHeadAnimationState.Hurt;
    }

    void HurtUpdate()
    {

    }

    void HurtExit()
    {

    }

    // ---

    void Start()
    {
        liver = GetComponent<Liver>();
        animator = GetComponent<Animator>();
        rigidbody = GetComponent<Rigidbody>();

        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();

        healthUi = Instantiate(Prefabs.Get("EnemyHP"), GameObject.FindGameObjectWithTag("Canvas").transform);
        healthUi.GetComponent<UiFollow>().target = hpTarget.transform;
        healthUi.SetActive(false);
        liver.SetUi(healthUi.GetComponentInChildren<HpBar>().GetComponent<RectTransform>());


        start = IdleStart;
        update = IdleUpdate;
        exit = IdleExit;
    }

    // Update is called once per frame
    void Update()
    {
        animator.SetInteger("State", (int)animationState);
        update();

        if (liver != null && liver.Health <= 0)
        {
            Instantiate(Prefabs.Get("HitEffect"), transform.transform.position, Quaternion.identity);
            if (healthUi != null) Destroy(healthUi);
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
            else
            {
                IHurter hurter = other.GetComponent<IHurter>();
                if (hurter != null)
                {
                    hurter.LandedHit(gameObject);
                }
            }

            ChangeState(FlyingHeadState.Hurt);

            liver.TakeDamage(1);
            Instantiate(Prefabs.Get("HitEffect"), transform.transform.position, Quaternion.identity);
            Vector3 direction = other.transform.position - transform.position;
            direction.y = 0;
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

    void ChangeState(FlyingHeadState newState)
    {
        exit();

        state = newState;

        switch (newState)
        {
            default:
            case FlyingHeadState.Idle:
                start = IdleStart;
                update = IdleUpdate;
                exit = IdleExit;
                break;

            case FlyingHeadState.Attack:
                start = AttackStart;
                update = AttackUpdate;
                exit = AttackExit;
                break;

            case FlyingHeadState.Follow:
                start = FollowStart;
                update = FollowUpdate;
                exit = FollowExit;
                break;

            case FlyingHeadState.Hurt:
                start = HurtStart;
                update = HurtUpdate;
                exit = HurtExit;
                break;
        }

        start();
    }

    // Animation Events

    void AttackEnd()
    {
        attackCooldown = new List<float>() { 2f, 1f, 5f }.Random();
        ChangeState(FlyingHeadState.Follow);
    }

    void Barf()
    {
        Vector3 direction = player.transform.position - transform.position;
        direction.Normalize();
        Rigidbody instance = Instantiate(Prefabs.Get("Bullet"), transform.position, Quaternion.identity).GetComponent<Rigidbody>();
        instance.velocity = direction * 10;
        Sounds.Play("Cough", transform.position);
    }

    void HurtEnd()
    {
        ChangeState(FlyingHeadState.Idle);
    }
}
