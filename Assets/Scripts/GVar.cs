using UnityEngine;

public class GVar : MonoBehaviour
{
    bool isPaused;
    public bool IsPaused { get { return Instance.isPaused; } set { Instance.isPaused = value; } }

    [Header("Player Movement")]
    /// <summary>
    /// The force applied when the player jumps.
    /// </summary>
    [SerializeField] float jumpForce;
    public float JumpForce => jumpForce;

    /// <summary>
    /// The acceleration applied to the player when moving on the ground.
    /// </summary>
    [SerializeField] float walkAcceleration;
    public float WalkAcceleration => walkAcceleration;

    /// <summary>
    /// The acceleration applied to the player when moving in the air.
    /// </summary>
    [SerializeField] float airAcceleration;
    public float AirAcceleration => airAcceleration;

    /// <summary>
    /// The maximum speed the player can reach while only walking
    /// </summary>
    [SerializeField] float groundMaxHorizSpeed;
    public float GroundMaxHorizSpeed => groundMaxHorizSpeed;

    [Header("Player Boost")]
    /// <summary>
    /// The upwards force applied to the player when boosting.
    /// </summary>
    [SerializeField] float boostForce;
    public float BoostForce => boostForce;

    /// <summary>
    /// The horizontal force applied to the player when pressing WASD when boosting.
    /// </summary>
    [SerializeField] float boostAcceleration;
    public float BoostAcceleration => boostAcceleration;

    /// <summary>
    /// The amount of boost fuel spent per second while using boost. max fuel is 100.
    /// </summary>
    [SerializeField] float boostFuelUse;
    public float BoostFuelUse => boostFuelUse;

    /// <summary>
    /// The regen rate for boost fuel while grounded. max fuel is 100.
    /// </summary>
    [SerializeField] float boostFuelRegen;
    public float BoostFuelRegen => boostFuelRegen;

    [Header("Player Dash")]
    /// <summary>
    /// How long it takes for the dash to charge
    /// </summary>
    [SerializeField] float dashChargeTime;
    public float DashChargeTime => dashChargeTime;

    /// <summary>
    /// The force applied to the player when they dash
    /// </summary>
    [SerializeField] float dashForce;
    public float DashForce => dashForce;


    [Header("Mouse Settings")]
    /// <summary>
    /// The mouse sensitivity in the x axis.
    /// </summary>
    [SerializeField] float xMouseSensitivity;
    public float XMouseSensitivity => xMouseSensitivity;

    /// <summary>
    /// The mouse sensitivity in the y axis.
    /// </summary>
    [SerializeField] float yMouseSensitivity;
    public float YMouseSensitivity => yMouseSensitivity;

    [Header("Grapple Settings")]
    /// <summary>
    /// The force applied to the player every reel-in instance.
    /// </summary>
    [SerializeField] float reelInForce;
    public float ReelInForce => reelInForce;
    /// <summary>
    /// The speed at which the grapple head is launched from the gun
    /// </summary>
    [SerializeField] float grappleLaunchSpeed;
    public float GrappleLaunchSpeed => grappleLaunchSpeed;
    /// <summary>
    /// The speed at which the grapple head returns to the gun.
    /// </summary>
    [SerializeField] float grappleReturnSpeed;
    public float GrappleReturnSpeed => grappleReturnSpeed;
    /// <summary>
    /// the range at which the head snaps to the start pos when returning
    /// </summary>
    [SerializeField] float grappleReturnRadius;
    public float GrappleReturnRadius => grappleReturnRadius;
    /// <summary>
    /// the maximum range of the grapple
    /// </summary>
    [SerializeField] float grappleMaxDistance;
    public float GrappleMaxDistance => grappleMaxDistance;

    [Header("Bird Settings")]
    /// <summary>
    /// The force applied to the player when detaching from a bird.
    /// </summary>
    [SerializeField] float birdLaunchForce;
    public float BirdLaunchForce => birdLaunchForce;

    /// <summary>
    /// The distance from the bird the player must be before launching.
    /// </summary>
    [SerializeField] float birdLaunchRadius;
    public float BirdLaunchRadius => birdLaunchRadius;

    /// <summary>
    /// If attached to a bird for longer than this, stop grappling to avoid softlock.
    /// </summary>
    [SerializeField] float birdAutoDetatchTime;
    public float BirdAutoDetatchTime => birdAutoDetatchTime;


    public static GVar Instance { get; private set; }


    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject); // Only keep the first instance
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject); // Keep across scenes
    }

}
