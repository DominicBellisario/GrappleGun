using UnityEngine;

public class BulletExplosion : MonoBehaviour
{
    [SerializeField] float hitboxActiveTime;
    [SerializeField] float bulletExplosionForce;
    public float BulletExplosionForce { get { return bulletExplosionForce; } }

    private void Update()
    {
        hitboxActiveTime -= Time.deltaTime;
        if (hitboxActiveTime <= 0)
        {
            GetComponent<Collider>().enabled = false;
        }
    }
}
