using System.Collections;
using System.Runtime.CompilerServices;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraEffects : MonoBehaviour
{
    [SerializeField] Camera weaponCam;

    [Header("General FOV Warp Settings")]
    [SerializeField] float fovResetTime; //.1
    [SerializeField] float fovWarpTimeForBirdReel; //.25
    [SerializeField] float fovMaxWarpForBirdReel; // 90

    [Header("FOV Warp Settings for Dash")]
    [SerializeField] float fovMaxWarpForDash;
    [SerializeField] float fovWarpInTimeForDash;
    [SerializeField] float fovWaitTimeForDash;
    [SerializeField] float fovWarpOutTimeForDash;

    Camera cam;
    float startFOV;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        cam = GetComponent<Camera>();
        startFOV = cam.fieldOfView;
    }

    void OnEnable()
    {
        PlayerController.OnDashEvent += StartWarpFOVForDash;
        GrapplePhysics.OnBirdLaunch += ResetFOV;
        GrappleHead.OnGrappleHitReelEvent += WarpFOVForBirdReel;
        GrappleHead.OnGrappleHitBirdEvent += WarpFOVForBirdReel;
        GrapplePhysics.OnReelStick += ResetFOV;
        GrapplePhysics.OnFailsafeBirdReelDetatch += ResetFOV;
    }
    void OnDisable()
    {
        PlayerController.OnDashEvent -= StartWarpFOVForDash;
        GrapplePhysics.OnBirdLaunch -= ResetFOV;
        GrappleHead.OnGrappleHitReelEvent -= WarpFOVForBirdReel;
        GrappleHead.OnGrappleHitBirdEvent -= WarpFOVForBirdReel;
        GrapplePhysics.OnReelStick -= ResetFOV;
        GrapplePhysics.OnFailsafeBirdReelDetatch -= ResetFOV;
    }
    private void ResetFOV() { StartCoroutine(WarpFOV(fovResetTime, 0f, true)); }
    private void StartWarpFOVForDash() { StartCoroutine(WarpFOVForDash()); }
    private void WarpFOVForBirdReel(Collision unused1, int unused2) { StartCoroutine(WarpFOV(fovWarpTimeForBirdReel, fovMaxWarpForBirdReel, false)); }

    private IEnumerator WarpFOVForDash()
    {
        float t = 0;
        while (t < fovWarpInTimeForDash)
        {
            cam.fieldOfView = Mathf.Lerp(startFOV, fovMaxWarpForDash, t / fovWarpInTimeForDash);
            weaponCam.fieldOfView = Mathf.Lerp(startFOV, fovMaxWarpForDash, t / fovWarpInTimeForDash);
            t += Time.deltaTime;
            yield return null;
        }

        t = 0;
        yield return new WaitForSeconds(fovWaitTimeForDash);

        while (t < fovWarpOutTimeForDash)
        {
            cam.fieldOfView = Mathf.Lerp(fovMaxWarpForDash, startFOV, t / fovWarpOutTimeForDash);
            weaponCam.fieldOfView = Mathf.Lerp(fovMaxWarpForDash, startFOV, t / fovWarpOutTimeForDash);
            t += Time.deltaTime;
            yield return null;
        }
        cam.fieldOfView = startFOV;
    }

    private IEnumerator WarpFOV(float time, float targetFOV, bool resetToStartFOVWhenDone)
    {
        float t = 0;
        float fovBefore = cam.fieldOfView;
        if (resetToStartFOVWhenDone) targetFOV = startFOV;
        while (t < time)
        {
            cam.fieldOfView = Mathf.Lerp(fovBefore, targetFOV, t / time);
            weaponCam.fieldOfView = Mathf.Lerp(fovBefore, fovMaxWarpForDash, t / time);
            t += Time.deltaTime;
            yield return null;
        }
        if (resetToStartFOVWhenDone) cam.fieldOfView = startFOV;
    }
}
