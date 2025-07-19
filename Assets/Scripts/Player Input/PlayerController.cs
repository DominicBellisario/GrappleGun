using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Scripting.APIUpdating;

public class PlayerController : MonoBehaviour
{
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

    /// <summary>
    /// The Rigidbody component attached to the player.
    /// </summary>
    Rigidbody rb;
    /// <summary>
    /// The current force applied to the player this frame.
    /// </summary>
    Vector3 walkForceThisFrame;


    void Start()
    {
        rb = GetComponent<Rigidbody>();
        walkForceThisFrame = Vector3.zero;
    }

    void Update()
    {
        // Apply the current movement force to the Rigidbody2D
        if (walkForceThisFrame != Vector3.zero)
        {
            //Debug.Log("Applying force: " + walkForceThisFrame);

            // Calculate the new velocity based on the current force and the Rigidbody's existing velocity
            Vector3 newVelocity = rb.linearVelocity + (walkForceThisFrame * Time.deltaTime);

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
        Debug.Log("Jump");
        // Apply an impulse force to the Rigidbody2D to make the player jump
        rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
    }
}
