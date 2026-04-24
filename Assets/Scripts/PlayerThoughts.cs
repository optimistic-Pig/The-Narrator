using System.Collections;
using UnityEngine;
using TMPro;

/// <summary>
/// Displays the player's inner-monologue thoughts as a subtle bottom-of-screen
/// overlay while in the 3D office world.
///
/// SETUP:
///   1. Add this component to a GameObject in the scene (e.g. on Managers, or
///      as its own "Thoughts" object).
///   2. Create a UI panel in your Canvas called "ThoughtsPanel":
///        Canvas
///        └─ ThoughtsPanel          ← assign to thoughtPanel below
///            ├─ Image              (semi-transparent dark background, optional)
///            └─ ThoughtsText (TMP) ← assign to thoughtText below
///      Suggested anchor: bottom-center, ~900 px wide, ~70 px tall,
///      ~30 px above screen bottom.
///      Add a CanvasGroup component to ThoughtsPanel.
///   3. Assign thoughtPanel and thoughtText in the Inspector.
///   4. Wire PlayerController.playerThoughts to this component.
///
/// USAGE:
///   PlayerThoughts.Instance.ShowThought("Text here.");
///   PlayerThoughts.Instance.ShowThought("Urgent text!", duration: 6f);
/// </summary>
public class PlayerThoughts : MonoBehaviour
{
    // =====================================================================
    // SINGLETON
    // =====================================================================

    public static PlayerThoughts Instance { get; private set; }

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(this); return; }
        Instance = this;
    }

    // =====================================================================
    // INSPECTOR REFERENCES
    // =====================================================================

    [Header("UI")]
    public GameObject     thoughtPanel;
    public TextMeshProUGUI thoughtText;

    [Header("Timing")]
    [Tooltip("How long the thought stays fully visible before fading.")]
    public float displayDuration = 4f;
    [Tooltip("How long the fade-out takes.")]
    public float fadeDuration    = 0.6f;

    // =====================================================================
    // PRIVATE STATE
    // =====================================================================

    private CanvasGroup  _group;
    private Coroutine    _showRoutine;
    private bool         _officeMode = true;  // only show while in 3D world

    // =====================================================================
    // UNITY LIFECYCLE
    // =====================================================================

    void Start()
    {
        if (thoughtPanel == null) return;

        _group = thoughtPanel.GetComponent<CanvasGroup>();
        if (_group == null) _group = thoughtPanel.AddComponent<CanvasGroup>();

        // Configure the text component for a clean single-line caption style:
        // auto-size font to fill the panel width without wrapping.
        if (thoughtText != null)
        {
            thoughtText.enableWordWrapping  = false;
            thoughtText.overflowMode        = TMPro.TextOverflowModes.Ellipsis;
            thoughtText.enableAutoSizing    = true;
            thoughtText.fontSizeMin         = 12f;
            thoughtText.fontSizeMax         = 22f;
            thoughtText.alignment           = TMPro.TextAlignmentOptions.Center;
        }

        thoughtPanel.SetActive(false);
    }

    // =====================================================================
    // PUBLIC API
    // =====================================================================

    /// <summary>
    /// Show a thought. If another thought is already showing, it is replaced.
    /// Only shows when the player is in office (3D world) mode.
    /// </summary>
    public void ShowThought(string message, float duration = -1f)
    {
        if (thoughtPanel == null || thoughtText == null) return;
        if (!_officeMode) return;

        float dur = duration > 0f ? duration : displayDuration;

        if (_showRoutine != null) StopCoroutine(_showRoutine);
        thoughtText.text = message;
        thoughtPanel.SetActive(true);
        if (_group != null) _group.alpha = 1f;
        _showRoutine = StartCoroutine(HideAfterDelay(dur));
    }

    /// <summary>
    /// Hide the current thought immediately (no fade).
    /// </summary>
    public void HideThought()
    {
        if (_showRoutine != null) StopCoroutine(_showRoutine);
        if (thoughtPanel != null) thoughtPanel.SetActive(false);
    }

    /// <summary>
    /// Called by PlayerController to gate thoughts:
    ///   true  = player is in the 3D office world (show thoughts)
    ///   false = player is in an interview panel  (hide thoughts)
    /// </summary>
    public void SetOfficeMode(bool inOffice)
    {
        _officeMode = inOffice;
        if (!inOffice) HideThought();
    }

    // =====================================================================
    // COROUTINE
    // =====================================================================

    private System.Collections.IEnumerator HideAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        if (_group != null)
        {
            float t = 0f;
            while (t < fadeDuration)
            {
                _group.alpha = 1f - (t / fadeDuration);
                t += Time.deltaTime;
                yield return null;
            }
            _group.alpha = 0f;
        }

        if (thoughtPanel != null) thoughtPanel.SetActive(false);
        _showRoutine = null;
    }
}
