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
///      ~30 px above screen bottom. Anchor the panel's pivot to its bottom
///      edge (pivot.y = 0) — Start() will also force this in code — so that
///      when a long thought grows the panel taller, it grows upward instead
///      of pushing below the screen.
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

    [Header("Sizing")]
    [Tooltip("Panel never shrinks shorter than this, even for a one-word thought.")]
    public float minPanelHeight   = 70f;
    [Tooltip("Extra vertical breathing room (px) added above/below the wrapped text.")]
    public float verticalPadding  = 24f;

    // =====================================================================
    // PRIVATE STATE
    // =====================================================================

    private CanvasGroup   _group;
    private Coroutine     _showRoutine;
    private bool          _officeMode = true;  // only show while in 3D world
    private RectTransform _panelRt;
    private RectTransform _textRt;

    // =====================================================================
    // UNITY LIFECYCLE
    // =====================================================================

    void Start()
    {
        if (thoughtPanel == null) return;

        _group = thoughtPanel.GetComponent<CanvasGroup>();
        if (_group == null) _group = thoughtPanel.AddComponent<CanvasGroup>();

        _panelRt = thoughtPanel.GetComponent<RectTransform>();

        // Word wrap onto multiple lines instead of forcing one line and then
        // ellipsis-cutting whatever doesn't fit. ResizePanelToFitText() below
        // grows the panel to match however many lines that produces.
        if (thoughtText != null)
        {
            _textRt = thoughtText.GetComponent<RectTransform>();

            thoughtText.enableWordWrapping  = true;
            thoughtText.overflowMode        = TMPro.TextOverflowModes.Overflow;
            thoughtText.enableAutoSizing    = true;
            thoughtText.fontSizeMin         = 36f;
            thoughtText.fontSizeMax         = 40f;
            thoughtText.alignment           = TMPro.TextAlignmentOptions.Center;
        }

        // Force the pivot's Y to the bottom edge so growing sizeDelta.y
        // (see ResizePanelToFitText) extends the panel upward, keeping its
        // bottom-of-screen anchor point fixed rather than growing downward
        // off-screen or growing evenly in both directions.
        if (_panelRt != null)
            _panelRt.pivot = new Vector2(_panelRt.pivot.x, 0f);

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
        ResizePanelToFitText();
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
    ///   false = player is in an interview panel, or the game has ended
    ///           (hide thoughts, and block any new ones until re-enabled)
    /// </summary>
    public void SetOfficeMode(bool inOffice)
    {
        _officeMode = inOffice;
        if (!inOffice) HideThought();
    }

    // =====================================================================
    // DYNAMIC PANEL SIZING
    // =====================================================================

    /// <summary>
    /// Grows thoughtPanel's height to fit however many lines the current
    /// message wraps onto at the panel's actual on-screen width, so text is
    /// never cut off with "...". Combined with the pivot fix in Start(), this
    /// growth always extends upward from the panel's fixed bottom edge.
    /// </summary>
    private void ResizePanelToFitText()
    {
        if (_panelRt == null || _textRt == null) return;

        float width = _textRt.rect.width;
        if (width <= 0f) width = _panelRt.rect.width;
        if (width <= 0f) return;   // layout not ready yet — keep current size

        Vector2 preferred = thoughtText.GetPreferredValues(thoughtText.text, width, 0f);
        float newHeight = Mathf.Max(minPanelHeight, preferred.y + verticalPadding);

        var size = _panelRt.sizeDelta;
        _panelRt.sizeDelta = new Vector2(size.x, newHeight);
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
