using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerTriggers : MonoBehaviour
{
    [SerializeField] Timer timer;
    Rigidbody rb;
    GVar gvar;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
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

        else if (trigger.gameObject.CompareTag("Target"))
        {
            rb.linearVelocity = Vector3.zero;
            transform.position = new Vector3(0, 50, -20);
            timer.TimerSequence(false);
        }

        else if (trigger.gameObject.CompareTag("Start Level"))
        {
            if (timer != null)
            {
                timer.TimerSequence(true);
            }
        }

        else if (trigger.gameObject.CompareTag("Bullet Explosion"))
        {
            rb.AddExplosionForce(gvar.BulletExplosionForce, trigger.gameObject.transform.position, 3f);
        }
    } 

    IEnumerator SwitchInterpolation()
    {
        rb.interpolation = RigidbodyInterpolation.None;
        yield return new WaitForSeconds(0.1f);
        rb.interpolation = RigidbodyInterpolation.Interpolate;
    }
}
