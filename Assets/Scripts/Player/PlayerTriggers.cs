using System;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(PlayerEvents))]
public class PlayerTriggers : MonoBehaviour
{
    // --- EVENTS --- 
    public static event Action<float> OnPlayerStartLevel;
    public static event Action OnPlayerReachedTarget;

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
            //record the time they finished
            //display the time
            OnPlayerReachedTarget?.Invoke();

            // reset the scene after a bit
            StartCoroutine(Helper.DoThisAfterDelay(3.0f, () => pEvents.OutOfBounds()));
            
            // reset the checkpoint
            gvar.ResetCheckpoint();
        }

        // start the timer once they start the level
        else if (trigger.gameObject.CompareTag("Start Level"))
        {
            OnPlayerStartLevel?.Invoke(0);
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
            gvar.CurrentCheckpointPos = trigger.gameObject.transform.position;
            gvar.CurrentCheckpointRotation = trigger.gameObject.transform.GetChild(0).eulerAngles;
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
