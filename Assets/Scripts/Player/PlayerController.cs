using System;
using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Raycasts))]
[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    // --- EVENTS ---
    public static event Action<RaycastHit> OnShootGrappleEvent;
    public static event Action OnReturnGrappleEvent;
    public static event Action<RaycastHit> OnShootGunEvent;
    public static event Action OnDashEvent;
    public static event Action OnBoostStartEvent;
    public static event Action OnBoostStopEvent;
    public static event Action OnBoostEmptyEvent;

    /// <summary>
    /// the player's first person camera
    /// </summary>
    [SerializeField] GameObject playerCam;
    [SerializeField] GrappleLag grappleLag;
    [SerializeField] GunLag gunLag;
    /// <summary>
    /// The Rigidbody component attached to the player.
    /// </summary>
    Rigidbody rb;

    /// <summary>
    /// The current force applied to the player this frame.
    /// </summary>
    Vector2 movementInputThisFrame;

    /// <summary>
    /// The current rotation of the mouse on the X axis.
    /// this is needed becuause getting the mouse rotation from the camera
    /// can cause the camera to flip when looking up and down.
    /// </summary>
    Vector2 mouseRotation;
    Vector2 lookInput;

    bool isReloaded;

    /// <summary>
    /// wether or not the player is stuck to a reel surface
    /// </summary>
    public bool IsStuck { get; set; }

    GVar gvar;

    /// <summary>
    /// wether or not the grapple can be shot
    /// if it is coming back to the player, it cannot be shot
    /// </summary>
    public bool CanUseGrapple { get; set; }
    private void CanUseGrappleTrue() { CanUseGrapple = true; }
    private void CanUseGrappleFalse() { CanUseGrapple = false; }

    /// <summary>
    /// the current amount of boost fuel
    /// </summary>
    public float CurrentBoostFuel { get; set; }

    /// <summary>
    /// keeps track of whether the player is boosting or not.
    /// </summary>
    public bool IsBoosting { get; set; }

    public float CurrentDashCharge { get; set; }

    public bool CanDash { get; set; }


    void Start()
    {
        gvar = GVar.Instance;
        rb = GetComponent<Rigidbody>();

        movementInputThisFrame = Vector2.zero;
        lookInput = Vector2.zero;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        //set the player rotation to the last recorded checkpoint
        mouseRotation = gvar.CurrentCheckpointRotation;

        IsBoosting = false;
        CurrentBoostFuel = 100f;

        CanDash = true;
        CurrentDashCharge = gvar.DashChargeTime;

        // wait a bit before allowing the grapple or gun to be used so they dont shoot before teleporting
        CanUseGrapple = false;
        StartCoroutine(Helper.DoThisAfterDelay(0.25f, () => CanUseGrapple = true));
        isReloaded = false;
        StartCoroutine(Helper.DoThisAfterDelay(0.25f, () => isReloaded = true));

        IsStuck = false;
    }

    void OnEnable()
    {
        GrappleHead.OnStartGrappleReturnEvent += HandleStartReturn;
        GrappleHead.OnEndGrappleReturnEvent += HandleEndReturn;
        GrappleHead.OnGrappleHitReelEvent += HandleHitReel;
        GrappleHead.OnGrappleHitBirdEvent += HandleHitBird;
        GrapplePhysics.OnReelStick += HandleReelStick;
    }

    void OnDisable()
    {
        GrappleHead.OnStartGrappleReturnEvent -= HandleStartReturn;
        GrappleHead.OnEndGrappleReturnEvent -= HandleEndReturn;
        GrappleHead.OnGrappleHitReelEvent -= HandleHitReel;
        GrappleHead.OnGrappleHitBirdEvent -= HandleHitBird;
        GrapplePhysics.OnReelStick -= HandleReelStick;
    }

    void HandleStartReturn() => CanUseGrapple = false;
    void HandleEndReturn(float time) => CanUseGrapple = true;

    void HandleHitReel(Collision collision, int type)
    {
        CanUseGrapple = false;
        IsStuck = false;
        rb.constraints = RigidbodyConstraints.FreezeRotation;
    }
    void HandleReelStick()
    {
        IsStuck = true; 
        rb.constraints = RigidbodyConstraints.FreezeAll;
    }

    void HandleHitBird(Collision collision, int type)
    {
        CanUseGrapple = false;
        IsStuck = false;
        rb.constraints = RigidbodyConstraints.FreezeRotation;
    }

    void Update()
    {
        //get the current surface the player is on, if any
        RaycastHit downRaycastHit = GetComponent<Raycasts>().DownRaycastHit;

        // Apply the current movement force to the Rigidbody2D
        if (movementInputThisFrame != Vector2.zero)
        {
            // If the player is grounded, apply the walk acceleration
            if (downRaycastHit.collider != null) { MovePlayer(downRaycastHit, gvar.WalkAcceleration); }
            // If the player is in the air and boosting, apply the boost acceleration
            else if (IsBoosting) { MovePlayer(downRaycastHit, gvar.BoostAcceleration); }
            // If the player is in the air and not boosting, apply the air acceleration
            else { MovePlayer(downRaycastHit, gvar.AirAcceleration); }
        }

        //apply boost when boosting
        if (IsBoosting)
        {
            // keep boosting if there is fuel and the player is not grounded
            if (CurrentBoostFuel > 0f && downRaycastHit.collider == null)
            {
                rb.AddForce(gvar.BoostForce * Time.deltaTime * Vector3.up, ForceMode.Impulse);
                CurrentBoostFuel -= gvar.BoostFuelUse * Time.deltaTime;
            }
            else
            {
                IsBoosting = false;
                // start playing boost stop sound
                // stop vibration for grapple and gun
                // stop playing boost particles
                OnBoostStopEvent?.Invoke();
            }
        }
        //recharge boost when grounded
        else if ((downRaycastHit.collider != null || IsStuck) && CurrentBoostFuel < 100f)
        {
            CurrentBoostFuel += gvar.BoostFuelRegen * Time.deltaTime;
        }

        //speed is hard capped while grounded, prevents sliding at high entry speeds
        if (downRaycastHit.collider != null || IsStuck)
        {
            // rb.linearVelocity = new Vector3(
            // Mathf.Clamp(rb.linearVelocity.x, -gvar.GroundMaxHorizSpeed, gvar.GroundMaxHorizSpeed),
            // rb.linearVelocity.y,
            // Mathf.Clamp(rb.linearVelocity.z, -gvar.GroundMaxHorizSpeed, gvar.GroundMaxHorizSpeed));

            // dash is reset when grounded
            CanDash = true;
        }
    }

    void LateUpdate()
    {
        if (gvar.IsPaused) return;
        //apply mouse look
        // Adjust by deltaTime for consistent rotation speed
        float deltaX = lookInput.x * gvar.MouseSensitivity * Time.deltaTime * 50f;
        float deltaY = lookInput.y * gvar.MouseSensitivity * Time.deltaTime * 50f;

        mouseRotation.y += deltaX;
        mouseRotation.x -= deltaY;
        mouseRotation.x = Mathf.Clamp(mouseRotation.x, -89f, 89f);

        playerCam.transform.localRotation = Quaternion.Euler(mouseRotation.x, mouseRotation.y, 0f);

        // update gun lag, etc.
        grappleLag.lookInput = lookInput;
        gunLag.lookInput = lookInput;
    }

    /// <summary>
    /// Method to handle the movement input from the player.
    /// This method is called when the move keys are pressed.
    /// </summary>
    /// <param name="inputValue"></param>
    private void OnMove(InputValue inputValue)
    {
        if (gvar.IsPaused)
        {
            movementInputThisFrame = Vector2.zero;
            return;
        }
        movementInputThisFrame = inputValue.Get<Vector2>();
    }

    private void MovePlayer(RaycastHit downRaycastHit, float acceleration)
    {
        // Input force alligned to the world grid
        Vector3 inputForce = new Vector3(movementInputThisFrame.x, 0f, movementInputThisFrame.y) * acceleration;

        Vector3 camForward = playerCam.transform.forward;
        camForward.y = 0f;
        camForward.Normalize();

        Vector3 camRight = playerCam.transform.right;
        camRight.y = 0f;
        camRight.Normalize();

        //the force alligned to the camera
        Vector3 worldForce = camRight * inputForce.x + camForward * inputForce.z;

        // apply the force tagental to the normal of the surface the player is on
        Vector3 slopeNormal = downRaycastHit.collider ? downRaycastHit.normal : Vector3.up;
        Vector3 slopeForce = Vector3.ProjectOnPlane(worldForce, slopeNormal);

        Vector3 horizVel = new(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
        if (horizVel.magnitude >= gvar.GroundMaxHorizSpeed)
        {
            // Already at/above max speed = only keep sideways input
            Vector3 velDir = horizVel.normalized;
            float dot = Vector3.Dot(slopeForce, velDir);

            if (dot > 0f)
            {
                slopeForce -= velDir * dot; // remove forward component
            }
        }

        // Apply velocity change
        rb.linearVelocity += slopeForce * Time.deltaTime;
    }

    /// <summary>
    /// Method to handle the jump action input from the player.
    /// This method is called when the jump key is pressed.
    /// </summary>
    private void OnJump(InputValue inputValue)
    {
        if (gvar.IsPaused) return;

        // if the button was pressed
        if (inputValue.isPressed)
        {
            if (IsStuck)
            {
                IsStuck = false;
                rb.constraints = RigidbodyConstraints.FreezeRotation;
                //apply a force to the player in the direction the player is looking
                rb.AddForce(playerCam.transform.forward * gvar.ReelLaunchForce, ForceMode.Impulse);
                return;
            }
            // if the player is grounded, jump
            else if (GetComponent<Raycasts>().DownRaycastHit.collider != null)
            {
                // Apply an impulse force to the Rigidbody2D to make the player jump
                rb.AddForce(Vector3.up * gvar.JumpForce, ForceMode.Impulse);
            }
            // if the player is not grounded and has fuel, activate the boost, particles, and sound
            else if (CurrentBoostFuel > 0f)
            {
                IsBoosting = true;
                // start playing boost sound
                // start vibration for grapple and gun
                // start playing boost particles
                OnBoostStartEvent?.Invoke();
            }
            // they have no fuel, play empty boost sound
            else
            {
                //play empty boost sound
                OnBoostEmptyEvent?.Invoke();
            }
        }
        // if the button was released, stop boosting
        else if (IsBoosting)
        {
            IsBoosting = false;

            // start playing boost stop sound
            // stop vibration for grapple and gun
            // stop playing boost particles
            OnBoostStopEvent?.Invoke();
        }
    }

    /// <summary>
    /// Method to handle the look input from the player.
    /// This method is called when the mouse is moved
    /// </summary>
    /// <param name="inputValue"></param>
    private void OnLook(InputValue inputValue)
    {
        if (gvar.IsPaused)
        {
            lookInput = Vector2.zero;
            return;
        }
        lookInput = inputValue.Get<Vector2>();
    }

    /// <summary>
    /// Method to handle the grapple input from the player.
    /// This method is called when the grapple key is pressed.
    /// </summary>
    /// <param name="inputValue"></param>
    private void OnGrapple(InputValue inputValue)
    {
        if (gvar.IsPaused) return;
        if (!CanUseGrapple) return;
        if (inputValue.isPressed)
        {
            // launch the grapple head towards the point hit by the forward raycast
            // add recoil
            // spawn muzzle flash on grapple muzzle
            // play grapple shoot sound
            OnShootGrappleEvent?.Invoke(playerCam.GetComponent<Raycasts>().ForwardRaycastHit);
        }
        else
        {
            // return the grapple head to the gun
            OnReturnGrappleEvent?.Invoke();
        }
    }

    private void OnReel(InputValue inputValue)
    {
        //if (gvar.IsPaused) return;

        //get the scroll direction
        //float value = inputValue.Get<Vector2>().y;
        //if the direction is negative, reel the player in
        //if (value == -1 && grappleHead.GetComponent<GrappleHead>().IsAttached)
        //{
        //rb.AddForce((grappleHead.transform.position - transform.position).normalized * gvar.ReelInForce);
        //}
    }

    private void OnDash(InputValue inputValue)
    {
        if (gvar.IsPaused) return;

        if (inputValue.isPressed)
        {
            if (CurrentDashCharge == gvar.DashChargeTime && CanDash)
            {
                // unstick the player if they are stuck
                if (IsStuck)
                {
                    IsStuck = false;
                    rb.constraints = RigidbodyConstraints.FreezeRotation;
                }

                // add a force in the direction the player is facing
                Vector3 camForward = playerCam.transform.forward;
                camForward.y = 0f;
                camForward.Normalize();
                rb.AddForce(camForward * gvar.DashForce, ForceMode.VelocityChange);
                CurrentDashCharge = 0f;
                CanDash = false;
                StartCoroutine(ChargeDash());

                // plays dash sound
                // adds kickback to the grapple and gun
                // warp the camera
                OnDashEvent?.Invoke();
            }
        }
    }

    private IEnumerator ChargeDash()
    {
        while (CurrentDashCharge < gvar.DashChargeTime)
        {
            CurrentDashCharge += Time.deltaTime;
            yield return null;
        }
        CurrentDashCharge = gvar.DashChargeTime;
    }

    private void OnShoot(InputValue inputValue)
    {
        if (gvar.IsPaused) return;

        if (inputValue.isPressed && isReloaded)
        {
            // reload the gun after a delay
            isReloaded = false;
            StartCoroutine(Helper.DoThisAfterDelay(gvar.GunReloadTime, () => isReloaded = true));

            // fire the gun
            // apply recoil to the gun
            OnShootGunEvent?.Invoke(playerCam.GetComponent<Raycasts>().ForwardRaycastHit);
        }
    }
}
