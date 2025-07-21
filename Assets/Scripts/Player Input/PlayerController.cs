using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Scripting.APIUpdating;

public class PlayerController : MonoBehaviour
{
    /// <summary>
    /// the player's first person camera
    /// </summary>
    [SerializeField] GameObject playerCam;

    [Header("Movement Values")]
    /// <summary>
    /// The force applied when the player jumps.
    /// </summary>
    [SerializeField] float jumpForce;
    /// <summary>
    /// The acceleration applied to the player when moving on the ground.
    /// </summary>
    [SerializeField] float walkAcceleration;
    /// <summary>
    /// The maximum speed the player can reach while only walking
    /// </summary>
    [SerializeField] float groundMaxHorizSpeed;

    [Header("Mouse Settings")]
    [SerializeField] float xMouseSensitivity;
    [SerializeField] float yMouseSensitivity;

    /// <summary>
    /// The Rigidbody component attached to the player.
    /// </summary>
    Rigidbody rb;
    /// <summary>
    /// The current force applied to the player this frame.
    /// </summary>
    Vector3 walkForceThisFrame;
    /// <summary>
    /// The current rotation of the mouse on the X axis.
    /// this is needed becuause getting the mouse rotation from the camera
    /// can cause the camera to flip when looking up and down.
    /// </summary>
    float mouseXRotation;


    void Start()
    {
        rb = GetComponent<Rigidbody>();

        walkForceThisFrame = Vector3.zero;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        mouseXRotation = 0f;
    }

    void Update()
    {
        // Apply the current movement force to the Rigidbody2D
        if (walkForceThisFrame != Vector3.zero)
        {
            // Rotate the walk force based on the player's current rotation
            Vector3 rotatedWalkForce = Quaternion.Euler(0f, transform.eulerAngles.y, 0f) * walkForceThisFrame;

            // Calculate the new velocity based on the current force and the Rigidbody's existing velocity
            Vector3 newVelocity = rb.linearVelocity + (rotatedWalkForce * Time.deltaTime);

            //apply the force if this will not make the player exceed the maximum speed
            if (new Vector2(newVelocity.x, newVelocity.z).magnitude < groundMaxHorizSpeed)
            {
                // Clamp the velocity to the maximum speed
                newVelocity.x = Mathf.Clamp(newVelocity.x, -groundMaxHorizSpeed, groundMaxHorizSpeed);
                newVelocity.z = Mathf.Clamp(newVelocity.z, -groundMaxHorizSpeed, groundMaxHorizSpeed);
                //apply the velocity
                rb.linearVelocity = newVelocity;
            }
        }
    }

    /// <summary>
    /// Method to handle the movement input from the player.
    /// This method is called when the move keys are pressed.
    /// </summary>
    /// <param name="inputValue"></param>
    private void OnMove(InputValue inputValue)
    {
        Vector2 velocity = inputValue.Get<Vector2>() * walkAcceleration;
        walkForceThisFrame = new Vector3(velocity.x, rb.linearVelocity.y, velocity.y);
    }

    /// <summary>
    /// Method to handle the jump action input from the player.
    /// This method is called when the jump key is pressed.
    /// </summary>
    private void OnJump()
    {
        // if the player grounded, jump
        if (GetComponent<Raycasts>().DownRaycastHit)
        {
            Debug.Log("Jump");
            // Apply an impulse force to the Rigidbody2D to make the player jump
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            return;
        }
        Debug.Log("Cannot jump, not grounded");
    }

    /// <summary>
    /// Method to handle the look input from the player.
    /// This method is called when the mouse is moved
    /// </summary>
    /// <param name="inputValue"></param>
    private void OnLook(InputValue inputValue)
    {
        // This method can be used to handle player look input if needed
        // Currently, it does nothing but can be expanded for camera control or other purposes
        Vector2 lookInput = inputValue.Get<Vector2>();
        //Debug.Log("Look input: " + lookInput);
        //rotate the parent object horizontally
        transform.Rotate(Vector3.up, lookInput.x * xMouseSensitivity * Time.deltaTime);
        //rotate the camera vertically
        mouseXRotation -= lookInput.y * yMouseSensitivity * Time.deltaTime;
        mouseXRotation = Mathf.Clamp(mouseXRotation, -90f, 90f);
        Debug.Log("mouseXRotation: " + mouseXRotation);
        // Apply the clamped rotation to the camera
        playerCam.transform.localRotation = Quaternion.Euler(mouseXRotation, 0f, 0f);
    }
}
