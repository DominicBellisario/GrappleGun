using System;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Raycasts))]
public class PlayerController : MonoBehaviour
{
    /// <summary>
    /// the player's first person camera
    /// </summary>
    [SerializeField] GameObject playerCam;
    /// <summary>
    /// The Rigidbody component attached to the player.
    /// </summary>
    Rigidbody rb;


    [Header("Standard Movement Values")]
    /// <summary>
    /// The force applied when the player jumps.
    /// </summary>
    [SerializeField] float jumpForce;
    /// <summary>
    /// The acceleration applied to the player when moving on the ground.
    /// </summary>
    [SerializeField] float walkAcceleration;
    /// <summary>
    /// The acceleration applied to the player when moving in the air.
    /// </summary>
    [SerializeField] float airAcceleration;
    /// <summary>
    /// The maximum speed the player can reach while only walking
    /// </summary>
    [SerializeField] float groundMaxHorizSpeed;
    /// <summary>
    /// The current force applied to the player this frame.
    /// </summary>
    Vector2 movementInputThisFrame;


    [Header("Boost Settings")]
    /// <summary>
    /// The upwards force applied to the player when boosting.
    /// </summary>
    [SerializeField] float boostForce;
    /// <summary>
    /// The horizontal force applied to the player when pressing WASD when boosting.
    /// </summary>
    [SerializeField] float boostAcceleration;
    /// <summary>
    /// the amount of boost fuel spent per second while using boost. max fuel is 100 
    /// </summary>
    [SerializeField] float boostFuelUse;
    /// <summary>
    /// the regen rate for boost fuel while grounded. max fuel is 100
    /// </summary>
    [SerializeField] float boostFuelRegen;
    /// <summary>
    /// keeps track of whether the player is boosting or not.
    /// </summary>
    bool isBoosting;


    [Header("Mouse Settings")]
    [SerializeField] float xMouseSensitivity;
    [SerializeField] float yMouseSensitivity;
    /// <summary>
    /// The current rotation of the mouse on the X axis.
    /// this is needed becuause getting the mouse rotation from the camera
    /// can cause the camera to flip when looking up and down.
    /// </summary>
    Vector2 mouseRotation;

    [Header("Grapple Settings")]
    /// <summary>
    /// The point where the grapple will be spawned from.
    /// </summary>
    [SerializeField] GameObject grappleStart;
    [SerializeField] GameObject grappleHead;
    [SerializeField] float reelInForce;


    /// <summary>
    /// wether or not the grapple can be shot
    /// if it is coming back to the player, it cannot be shot
    /// </summary>
    public bool CanUseGrapple { get; set; }

    /// <summary>
    /// the current amount of boost fuel
    /// </summary>
    public float CurrentBoostFuel { get; set; }


    void Start()
    {
        rb = GetComponent<Rigidbody>();

        movementInputThisFrame = Vector2.zero;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        mouseRotation = Vector2.zero;

        isBoosting = false;
        CurrentBoostFuel = 100f;

        CanUseGrapple = true;
    }

    void Update()
    {
        //get the current surface the player is on, if any
        RaycastHit downRaycastHit = GetComponent<Raycasts>().DownRaycastHit;

        // Apply the current movement force to the Rigidbody2D
        if (movementInputThisFrame != Vector2.zero)
        {
            // If the player is grounded, apply the walk acceleration
            if (downRaycastHit.collider != null) { MovePlayer(downRaycastHit, walkAcceleration); }
            // If the player is in the air and boosting, apply the boost acceleration
            else if (isBoosting) { MovePlayer(downRaycastHit, boostAcceleration); }
            // If the player is in the air and not boosting, apply the air acceleration
            else { MovePlayer(downRaycastHit, airAcceleration); }
        }

        //apply boost when boosting
        if (isBoosting)
        {
            if (CurrentBoostFuel > 0f)
            {
                rb.AddForce(boostForce * Time.deltaTime * Vector3.up, ForceMode.Impulse);
                CurrentBoostFuel -= boostFuelUse * Time.deltaTime;
            }
        }
        //recharge boost when grounded
        else if (downRaycastHit.collider != null && CurrentBoostFuel < 100f)
        {
            CurrentBoostFuel += boostFuelRegen * Time.deltaTime;
        }

        //speed is hard capped while grounded, prevents sliding at high entry speeds
        if (downRaycastHit.collider != null)
        {
            rb.linearVelocity = new Vector3(
                Mathf.Clamp(rb.linearVelocity.x, -groundMaxHorizSpeed, groundMaxHorizSpeed),
                rb.linearVelocity.y,
                Mathf.Clamp(rb.linearVelocity.z, -groundMaxHorizSpeed, groundMaxHorizSpeed));
        }
    }

