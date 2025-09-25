using System.Collections;
using UnityEngine;

public class PlayerEvents : MonoBehaviour
{
    [SerializeField] VisualEffects fadePanel;
    [SerializeField] float fadeTime;
    [SerializeField] int playerHealth;
    [SerializeField] int playerInvulnTime;
    bool invulnerable;

    GVar gvar;
    SceneHelper sceneHelper;

    void Start()
    {
        gvar = GVar.Instance;
        sceneHelper = SceneHelper.Instance;
        invulnerable = false;

        //send the player to the current checkpoint
        if (gvar.CurrentCheckpoint != Vector3.zero) { StartCoroutine(Respawn()); }
    }

    // player hits a death plain, reset them
    public void OutOfBounds()
    {
        StartCoroutine(_OutOfBounds());
    }

    private IEnumerator _OutOfBounds()
    {
        //fade to black
        fadePanel.Fade(0f, 1f, fadeTime);
        yield return new WaitForSeconds(fadeTime + 0.5f);
        //reload the scene
        sceneHelper.ReloadScene();
    }

    public void ChangeHealth(int healthChange)
    {
        if (invulnerable) return;
        invulnerable = true;
        StartCoroutine(Helper.DoThisAfterDelay(playerInvulnTime, () => invulnerable = false));

        playerHealth += healthChange;
        if (playerHealth <= 0) StartCoroutine(_OutOfBounds());
    }
    

    private IEnumerator Respawn()
    {
        yield return new WaitForEndOfFrame();
        transform.position = gvar.CurrentCheckpoint;
    }
}
