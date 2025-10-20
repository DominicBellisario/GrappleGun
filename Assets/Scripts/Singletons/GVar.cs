using UnityEngine;
using UnityEngine.Audio;

public class GVar : MonoBehaviour
{
    // GAME SETTINGS / STATES

    /// <summary>
    /// wether or not the game is paused
    /// </summary>
    public bool IsPaused { get; set; }

    /// <summary>
    /// the current checkpoint the player will spawn at if they die
    /// </summary>
    public Vector3 CurrentCheckpointPos { get; set; }
    /// <summary>
    /// the current rotation the player will spawn at if they die
    /// </summary>
    public Vector3 CurrentCheckpointRotation { get; set; }

    public void ResetCheckpoint()
    {
        Instance.CurrentCheckpointPos = Vector3.zero;
        Instance.CurrentCheckpointRotation = Vector3.zero;
    }

    /// <summary>
    /// the time recorded when the scene was last reset
    /// </summary>
    float lastRecordedTime;
    public float LastRecordedTime
    {
        get { return Instance.lastRecordedTime; }
        set
        {
            lastRecordedTime = value;
            PlayerPrefs.SetFloat("Last Recorded Time", lastRecordedTime);
        }
    }

    // PLAYER SETTINGS: ALL TAKEN FROM PLAYERPREFS
    float mouseSensitivity;
    public float MouseSensitivity
    {
        get { return Instance.mouseSensitivity; }
        set
        {
            mouseSensitivity = value;
            PlayerPrefs.SetFloat("Mouse Sensitivity", mouseSensitivity);
        }
    }


    [SerializeField] AudioMixer masterMixer;
    [SerializeField] GameObject audioSourcePrefab;
    public GameObject AudioSourcePrefab { get { return Instance.audioSourcePrefab; } }
    float masterVolume;
    public float MasterVolume
    {
        get { return Instance.masterVolume; }
        set
        {
            masterVolume = value;
            PlayerPrefs.SetFloat("Master Volume", masterVolume);
            masterMixer.SetFloat("Master Volume", masterVolume);
        }
    }

    /// ------------- CURRENT PLAYER STATS -------------
    /// <summary>
    /// the current distance between the grapple head and the launcher
    /// </summary>
    public float CurrentRopeLength { get; set; }
    /// <summary>
    /// the current amount of boost fuel the player has
    /// </summary>
    public float CurrentBoostFuel { get; set; }
    /// <summary>
    /// the current dash charge the player has
    /// </summary>
    public float CurrentDashCharge { get; set; }
    /// <summary>
    /// whether or not the player can dash
    /// </summary>
    public bool CanDash { get; set; }
    /// <summary>
    /// the current health the player has
    /// </summary>
    public int CurrentHealth { get; set; }
    /// <summary>
    /// the player's rigidbody
    /// </summary>
    public Rigidbody PlayerRb { get; set; }


    [SerializeField] int maxHealth;
    public int MaxHealth => maxHealth;

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

    [Header("Reel Settings")]
    /// <summary>
    /// The force applied to the player when detaching from a bird.
    /// </summary>
    [SerializeField] float reelLaunchForce;
    public float ReelLaunchForce => reelLaunchForce;

    /// <summary>
    /// The distance from the bird the player must be before launching.
    /// </summary>
    [SerializeField] float reelLaunchRadius;
    public float ReelLaunchRadius => reelLaunchRadius;

    /// <summary>
    /// If attached to a bird for longer than this, stop grappling to avoid softlock.
    /// </summary>
    [SerializeField] float reelAutoDetatchTime;
    public float ReelAutoDetatchTime => reelAutoDetatchTime;

    [Header("Bird Settings")]
    /// <summary>
    /// the speed at which the elasticity of the grapple increases when attached to a bird
    /// </summary>
    [SerializeField] float birdElasticityIncreaseSpeed;
    public float BirdElasticityIncreaseSpeed => birdElasticityIncreaseSpeed;
    /// <summary>
    /// the max speed the player can leave the bird with
    /// </summary>
    [SerializeField] float birdMaxVelocity;
    public float BirdLaunchSpeed => birdMaxVelocity;

    [Header("Gun Settings")]
    /// <summary>
    /// The time in seconds it takes for the gun to reload
    /// </summary>
    [SerializeField] float gunReloadTime;
    public float GunReloadTime => gunReloadTime;

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

    void Start()
    {
        mouseSensitivity = PlayerPrefs.GetFloat("Mouse Sensitivity", 0.25f);
        masterVolume = PlayerPrefs.GetFloat("Master Volume", -20f);
        masterMixer.SetFloat("Master Volume", masterVolume);
    }

}
