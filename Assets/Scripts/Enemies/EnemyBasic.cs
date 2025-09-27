using UnityEngine;

public class EnemyBasic : Enemy
{
    [SerializeField] protected float acceleration;

    // enemy moves towards target and faces their velocity
    protected override void Idle()
    {
        base.Idle();
        rb.AddForce(CalculateForce(startPos, acceleration * Time.deltaTime), ForceMode.Acceleration);
        RotateEnemy();
    }

    // enemy moves towards player and faces their velocity
    protected override void Aware()
    {
        base.Aware();
        rb.AddForce(CalculateForce(player.transform.position, acceleration * Time.deltaTime), ForceMode.Acceleration);
        RotateEnemy();
    }

    // enemy moves towards player and faces their velocity
    protected override void Attacking()
    {
        base.Attacking();
    }

    // no changes in dead
    protected override void Dead()
    {
        base.Dead();
    }

    // calculated differently for different enemies
    protected virtual Vector3 CalculateForce(Vector3 target, float multiplier)
    {
        return Vector3.zero;
    }

    // rotate enemy in the direction they are facing
    protected void RotateEnemy()
    {
        Vector3 velocity = rb.linearVelocity.normalized;
        if (velocity == Vector3.zero) return;

        Quaternion targetRot = Quaternion.LookRotation(velocity);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, 10f * Time.deltaTime);
    }

    protected override void OnCollisionEnter(Collision collision)
    {
        base.OnCollisionEnter(collision);

        if (collision.gameObject.CompareTag("Player") && canDamagePlayer)
        {
            player.GetComponent<PlayerEvents>().DecreaseHealth(damage);
            player.GetComponent<Rigidbody>().AddExplosionForce(knockbackForce, transform.position, 10f, 1f, ForceMode.Impulse);
        }
    }

    protected override void OnTriggerEnter(Collider collider)
    {
        base.OnTriggerEnter(collider);

        if (collider.gameObject.CompareTag("Bullet Explosion") && isVulnerable)
        {
            BulletExplosion explosion = collider.gameObject.GetComponent<BulletExplosion>();
            // enemy takes damage
            TakeDamage(explosion.Damage);
            // launch them away from the explosion
            rb.AddExplosionForce(explosion.BulletExplosionForce, collider.gameObject.transform.position, 3f);
        }
    }
}
