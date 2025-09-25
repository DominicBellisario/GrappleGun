using System.Collections;
using UnityEngine;

[RequireComponent(typeof(SphereCollider))]
public class BulletExplosion : MonoBehaviour
{
    [SerializeField] float hitboxActiveTime;
    [SerializeField] float bulletExplosionForce;
    public float BulletExplosionForce { get { return bulletExplosionForce; } }
    [SerializeField] int damage;
    public int Damage { get { return damage; } }
    
    SphereCollider sphereCollider;
    float startRadius;
    void Start()
    {
        sphereCollider = GetComponent<SphereCollider>();
        startRadius = sphereCollider.radius;
        sphereCollider.radius = 0.01f;
        StartCoroutine(ExpandRadius());
    }

    private IEnumerator ExpandRadius()
    {
        float t = 0f;
        while (t < hitboxActiveTime)
        {
            t += Time.deltaTime;
            sphereCollider.radius = Mathf.Lerp(0f, startRadius, t / hitboxActiveTime);
            yield return null;
        }
        sphereCollider.enabled = false;
    }
}
