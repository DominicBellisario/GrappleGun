using UnityEngine;

[RequireComponent(typeof(ParticleSystem))]
public class GrappleSparks : MonoBehaviour
{
    [SerializeField] GrappleHead grappleHead;
    /// <summary>
    /// the change in the distance between the grapple head and the player to start the sparks
    /// </summary>
    [SerializeField] float distanceChangeToStart;
    float lastDistance;
    ParticleSystem ps;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        lastDistance = grappleHead.CurrentRopeLength;
        ps = GetComponent<ParticleSystem>();
    }

    // Update is called once per frame
    void Update()
    {
        float distanceChangeToStartDT = distanceChangeToStart * Time.deltaTime;
        if (Mathf.Abs(grappleHead.CurrentRopeLength - lastDistance) > distanceChangeToStartDT)
        {
            if (!ps.isPlaying) ps.Play();
        }
        else
        {
            if (ps.isPlaying) ps.Stop();
        }
        lastDistance = grappleHead.CurrentRopeLength;
    }
}
