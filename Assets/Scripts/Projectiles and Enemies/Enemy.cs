using System.Collections;
using UnityEngine;

public enum EnemyState
{
    /// <summary>
    /// the enemy is not aware of the player
    /// </summary>
    Idle,
    /// <summary>
    /// the enemy is aware of the player
    /// </summary>
    Aware,
    /// <summary>
    /// the enemy is attacking the player
    /// </summary>
    Attacking,
    /// <summary>
    /// the enemy is stunned
    /// </summary>
    Stunned,
    /// <summary>
    /// the enemy is no more
    /// </summary>
    Dead,
    /// <summary>
    /// transitory state where nothing happens bc a coroutine is running
    /// </summary>
    Waiting
}

[RequireComponent(typeof(Rigidbody))]
public class Enemy : MonoBehaviour
{
    [SerializeField] protected int health;
    [SerializeField] protected float invulnTime;
    [SerializeField] protected float maxSpeed;
    [SerializeField] protected float acceleration;
    [SerializeField] protected int damage;
    [SerializeField] protected float knockbackForce;
    [SerializeField] protected float stunTime;
    [SerializeField] protected float deathTime;
    [SerializeField] protected SphereCollider awareZone;
    protected float aggroRange;
    [SerializeField] protected SphereCollider attackZone;
    protected float attackRange;

    protected EnemyState currentState;
    protected Rigidbody rb;
    protected GameObject player;
    protected Vector3 startPos;

    protected bool canDamagePlayer;
    protected bool canMove;
    protected bool canBeStunned;
    protected bool canDie;
    protected bool isVulnerable;

    protected virtual void Start()
    {
        aggroRange = awareZone.radius;
        attackRange = attackZone.radius;
        currentState = EnemyState.Idle;
        rb = GetComponent<Rigidbody>();
        player = GameObject.FindGameObjectWithTag("Player");
        startPos = transform.position;
        canDamagePlayer = damage > 0;
        canMove = maxSpeed > 0;
        canBeStunned = stunTime > 0;
        canDie = health > 0;
        isVulnerable = true;
    }

    protected virtual void Update()
    {
        switch (currentState)
        {
            case EnemyState.Idle:
                Idle();
                break;
            case EnemyState.Aware:
                Aware();
                break;
            case EnemyState.Attacking:
                Attacking();
                break;
            case EnemyState.Stunned:
                Stunned();
                break;
            case EnemyState.Dead:
                Dead();
                break;
        }
    }

    protected virtual void Idle() { }
    protected virtual void Aware() { }
    protected virtual void Attacking() { }
    protected virtual void Stunned()
    {
        currentState = EnemyState.Waiting;
        StartCoroutine(Helper.DoThisAfterDelay(stunTime, () => ResetCurrentState()));
    }
    protected virtual void Dead()
    {
        currentState = EnemyState.Waiting;
    }

    public void OnZoneEnter(string zoneName)
    {
        // do nothing if the enemy is dead or waiting
        if (currentState == EnemyState.Dead || currentState == EnemyState.Waiting) return;
        switch (zoneName)
        {
            case "Aware Zone":
                currentState = EnemyState.Aware;
                break;
            case "Attack Zone":
                currentState = EnemyState.Attacking;
                break;
        }
    }

    public void OnZoneExit(string zoneName)
    {
        // do nothing if the enemy is dead or waiting
        if (currentState == EnemyState.Dead || currentState == EnemyState.Waiting) return;
        switch (zoneName)
        {
            case "Aware Zone":
                currentState = EnemyState.Idle;
                break;
            case "Attack Zone":
                currentState = EnemyState.Aware;
                break;
        }
    }

    protected void ResetCurrentState()
    {
        float distance = Vector3.Distance(player.transform.position, transform.position);
        // determine what state the enemy should be in
        if (distance > aggroRange) { currentState = EnemyState.Idle; }
        else if (distance > attackRange) { currentState = EnemyState.Aware; }
        else { currentState = EnemyState.Attacking; }
    }

    protected void TakeDamage(int damage)
    {
         // enemy is immune to damage for a time
            isVulnerable = false;
            StartCoroutine(Helper.DoThisAfterDelay(invulnTime, () => isVulnerable = true));

            //stun them if possible
            if (canBeStunned) { currentState = EnemyState.Stunned; }

            // reduce health if they can
            if (canDie)
            {
                health -= damage;
                // if health is 0, they die
                if (health <= 0) { currentState = EnemyState.Dead; }
            }
    }
}
