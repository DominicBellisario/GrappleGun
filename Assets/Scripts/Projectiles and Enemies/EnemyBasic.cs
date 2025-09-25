using System.Collections;
using UnityEngine;

public class EnemyBasic : Enemy
{
    [SerializeField] bool isGrounded;


    protected override void Idle()
    {
        base.Idle();
        if (!canMove) return;
        rb.AddForce(CalculateForce(startPos), ForceMode.Acceleration);
        rb.linearVelocity = Vector3.ClampMagnitude(rb.linearVelocity, maxSpeed);
        RotateEnemy();
    }

    protected override void Aware()
    {
        base.Aware();
        if (!canMove) return;
        rb.AddForce(CalculateForce(player.transform.position), ForceMode.Acceleration);
        rb.linearVelocity = Vector3.ClampMagnitude(rb.linearVelocity, maxSpeed);
        RotateEnemy();
    }

    protected override void Attacking()
    {
        base.Attacking();
        if (!canMove) return;
        rb.AddForce(CalculateForce(player.transform.position), ForceMode.Acceleration);
        rb.linearVelocity = Vector3.ClampMagnitude(rb.linearVelocity, maxSpeed);
        RotateEnemy();
    }

    protected override void Dead()
    {
        base.Dead();
        // let the enemy ragdoll
        rb.constraints = RigidbodyConstraints.None;
        // destroy the enemy after a time
        StartCoroutine(Helper.DoThisAfterDelay(deathTime, () => Destroy(gameObject)));
    }

    private Vector3 CalculateForce(Vector3 target)
    {
        //remove y axis from calculation if the enemy is grounded
        if (!isGrounded)
        {
            return Vector3.Normalize(target - transform.position) * acceleration;
        }
        else
        {
            Vector3 horizontalDistance = target - transform.position;
            horizontalDistance.y = 0f;
            return Vector3.Normalize(horizontalDistance) * acceleration;
        }
    }

    // rotate enemy in the direction they are facing
    private void RotateEnemy()
    {
        Vector3 velocity = rb.linearVelocity.normalized;
        if (velocity == Vector3.zero) return;

        Quaternion targetRot = Quaternion.LookRotation(velocity);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, 10f * Time.deltaTime);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Bullet") && isVulnerable)
        {
            //enemy takes damage
            TakeDamage(collision.gameObject.GetComponent<Bullet>().Damage);
        }
        else if (collision.gameObject.CompareTag("Player"))
        {
            player.GetComponent<PlayerEvents>().DecreaseHealth(damage);
            player.GetComponent<Rigidbody>().AddExplosionForce(knockbackForce, transform.position, 1f, 1f, ForceMode.Impulse);
        }
    }

    private void OnTriggerEnter(Collider collider)
    {
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
