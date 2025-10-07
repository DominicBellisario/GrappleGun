using UnityEngine;

public class BirdEffects : MonoBehaviour
{
    [SerializeField] GameObject body;
    [SerializeField] ParticleSystem birdHitEffect;
    [SerializeField] float rotationSpeed;

    void Update()
    {
        // rotate the bird
        body.transform.Rotate(rotationSpeed * Time.deltaTime * Vector3.up, Space.World);
        body.transform.Rotate(-rotationSpeed * Time.deltaTime * Vector3.left, Space.World);
        body.transform.Rotate(rotationSpeed * Time.deltaTime * Vector3.forward, Space.World);
    }
}
