using UnityEngine;

public class EnemyGroundJump : EnemyBasic
{
    [SerializeField] Vector2 jumpForce;
    [SerializeField] float attackTime;
    // no changes in idle
    protected override void Idle()
    {
        base.Idle();
    }

    // no changes in aware
    protected override void Aware()
    {
        base.Aware();
    }

    protected override void Attacking()
    {
        base.Attacking();
        // enemy jumps at player
        rb.AddForce(CalculateForce(player.transform.position, jumpForce.x) + new Vector3(0f, jumpForce.y, 0f), ForceMode.Impulse);
        // enemy waits a bit, then resets
        currentState = EnemyState.Waiting;
        StartCoroutine(Helper.DoThisAfterDelay(attackTime, () => ResetCurrentState()));
    }

    // no changes in dead
    protected override void Dead()
    {
        base.Dead();
    }

    // enemy only moves horizontaly
    protected override Vector3 CalculateForce(Vector3 target, float multiplier)
    {
        Vector3 horizontalDistance = target - transform.position;
        horizontalDistance.y = 0f;
        return multiplier * Vector3.Normalize(horizontalDistance);
    }
}
