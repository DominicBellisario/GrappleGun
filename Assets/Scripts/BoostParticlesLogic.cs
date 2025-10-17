using UnityEngine;

[RequireComponent(typeof(ParticleSystem))]
public class BoostParticlesLogic : MonoBehaviour
{
    ParticleSystem pSystem;

    void Start()
    {
        pSystem = GetComponent<ParticleSystem>();
    }

    void OnEnable()
    {
        PlayerController.OnBoostStartEvent += () => pSystem.Play();
        PlayerController.OnBoostStopEvent += () => pSystem.Stop(false, ParticleSystemStopBehavior.StopEmitting);
    }
    void OnDisable()
    {
        PlayerController.OnBoostStartEvent -= () => pSystem.Play();
        PlayerController.OnBoostStopEvent += () => pSystem.Stop(false, ParticleSystemStopBehavior.StopEmitting);
    }
}
