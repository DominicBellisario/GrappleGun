using System;
using UnityEngine;

[RequireComponent(typeof(Raycasts))]
public class GrappleRangeRaycast : MonoBehaviour
{
    // --- EVENTS ---
    public static event Action<string> OnDetectedSurfaceChange;
    
    LayerMask mask;
    static float raycastRange;

    string lastRecordedTag;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        raycastRange = GVar.Instance.GrappleMaxDistance;
        mask = GetComponent<Raycasts>().targetableLayers;
        lastRecordedTag = "";
    }

    // Update is called once per frame
    void Update()
    {
        //shoot a raycast that shows the max range of the grapple
        Physics.Raycast(transform.position, transform.forward, out RaycastHit hit, raycastRange, mask);

        string currentTag = "";

        if (hit.collider != null) { currentTag = hit.collider.gameObject.tag; }

        if (currentTag != lastRecordedTag)
        {
            lastRecordedTag = currentTag;
            OnDetectedSurfaceChange?.Invoke(currentTag);
        }
    }
}
