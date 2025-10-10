using System.Collections;
using UnityEngine;

public class CameraEffects : MonoBehaviour
{
    [SerializeField] Camera weaponCam;

    [Header("FOV Warp Settings for Dash")]
    [SerializeField] float fovMaxWarp;
    [SerializeField] float fovWarpInTime;
    [SerializeField] float fovWaitTime;
    [SerializeField] float fovWarpOutTime;

    Camera cam;
    float startFOV;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        cam = GetComponent<Camera>();
        startFOV = cam.fieldOfView;
    }

    public IEnumerator WarpFOVForDash()
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
    
    public IEnumerator WarpFOV(float time, float targetFOV, bool resetToStartFOVWhenDone)
    {
        float t = 0;
        float fovBefore = cam.fieldOfView;
        if (resetToStartFOVWhenDone) targetFOV = startFOV;
        while (t < time)
        {
            cam.fieldOfView = Mathf.Lerp(fovBefore, targetFOV, t / time);
            weaponCam.fieldOfView = Mathf.Lerp(fovBefore, fovMaxWarp, t / time);
            t += Time.deltaTime;
            yield return null;
        }
        if (resetToStartFOVWhenDone) cam.fieldOfView = startFOV;
    }
}
