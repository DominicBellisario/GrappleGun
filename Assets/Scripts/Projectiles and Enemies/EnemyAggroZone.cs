using UnityEngine;

[RequireComponent(typeof(SphereCollider))]
public class EnemyAggroZone : MonoBehaviour
{
    private Enemy parent;

    void Awake()
    {
        parent = GetComponentInParent<Enemy>();
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player")) parent.OnZoneEnter(gameObject.name);            
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player")) parent.OnZoneExit(gameObject.name);
    }
}
