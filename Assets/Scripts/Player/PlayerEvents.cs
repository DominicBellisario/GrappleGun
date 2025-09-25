using System.Collections;
using UnityEngine;

public class PlayerEvents : MonoBehaviour
{
    [SerializeField] FadeOut fadePanel;
    [SerializeField] DamageBorder damagePanel;
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
        if (gvar.CurrentCheckpoint != Vector3.zero) { StartCoroutine(Respawn()); }
    }

    // player hits a death plain, reset them
    public void OutOfBounds()
    {
        //fade to black
        fadePanel.Fade(0f, 1f, fadeTime);
        //wait, then reload the scene
        StartCoroutine(Helper.DoThisAfterDelay(fadeTime, () => sceneHelper.ReloadScene()));
    }

    public void DecreaseHealth(int healthChange)
    {
        // dont do anything if the player is invulnerable
        if (invulnerable) return;

        // make player invulnerable for a bit
        invulnerable = true;
        StartCoroutine(Helper.DoThisAfterDelay(playerInvulnTime, () => invulnerable = false));

        // Ui stuff
        damagePanel.PlayDamageEffect();

        // change player health and check if they die
        health -= healthChange;
        if (health <= 0) OutOfBounds();

        // start regening health
        StopCoroutine(regenHealth);
        regenHealth = RegenHealth();
        StartCoroutine(regenHealth);
    }

    private IEnumerator Respawn()
    {
        yield return new WaitForEndOfFrame();
        transform.position = gvar.CurrentCheckpoint;
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
