using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

[RequireComponent(typeof(CanvasGroup))]
public class Pause : MonoBehaviour
{
    [SerializeField] GrappleHead grappleHead;
    [SerializeField] CanvasGroup options;
    [SerializeField] CanvasGroup control;

    [SerializeField] float fadeLength;

    CanvasGroup pausePanel;
    GVar gvar;

    void Start()
    {
        gvar = GVar.Instance;
        pausePanel = GetComponent<CanvasGroup>();
    }

    void OnPause(InputValue inputValue)
    {
        if (inputValue.isPressed) { PauseGame(); }
    }

    public void PauseGame()
    {
        StopAllCoroutines();
        gvar.IsPaused = !gvar.IsPaused;

        // activate pause menu, close other menus if active, and stop time
        if (gvar.IsPaused)
        {
            pausePanel.interactable = true;
            Time.timeScale = 0f;
            AudioListener.pause = true;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            options.alpha = 0f;
            options.interactable = false;
            options.blocksRaycasts = false;
            control.alpha = 0f;
            control.interactable = false;
            control.blocksRaycasts = false;

            StartCoroutine(PauseFade(1f, fadeLength));

        }
        // deactivate menu and start time
        else
        {
            pausePanel.interactable = false;
            Time.timeScale = 1f;
            AudioListener.pause = false;
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            StartCoroutine(PauseFade(0f, fadeLength));
            // prevents grapple from staying mid-air if paused while traveling
            grappleHead.StartCoroutine(grappleHead.ReturnToGun());
        }
    }

    IEnumerator PauseFade(float endAlpha, float effectTime)
    {
        float t = 0f;
        float startAlpha = pausePanel.alpha;
        while (t < effectTime)
        {
            t += Time.unscaledDeltaTime;
            pausePanel.alpha = Mathf.Lerp(startAlpha, endAlpha, t / effectTime);
            yield return null;
        }
    }
}
