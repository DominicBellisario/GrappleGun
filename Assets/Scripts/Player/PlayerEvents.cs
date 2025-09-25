using System.Collections;
using UnityEngine;

public class PlayerEvents : MonoBehaviour
{
    [SerializeField] FadeOut fadePanel;
    [SerializeField] DamageBorder damagePanel;
    [SerializeField] float fadeTime;
    [SerializeField] int playerHealth;
    int maxHealth;
    [SerializeField] float playerHealthRegenTime;
    public int PlayerHealth {  get { return playerHealth; } }
    [SerializeField] int playerInvulnTime;
    bool invulnerable;

    GVar gvar;
    SceneHelper sceneHelper;

    void Start()
    {
        gvar = GVar.Instance;
        sceneHelper = SceneHelper.Instance;
        invulnerable = false;
        maxHealth = playerHealth;

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

    public void ChangeHealth(int healthChange)
    {
        // dont do anything if the player is harmed while invulnerable
        if (invulnerable && healthChange <= 0) return;

        // make player invulnerable for a bit
        invulnerable = true;
        StartCoroutine(Helper.DoThisAfterDelay(playerInvulnTime, () => invulnerable = false));

        // Ui stuff
        damagePanel.PlayDamageEffect();

        // change player health and check if they die
        playerHealth += healthChange;
        if (playerHealth <= 0) OutOfBounds();

        // start regen health
        if (playerHealth < maxHealth) { StartCoroutine(RegenHealth()); }
    }

    private IEnumerator Respawn()
    {
        yield return new WaitForEndOfFrame();
        transform.position = gvar.CurrentCheckpoint;
    }

    private IEnumerator RegenHealth()
    {
        yield return new WaitForSeconds(playerHealthRegenTime);

    }
}
