using System.Collections;
using UnityEngine;

public class EnemyFlyBomb : EnemyBasic
{
    [SerializeField] GameObject explosion;
    [SerializeField] AnimationCurve explosionPulseCurve;
    [SerializeField] float maxSpeed;

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
        // enemy primes explosive
        StartCoroutine(PrimeExplosion());
        currentState = EnemyState.Waiting;
    }

    // primes explosion on death
    protected override void Dead()
    {
        base.Dead();
        StartCoroutine(PrimeExplosion());
    }

    // enemy can fly wherever it wants
    protected override Vector3 CalculateForce(Vector3 target, float multiplier)
    {
        return multiplier * Vector3.Normalize(target - transform.position);
    }
    
    // flash faster and faster before exploding
    private IEnumerator PrimeExplosion()
    {
        float t = 0f;
        while (t < deathTime)
        {
            float currentCurveValue = explosionPulseCurve.Evaluate(t / deathTime);

            bodyMaterial.color = Color.Lerp(Color.white, damagedColor, currentCurveValue);

            t += Time.deltaTime;
            yield return null;
        }
        Explode();
    }

    // create explosion and die
    private void Explode()
    {
        Instantiate(explosion, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }
}
