using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public enum PlayerAnimationState
{
    Idle = 1,
    Walk = 2,
    Attack = 3,
    Attack2 = 4,
    Dodge = 5,
    Splat = 6,
    Hurt = 7,
    Fall = 8,
    Heal = 9,
    NoPotion = 10
}

public enum PlayerState
{
    Normal,
    Attack,
    Dodge,
    Hurt,
    Fall,
    Heal
}

public class Player : MonoBehaviour, IHurter
{
    private Rigidbody rigidbody;
    private CapsuleCollider collider;
    private Transform camera;
    private Animator animator;
    private Liver liver;
    private Trigger trigger;

    private float startGravity;
    private int slopeCount = 0;
    private Vector2 angle;
    private PlayerAnimationState animationState = 0;
    private PlayerState state = 0;
    private bool canEndAttack = false;
    private bool canGetUp = false;
    private bool canGoToNextAttack = false;
    private bool queueAttack = false;
    private Vector2 input = Vector2.zero;
    private Vector2 lastInput = Vector2.zero;
    private float dodgeTime = 0;
    private bool isGrounded = false;
    private bool isDodgeInvincible = false;
    private float potionCount = 3;

    public float Speed = 10;
    public float Acceleration = 0.001f;
    public float Gravity = 1f;
    public float DodgeHeight = 8;
    public float DodgeSpeed = 12;
    public TextMeshProUGUI potionCountUi;

    private Action start;
    private Action update;
    private Action exit;

    // NORMAL

    void NormalStart()
    {
        canEndAttack = false;
        canGoToNextAttack = false;
    }

    void NormalUpdate()
    {

        input = Vector2.zero;

        input.x = Input.GetAxisRaw("Horizontal");
        input.y = Input.GetAxisRaw("Vertical");
        input.Normalize();

        if (!isGrounded)
        {
            input = Vector2.zero;
        }

        // ---

        if (input != Vector2.zero)
        {
            animationState = PlayerAnimationState.Walk;
            lastInput = input;
        }
        else
        {
            animationState = PlayerAnimationState.Idle;
        }

        if (Input.GetKeyDown(KeyCode.X))
        {
            ChangeState(PlayerState.Dodge);
        }

        if (!isGrounded)
        {
            ChangeState(PlayerState.Fall);
            return;
        }

        if (Input.GetKeyDown(KeyCode.Z))
        {
            ChangeState(PlayerState.Attack);
            return;
        }

        if (Input.GetKeyDown(KeyCode.C))
        {
            ChangeState(PlayerState.Heal);
            return;
        }

        // ---

        Vector3 movementX = Camera.main.transform.right * input.x;
        Vector3 movementZ = Camera.main.transform.forward * input.y;
        Vector3 movement = movementX + movementZ;

        // Vector3 movement = new Vector3(input.x, 0, input.y);

        if (movement != Vector3.zero)
            transform.rotation = Quaternion.LookRotation(new Vector3(movement.x, 0, movement.z));

        rigidbody.velocity = new Vector3(
            rigidbody.velocity.x.MoveOverTime(movement.x * Speed, Acceleration),
            rigidbody.velocity.y - (Gravity * Time.deltaTime),
            rigidbody.velocity.z.MoveOverTime(movement.z * Speed, Acceleration)
        );
    }

    void NormalExit()
    {

    }

    // ATTACK

    void AttackStart()
    {
        animationState = PlayerAnimationState.Attack;
    }

    void AttackUpdate()
    {

        input = Vector2.zero;

        // ---

        if (canEndAttack || canGoToNextAttack)
        {
            if (Input.GetKeyDown(KeyCode.Z) || queueAttack)
            {
                animationState = animationState == PlayerAnimationState.Attack ? PlayerAnimationState.Attack2 : PlayerAnimationState.Attack;
                canEndAttack = false;
                canGoToNextAttack = false;
                queueAttack = false;
            }
            else if (canEndAttack)
            {
                input.x = Input.GetAxisRaw("Horizontal");
                input.y = Input.GetAxisRaw("Vertical");
                input.Normalize();

                if (!isGrounded)
                {
                    input = Vector2.zero;
                }

                if (input != Vector2.zero)
                {
                    ChangeState(PlayerState.Normal);
                }
            }
        }
        else
        {
            if (Input.GetKeyDown(KeyCode.Z))
            {
                queueAttack = true;
            }
        }

        // ---

        Vector3 movementX = Camera.main.transform.right * input.x;
        Vector3 movementZ = Camera.main.transform.forward * input.y;
        Vector3 movement = movementX + movementZ;

        if (movement != Vector3.zero)
            transform.rotation = Quaternion.LookRotation(new Vector3(movement.x, 0, movement.z));

        rigidbody.velocity = new Vector3(
            rigidbody.velocity.x.MoveOverTime(movement.x * Speed, Acceleration),
            rigidbody.velocity.y - (Gravity * Time.deltaTime),
            rigidbody.velocity.z.MoveOverTime(movement.z * Speed, Acceleration)
        );
    }

