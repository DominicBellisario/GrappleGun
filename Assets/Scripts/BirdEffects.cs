using UnityEngine;

[RequireComponent(typeof(Collider))]
public class BirdEffects : MonoBehaviour
{
    [SerializeField] GameObject body;
    [SerializeField] ParticleSystem birdDeathParticles;
    [SerializeField] float birdDisableTime;
    [SerializeField] float rotationSpeed;

    Collider col;

    void Start()
    {
        col = GetComponent<Collider>();
    }

    void Update()
    {
        // rotate the bird
        body.transform.Rotate(rotationSpeed * Time.deltaTime * Vector3.up, Space.World);
        body.transform.Rotate(-rotationSpeed * Time.deltaTime * Vector3.left, Space.World);
        body.transform.Rotate(rotationSpeed * Time.deltaTime * Vector3.forward, Space.World);
    }

    public void Hit(Transform objectThatHitIt)
    {
        // rotate the particle system to face away from the object that hit it
        Quaternion guh = Quaternion.LookRotation(transform.position - objectThatHitIt.position);
        birdDeathParticles.transform.rotation = guh;
        Debug.Log(guh.eulerAngles);
        // play the hit effect
        birdDeathParticles.Play();
        // disable the bird
        SetBirdState(false);

        // re-enable the bird after a delay\
        StartCoroutine(Helper.DoThisAfterDelay(birdDisableTime, () => SetBirdState(true)));
    }

    private void SetBirdState(bool state)
    {
        body.SetActive(state);
        col.enabled = state;
    }
}
