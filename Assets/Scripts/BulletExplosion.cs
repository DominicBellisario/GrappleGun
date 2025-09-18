using UnityEngine;

public class BulletExplosion : MonoBehaviour
{
    [SerializeField] float hitboxActiveTime;

    private void Update()
    {
        hitboxActiveTime -= Time.deltaTime;
        if (hitboxActiveTime <= 0 )
        {
            GetComponent<Collider>().enabled = false;
        }
    }
}