    void AttackExit()
    {
        canGoToNextAttack = false;
        queueAttack = false;
        animator.speed = 1;
    }

    void DodgeStart()
    {
        dodgeTime = 0;
        animationState = PlayerAnimationState.Dodge;
        Vector3 movementX = Camera.main.transform.right * lastInput.x;
        Vector3 movementZ = Camera.main.transform.forward * lastInput.y;
        Vector3 movement = movementX + movementZ;
        rigidbody.velocity = new Vector3(movement.x * DodgeSpeed, DodgeHeight, movement.z * DodgeSpeed);
    }

    void DodgeUpdate()
    {
        dodgeTime += Time.deltaTime;

        input = Vector2.zero;

        // ---

        // ---

        if (isGrounded && dodgeTime >= 0.25f)
        {
            rigidbody.velocity = new Vector3(
                0,
                rigidbody.velocity.y - (Gravity * Time.deltaTime),
                0
            );
            animationState = PlayerAnimationState.Splat;
        }

        if (canGetUp)
        {
            input.x = Input.GetAxisRaw("Horizontal");
            input.y = Input.GetAxisRaw("Vertical");
            input.Normalize();

            if (input != Vector2.zero)
            {
                ChangeState(PlayerState.Normal);
            }
        }

        rigidbody.velocity = new Vector3(
            rigidbody.velocity.x,
            rigidbody.velocity.y - (Gravity * Time.deltaTime),
            rigidbody.velocity.z
        );
    }

    void DodgeExit()
    {
        dodgeTime = 0;
        isDodgeInvincible = false;
        canGetUp = false;
    }

    // HURT

    void HurtStart()
    {

    }

    void HurtUpdate()
    {
        rigidbody.velocity = Vector3.zero;
        animationState = PlayerAnimationState.Hurt;
    }

    void HurtExit()
    {

    }

    // FALL

    void FallStart()
    {

    }

    void FallUpdate()
    {
        animationState = PlayerAnimationState.Fall;
        if (isGrounded)
        {
            ChangeState(PlayerState.Normal);
        }

        Vector3 movementX = Camera.main.transform.right * input.x;
        Vector3 movementZ = Camera.main.transform.forward * input.y;
        Vector3 movement = movementX + movementZ;

        if (movement != Vector3.zero)
            transform.rotation = Quaternion.LookRotation(new Vector3(movement.x, 0, movement.z));

        rigidbody.velocity = new Vector3(
            rigidbody.velocity.x.MoveOverTime(0, Acceleration * 2f),
            rigidbody.velocity.y - (Gravity * Time.deltaTime),
            rigidbody.velocity.z.MoveOverTime(0, Acceleration * 2f)
        );
    }

    void FallExit()
    {

    }

    // Heal

    void HealStart()
    {
        rigidbody.velocity = Vector3.zero;
        if (potionCount > 0)
            animationState = PlayerAnimationState.Heal;
        else
            animationState = PlayerAnimationState.NoPotion;
    }

    void HealUpdate()
    {
    }

    void HealExit()
    {

    }

    // ---

    void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
        collider = GetComponent<CapsuleCollider>();
        animator = GetComponent<Animator>();
        liver = GetComponent<Liver>();
        trigger = GetComponentInChildren<Trigger>();
        trigger.OnStay = OnChildTriggerStay;
        camera = Camera.main.transform;
        startGravity = Gravity;

