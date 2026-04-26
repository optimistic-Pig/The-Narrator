using UnityEngine;

/// <summary>
/// Attach to the wall panel / control console GameObject in the office.
/// Tag the object "SecretConsole" so PlayerController can hit it with a raycast.
///
/// SETUP:
///   1. Create a Quad (or use an existing wall section) at the panel location.
///   2. Apply a material with your control-panel texture.
///   3. Tag it "SecretConsole" (add to Tags in Project Settings first).
///   4. Attach this script.
///   5. Assign secretBaseSpawnPoint — an empty Transform placed in the
///      secret base area of your scene (where the player should appear).
///   6. Assign playerTransform — drag the Player GameObject here.
///
/// HOW IT WORKS:
///   • PlayerController.cs checks the two GameStateManager flags.
///   • If both are set it calls console.Activate(playerController).
///   • Activate() fades out, teleports, fades in (optional) or just moves
///     the player directly if you prefer a hard cut.
/// </summary>
public class SecretConsole : MonoBehaviour
{
    [Header("Teleport Target")]
    [Tooltip("Empty Transform in the secret base where the player will appear.")]
    public Transform secretBaseSpawnPoint;

    [Tooltip("The Player GameObject (has CharacterController or Rigidbody).")]
    public Transform playerTransform;

    [Header("Optional Fade")]
    [Tooltip("If assigned, this CanvasGroup will be used to fade to black before teleporting.")]
    public CanvasGroup fadeGroup;
    [Tooltip("Seconds for the fade-out and fade-in.")]
    public float fadeDuration = 0.5f;

    /// <summary>Called by PlayerController when the player has both flags and clicks the console.</summary>
    public void Activate(MonoBehaviour caller)
    {
        if (secretBaseSpawnPoint == null)
        {
            Debug.LogError("[SecretConsole] secretBaseSpawnPoint is not assigned!");
            return;
        }
        if (playerTransform == null)
        {
            Debug.LogError("[SecretConsole] playerTransform is not assigned!");
            return;
        }

        caller.StartCoroutine(TeleportRoutine());
    }

    private System.Collections.IEnumerator TeleportRoutine()
    {
        // ── Fade out ──────────────────────────────────────────────────────
        if (fadeGroup != null)
        {
            float t = 0f;
            while (t < fadeDuration)
            {
                fadeGroup.alpha = t / fadeDuration;
                t += Time.deltaTime;
                yield return null;
            }
            fadeGroup.alpha = 1f;
        }

        // ── Teleport ──────────────────────────────────────────────────────
        // CharacterController must be disabled before moving, otherwise
        // Unity snaps the player back.
        var cc = playerTransform.GetComponent<CharacterController>();
        if (cc != null) cc.enabled = false;

        playerTransform.position = secretBaseSpawnPoint.position;
        playerTransform.rotation = secretBaseSpawnPoint.rotation;

        if (cc != null) cc.enabled = true;

        // Short pause so the player registers the new location
        yield return new WaitForSeconds(0.1f);

        // ── Fade in ───────────────────────────────────────────────────────
        if (fadeGroup != null)
        {
            float t = fadeDuration;
            while (t > 0f)
            {
                fadeGroup.alpha = t / fadeDuration;
                t -= Time.deltaTime;
                yield return null;
            }
            fadeGroup.alpha = 0f;
        }
    }
}