    /// <summary>
    /// Method to handle the movement input from the player.
    /// This method is called when the move keys are pressed.
    /// </summary>
    /// <param name="inputValue"></param>
    private void OnMove(InputValue inputValue)
    {
        movementInputThisFrame = inputValue.Get<Vector2>();
    }

    private void MovePlayer(RaycastHit downRaycastHit, float acceleration)
    {
        // Apply the desired acceleration to the movement input in local space
        Vector3 inputForce = new Vector3(movementInputThisFrame.x, 0f, movementInputThisFrame.y) * acceleration;

        // Get the camera's yaw-only rotation (ignore pitch and roll)
        Vector3 camForward = playerCam.transform.forward;
        camForward.y = 0f;
        camForward.Normalize();

        Vector3 camRight = playerCam.transform.right;
        camRight.y = 0f;
        camRight.Normalize();

        // Build world-space force based on camera yaw
        Vector3 worldForce = camRight * inputForce.x + camForward * inputForce.z;

        // Get slope normal (default to up if no hit)
        Vector3 slopeNormal = downRaycastHit.collider ? downRaycastHit.normal : Vector3.up;

        // Project force onto slope plane
        Vector3 slopeForce = Vector3.ProjectOnPlane(worldForce, slopeNormal);

        // Predict new velocity
        Vector3 newVelocity = rb.linearVelocity + slopeForce * Time.deltaTime;

        // Horizontal speed checks
        float newSpeed = Mathf.Sqrt(newVelocity.x * newVelocity.x + newVelocity.z * newVelocity.z);
        float currentSpeed = Mathf.Sqrt(rb.linearVelocity.x * rb.linearVelocity.x + rb.linearVelocity.z * rb.linearVelocity.z);

        // Only apply if under speed cap or slowing player down
        if (newSpeed < currentSpeed || newSpeed <= groundMaxHorizSpeed)
        {
            rb.linearVelocity = newVelocity;
        }
    }

    /// <summary>
    /// Method to handle the jump action input from the player.
    /// This method is called when the jump key is pressed.
    /// </summary>
    private void OnJump(InputValue inputValue)
    {
        //Debug.Log("Jump input received: " + inputValue.isPressed);
        // if the button was pressed
        if (inputValue.isPressed)
        {
            // if the player is grounded, jump
            if (GetComponent<Raycasts>().DownRaycastHit.collider != null)
            {
                //Debug.Log("Jump");
                // Apply an impulse force to the Rigidbody2D to make the player jump
                rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            }
            // if the player is not grounded, activate the boost
            else
            {
                isBoosting = true;
            }
        }
        // if the button was released, stop boosting
        else
        {
            isBoosting = false;
        }
    }

    /// <summary>
    /// Method to handle the look input from the player.
    /// This method is called when the mouse is moved
    /// </summary>
    /// <param name="inputValue"></param>
    private void OnLook(InputValue inputValue)
    {
        Vector2 lookInput = inputValue.Get<Vector2>();
        //Debug.Log("Look input: " + lookInput);

        //rotate the camera horizontally
        mouseRotation.y += lookInput.x * xMouseSensitivity * Time.deltaTime;

        //rotate the camera vertically
        mouseRotation.x -= lookInput.y * yMouseSensitivity * Time.deltaTime;
        //clamp the vertical rotation to prevent flipping
        mouseRotation.x = Mathf.Clamp(mouseRotation.x, -85f, 85f);

        // Apply the clamped rotation to the camera
        playerCam.transform.localRotation = Quaternion.Euler(mouseRotation.x, mouseRotation.y, 0f);
    }

    /// <summary>
    /// Method to handle the grapple input from the player.
    /// This method is called when the grapple key is pressed.
    /// </summary>
    /// <param name="inputValue"></param>
    private void OnGrapple(InputValue inputValue)
    {
        //Debug.Log("Grapple input received: " + inputValue.isPressed);
        if (!CanUseGrapple) return;
        if (inputValue.isPressed)
        {
            // launch the grapple head towards the point hit by the forward raycast
            grappleHead.GetComponent<GrappleHead>().Launch(playerCam.GetComponent<Raycasts>().ForwardRaycastHit.point);
        }
        else
        {
            // return the grapple head to the gun
            grappleHead.GetComponent<GrappleHead>().StartCoroutine(grappleHead.GetComponent<GrappleHead>().ReturnToGun());
        }
    }

    private void OnReel(InputValue inputValue)
    {
        float value = inputValue.Get<Vector2>().y;
        if (value == -1 && grappleHead.GetComponent<GrappleHead>().IsAttached)
        {
            rb.AddForce((grappleHead.transform.position - transform.position).normalized * reelInForce);
        }
    }
}