        start = NormalStart;
        update = NormalUpdate;
        exit = NormalExit;
    }

    void FixedUpdate()
    {
        isGrounded = false;
    }

    void Update()
    {
        collider.material.dynamicFriction = 1;
        Gravity = startGravity;
        animator.SetInteger("State", (int)animationState);

        update();

        if (potionCountUi != null)
        {
            potionCountUi.text = $"Potions: {potionCount}";
        }

        if (liver.Health <= 0)
        {
            Instantiate(Prefabs.Instance.HitEffect, transform.transform.position, Quaternion.identity);
            Destroy(gameObject);
        }
    }

    // ---

    public void LandedHit(GameObject other)
    {
        animator.speed = 0;
        Timer.Create(0.1f, () => {
            animator.speed = 1f;
        });
    }

    private void ChangeState(PlayerState newState)
    {
        exit();

        state = newState;

        switch (newState)
        {
            default:
            case PlayerState.Normal:
                start = NormalStart;
                update = NormalUpdate;
                exit = NormalExit;
                break;

            case PlayerState.Attack:
                start = AttackStart;
                update = AttackUpdate;
                exit = AttackExit;
                break;

            case PlayerState.Dodge:
                start = DodgeStart;
                update = DodgeUpdate;
                exit = DodgeExit;
                break;

            case PlayerState.Hurt:
                start = HurtStart;
                update = HurtUpdate;
                exit = HurtExit;
                break;

            case PlayerState.Fall:
                start = FallStart;
                update = FallUpdate;
                exit = FallExit;
                break;

            case PlayerState.Heal:
                start = HealStart;
                update = HealUpdate;
                exit = HealExit;
                break;
        }

        start();
    }

    // private bool IsGrounded()
    // {
    //     return true;
    //     // // return Physics.Raycast(transform.position, -Vector3.up, collider.bounds.extents.y + 0.5f);
    //     // bool didHit = Physics.SphereCast(
    //     //     origin: transform.position + new Vector3(0, collider.height / 2f, 0), 
    //     //     radius: collider.radius * 1.1f, 
    //     //     direction: Vector3.down, 
    //     //     hitInfo: out RaycastHit hit,
    //     //     layerMask: LayerMask.GetMask("Collision"),
    //     //     maxDistance: 2
    //     // );

    //     // if (Math.Abs(hit.normal.x) > 0.5f || Math.Abs(hit.normal.z) > 0.5f)
    //     // {
    //     //     slopeCount++;

    //     //     if (slopeCount > 30)
    //     //     {
    //     //         print("SLOPE");
    //     //         collider.material.dynamicFriction = 0;
    //     //         Gravity = startGravity * 5;
    //     //         return false;
    //     //     }
    //     // }
    //     // else
    //     // {
    //     //     slopeCount = 0;
    //     // }

    //     // return didHit;
    // }

    void OnCollisionStay(Collision collisionInfo)
    {
        // Debug-draw all contact points and normals
        foreach (ContactPoint contact in collisionInfo.contacts)
        {
            Debug.DrawRay(contact.point, contact.normal, Color.red);

            if (contact.point.y - 0.1f <= transform.position.y - collider.height / 2f)
            {
                isGrounded = true;

                if (Math.Abs(contact.normal.x) > 0.5f || Math.Abs(contact.normal.z) > 0.5f)
                {
                    slopeCount++;

                    if (slopeCount > 30)
                    {
                        print("SLOPE");
                        collider.material.dynamicFriction = 0;
                        Gravity = startGravity * 5;
                        isGrounded = false;
                    }
                }
                else
                {
                    slopeCount = 0;
                }
            }
        }
    }

    void OnCollisionExit(Collision collisionInfo)
    {
        // Debug-draw all contact points and normals
        foreach (ContactPoint contact in collisionInfo.contacts)
        {
            Debug.DrawRay(contact.point, contact.normal, Color.red);

            if (contact.point.y - 0.1f <= transform.position.y - collider.height / 2f)
            {
                isGrounded = false;

                if (Math.Abs(contact.normal.x) > 0.5f || Math.Abs(contact.normal.z) > 0.5f)
                {
                    slopeCount++;

                    if (slopeCount > 30)
                    {
                        collider.material.dynamicFriction = 0;
                        Gravity = startGravity * 5;
                        isGrounded = false;
                    }
                }
                else
                {
                    slopeCount = 0;
                }
            }
        }
    }

    private void OnChildTriggerStay(Collider other)
    {
        if (liver.IsInvincible || isDodgeInvincible) return;

        if (other.CompareTag("HurtEnvironment") || other.CompareTag("HurtEnemy"))
        {
            ChangeState(PlayerState.Hurt);
            Instantiate(Prefabs.Instance.HitEffect, transform.transform.position, Quaternion.identity);
            liver.TakeDamage(1, other.gameObject);
        }
    }

    // Animation Events

    private void AttackEnd(int parameter = 0)
    {
        if (animationState != PlayerAnimationState.Attack) return;

        if (parameter == 1)
        {
            ChangeState(PlayerState.Normal);
        }
        else if (parameter == 2)
        {
            canGoToNextAttack = true;
        }
        else
        {
            canEndAttack = true;
        }
    }

    private void Attack2End(int parameter = 0)
    {
        if (animationState != PlayerAnimationState.Attack2) return;

        if (parameter == 1)
        {
            ChangeState(PlayerState.Normal);
        }
        else if (parameter == 2)
        {
            canGoToNextAttack = true;
        }
        else
        {
            canEndAttack = true;
        }
    }

    private void DodgeEnd(int parameter = 0)
    {
        isDodgeInvincible = parameter == 1;
    }

    private void SplatEnd(int parameter = 0)
    {
        isDodgeInvincible = false;
        canGetUp = true;
    }

    private void HurtEnd()
    {
        ChangeState(PlayerState.Normal);
    }

    private void HealEnd(int parameter = 0)
    {
        if (parameter == 0)
        {
            liver.Health = liver.MaxHealth;
            potionCount -= 1;
        }
        else if (parameter == 1)
        {
            ChangeState(PlayerState.Normal);
        }
    }
}
