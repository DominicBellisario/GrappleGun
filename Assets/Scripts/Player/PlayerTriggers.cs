using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(PlayerEvents))]
public class PlayerTriggers : MonoBehaviour
{
    [SerializeField] Timer timer;
    Rigidbody rb;
    GVar gvar;
    PlayerEvents pEvents;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        pEvents = GetComponent<PlayerEvents>();
        gvar = GVar.Instance;
    }

    void OnTriggerEnter(Collider trigger)
    {
        if (trigger.gameObject.CompareTag("Jump Pad"))
        {
            //reset velocity and launch in the direction the pad is facing
            rb.linearVelocity = Vector3.zero;
            rb.AddForce(trigger.gameObject.GetComponent<JumpPad>().GetLaunchForceAndActivatePad(), ForceMode.VelocityChange);
            StartCoroutine(SwitchInterpolation());
        }

        // send player back to start and reset timer if they reach the end
        else if (trigger.gameObject.CompareTag("Target"))
        {
            // reset the scene
            pEvents.OutOfBounds();
            gvar.CurrentCheckpoint = Vector3.zero;
        }

        // start the timer once they start the level
        else if (trigger.gameObject.CompareTag("Start Level"))
        {
            if (timer != null) { timer.TimerStart(); }
        }

        // launch the player away from the explosion
        else if (trigger.gameObject.CompareTag("Bullet Explosion"))
        {
            rb.AddExplosionForce(trigger.gameObject.GetComponent<BulletExplosion>().BulletExplosionForce, trigger.gameObject.transform.position, 10f);
        }

        // launch the player away from the explosion and damage them
        else if (trigger.gameObject.CompareTag("Enemy Explosion"))
        {
            rb.linearVelocity = Vector3.zero;
            rb.AddExplosionForce(trigger.gameObject.GetComponent<BulletExplosion>().BulletExplosionForce, trigger.gameObject.transform.position, 10f, 1f);
            pEvents.DecreaseHealth(trigger.gameObject.GetComponent<BulletExplosion>().Damage);
        }

        // set a checkpoint
        else if (trigger.gameObject.CompareTag("Checkpoint"))
        {
            gvar.CurrentCheckpoint = trigger.gameObject.transform.position;
        }

        // reset the scene and tp the player to the last checkpoint if they hit one
        else if (trigger.gameObject.CompareTag("Death Plain"))
        {
            pEvents.OutOfBounds();
        }
    }

    IEnumerator SwitchInterpolation()
    {
        rb.interpolation = RigidbodyInterpolation.None;
        yield return new WaitForSeconds(0.1f);
        rb.interpolation = RigidbodyInterpolation.Interpolate;
    }
}
