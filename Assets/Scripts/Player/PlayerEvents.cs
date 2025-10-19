using System;
using System.Collections;
using UnityEngine;

public class PlayerEvents : MonoBehaviour
{
    // --- EVENTS --- 
    public static event Action OnPlayerDie;
    public static event Action OnPlayerOutOfBounds;
    public static event Action OnPlayerDecreaseHealth;
    public static event Action<float> OnPlayerRespawn;
    [SerializeField] float fadeTime;
    [SerializeField] int health;
    int maxHealth;
    [SerializeField] float playerHealthRegenTime;
    public int Health {  get { return health; } }
    [SerializeField] int playerInvulnTime;
    bool invulnerable;

    GVar gvar;
    SceneHelper sceneHelper;
    IEnumerator regenHealth;

    void Start()
    {
        gvar = GVar.Instance;
        sceneHelper = SceneHelper.Instance;
        invulnerable = false;
        maxHealth = health;
        regenHealth = RegenHealth();

        //send the player to the current checkpoint
        if (gvar.CurrentCheckpointPos != Vector3.zero) { StartCoroutine(Respawn()); }
    }

    // player hits a death plain, reset them
    public void OutOfBounds()
    {
        // record the time they died
        // fade to black
        // play out of bounds sound
        OnPlayerOutOfBounds?.Invoke();

        // wait, then reload the scene
        StartCoroutine(Helper.DoThisAfterDelay(fadeTime, () => sceneHelper.ReloadScene()));
    }

    // player runs out of health, reset them
    private void NoMoreHealth()
    {
        // record the time they died
        // fade to black
        // play death sound
        OnPlayerDie?.Invoke();

        //wait, then reload the scene
        StartCoroutine(Helper.DoThisAfterDelay(fadeTime, () => sceneHelper.ReloadScene()));
    }

    public void DecreaseHealth(int healthChange)
    {
        // dont do anything if the player is invulnerable
        if (invulnerable) return;

        // make the screen flash red
        OnPlayerDecreaseHealth?.Invoke();

        // make player invulnerable for a bit
        invulnerable = true;
        StartCoroutine(Helper.DoThisAfterDelay(playerInvulnTime, () => invulnerable = false));

        // change player health and check if they die
        health -= healthChange;
        if (health <= 0) NoMoreHealth();

        // start regening health
        StopCoroutine(regenHealth);
        regenHealth = RegenHealth();
        StartCoroutine(regenHealth);
    }

    private IEnumerator Respawn()
    {
        // tp the player to last checkpoint
        yield return new WaitForEndOfFrame();
        transform.position = gvar.CurrentCheckpointPos;

        // start the timer with their last recorded time
        OnPlayerRespawn?.Invoke(gvar.LastRecordedTime);
    }

    private IEnumerator RegenHealth()
    {
        // wait for a bit
        yield return new WaitForSeconds(playerHealthRegenTime);
        // stop loop if max health
        if (health == maxHealth) yield break;
        // add health
        health++;
        // regen more health
        regenHealth = RegenHealth();
        StartCoroutine(regenHealth);
    }
}
