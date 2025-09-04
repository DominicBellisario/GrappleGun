using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerTriggers : MonoBehaviour
{
    Rigidbody rb;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
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
    } 

    IEnumerator SwitchInterpolation()
    {
        rb.interpolation = RigidbodyInterpolation.None;
        yield return new WaitForSeconds(0.1f);
        rb.interpolation = RigidbodyInterpolation.Interpolate;
    }
}
