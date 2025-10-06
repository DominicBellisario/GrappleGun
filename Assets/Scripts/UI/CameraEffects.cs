using System.Collections;
using UnityEngine;

public class CameraEffects : MonoBehaviour
{
    
    [SerializeField] float fovMaxWarp;
    [SerializeField] float fovWarpInTime;
    [SerializeField] float fovWaitTime;
    [SerializeField] float fovWarpOutTime;

    Camera cam;
    [SerializeField] Camera weaponCam;
    float startFOV;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        cam = GetComponent<Camera>();
        startFOV = cam.fieldOfView;
    }

    public IEnumerator WarpFOV()
    {
        float t = 0;
        while (t < fovWarpInTime)
        {
            cam.fieldOfView = Mathf.Lerp(startFOV, fovMaxWarp, t / fovWarpInTime);
            weaponCam.fieldOfView = Mathf.Lerp(startFOV, fovMaxWarp, t / fovWarpInTime);
            t += Time.deltaTime;
            yield return null;
        }

        t = 0;
        yield return new WaitForSeconds(fovWaitTime);

        while (t < fovWarpOutTime)
        {
            cam.fieldOfView = Mathf.Lerp(fovMaxWarp, startFOV, t / fovWarpOutTime);
            weaponCam.fieldOfView = Mathf.Lerp(fovMaxWarp, startFOV, t / fovWarpOutTime);
            t += Time.deltaTime;
            yield return null;
        }
        cam.fieldOfView = startFOV;
    }
}
