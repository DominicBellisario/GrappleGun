using UnityEngine;

public class EnemyFlyBomb : EnemyBasic
{
    [SerializeField] protected float maxSpeed;

    protected override void Idle()
    {
        base.Idle();
        // flying enemy has velocity clamped
        rb.linearVelocity = Vector3.ClampMagnitude(rb.linearVelocity, maxSpeed);
    }

    protected override void Aware()
    {
        base.Aware();
        // flying enemy has velocity clamped
        rb.linearVelocity = Vector3.ClampMagnitude(rb.linearVelocity, maxSpeed);
    }

    protected override void Attacking()
    {
        base.Attacking();
        rb.AddForce(CalculateForce(player.transform.position, acceleration * Time.deltaTime), ForceMode.Acceleration);
        RotateEnemy();
        // flying enemy has velocity clamped
        rb.linearVelocity = Vector3.ClampMagnitude(rb.linearVelocity, maxSpeed);
    }

    // no changes in dead
    protected override void Dead()
    {
        base.Dead();
    }
    
    // enemy can fly wherever it wants
    protected override Vector3 CalculateForce(Vector3 target, float multiplier)
    {
        return multiplier * Vector3.Normalize(target - transform.position);
    }
}
