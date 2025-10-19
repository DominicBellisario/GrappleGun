using UnityEngine;

[RequireComponent(typeof(ParticleSystem))]
public class GrappleSparks : MonoBehaviour
{
    /// <summary>
    /// the change in the distance between the grapple head and the player to start the sparks
    /// </summary>
    [SerializeField] float distanceChangeToStart;
    float lastDistance;
    ParticleSystem ps;
    GVar gvar;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        gvar = GVar.Instance;
        lastDistance = gvar.CurrentRopeLength;
        ps = GetComponent<ParticleSystem>();
    }

    // Update is called once per frame
    void Update()
    {
        float distanceChangeToStartDT = distanceChangeToStart * Time.deltaTime;
        if (Mathf.Abs(gvar.CurrentRopeLength - lastDistance) > distanceChangeToStartDT)
        {
            if (!ps.isPlaying) ps.Play();
        }
        else
        {
            if (ps.isPlaying) ps.Stop();
        }
        lastDistance = gvar.CurrentRopeLength;
    }
}
