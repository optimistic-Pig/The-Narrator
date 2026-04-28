using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Manages the Main Menu scene for The Narrator.
///
/// SCENE SETUP:
///   Canvas
///   └─ MainPanel
///       ├─ TitleText        (TMP) "The Narrator"
///       ├─ SubtitleText     (TMP) tagline / credits display  ← reused
///       ├─ BeginDayBtn      → OnBeginDay()
///       ├─ ContinueBtn      → OnContinue()   (non-interactable)
///       ├─ MuteBtn          → OnToggleMute()
///       ├─ FullscreenBtn    → OnToggleFullscreen()
///       └─ CreditsBtn       → OnToggleCredits()
///
/// No sub-panels. Options are toggle buttons whose labels update in place.
/// Credits swap the subtitle text; clicking again restores the tagline.
/// </summary>
public class MainMenuManager : MonoBehaviour
{
    // =====================================================================
    // INSPECTOR REFERENCES
    // =====================================================================

    [Header("Panel")]
    public GameObject mainPanel;

    [Header("Buttons")]
    public Button beginDayBtn;
    public Button continueBtn;
    public Button optionsBtn;
    public Button muteBtn;
    public Button fullscreenBtn;
    public Button creditsBtn;

    [Header("Text")]
    public TextMeshProUGUI subtitleText;

    // =====================================================================
    // CONTENT
    // =====================================================================

    private const string TAGLINE       = "Truth is shaped by how it's told";
    private const string CREDITS_TEXT  = "Olivia Gray — UI/UX  ·  Miles Mattson — Programming  ·  Joseph Ortiz — Narrative & Sound";
    private const string GAME_SCENE    = "SampleScene";

    // =====================================================================
    // STATE
    // =====================================================================

    private bool showingCredits  = false;
    private bool showingOptions  = false;
    private bool muted           = false;

    // =====================================================================
    // UNITY LIFECYCLE
    // =====================================================================

    void Start()
    {
        AudioManager.Instance?.SwitchMusic(AudioManager.MusicTrack.Menu);

        if (mainPanel != null) mainPanel.SetActive(true);

        // Wire buttons
        if (beginDayBtn    != null) beginDayBtn   .onClick.AddListener(OnBeginDay);
        if (continueBtn    != null) continueBtn   .onClick.AddListener(OnContinue);
        if (optionsBtn     != null) optionsBtn    .onClick.AddListener(OnToggleOptions);
        if (muteBtn        != null) muteBtn       .onClick.AddListener(OnToggleMute);
        if (fullscreenBtn  != null) fullscreenBtn .onClick.AddListener(OnToggleFullscreen);
        if (creditsBtn     != null) creditsBtn    .onClick.AddListener(OnToggleCredits);

        // Hide options buttons until Options is clicked
        if (muteBtn       != null) muteBtn      .gameObject.SetActive(false);
        if (fullscreenBtn != null) fullscreenBtn.gameObject.SetActive(false);

        // Continue is locked until a save system exists
        if (continueBtn != null)
        {
            continueBtn.interactable = false;
            SetButtonLabel(continueBtn, "Continue  (coming soon)");
        }

        // Sync initial toggle button labels to actual state
        muted = AudioListener.volume <= 0f;
        RefreshMuteLabel();
        RefreshFullscreenLabel();

        // Subtitle starts as tagline
        if (subtitleText != null) subtitleText.text = TAGLINE;
    }

    // =====================================================================
    // BUTTON HANDLERS
    // =====================================================================

    public void OnBeginDay()
    {
        SceneManager.LoadScene(GAME_SCENE);
    }

    public void OnContinue()
    {
        // Placeholder — no save system yet
        Debug.Log("[MainMenu] Continue: no save system yet.");
    }

    public void OnToggleOptions()
    {
        showingOptions = !showingOptions;
        if (muteBtn       != null) muteBtn      .gameObject.SetActive(showingOptions);
        if (fullscreenBtn != null) fullscreenBtn.gameObject.SetActive(showingOptions);
    }

    public void OnToggleMute()
    {
        muted = !muted;
        AudioListener.volume = muted ? 0f : 1f;
        RefreshMuteLabel();
    }

    public void OnToggleFullscreen()
    {
        Screen.fullScreen = !Screen.fullScreen;
        RefreshFullscreenLabel();
    }

    public void OnToggleCredits()
    {
        showingCredits = !showingCredits;
        if (subtitleText != null)
            subtitleText.text = showingCredits ? CREDITS_TEXT : TAGLINE;
    }

    // =====================================================================
    // HELPERS
    // =====================================================================

    private void RefreshMuteLabel()
    {
        SetButtonLabel(muteBtn, muted ? "Unmute" : "Mute");
    }

    private void RefreshFullscreenLabel()
    {
        SetButtonLabel(fullscreenBtn, Screen.fullScreen ? "Windowed" : "Fullscreen");
    }

    private void SetButtonLabel(Button btn, string label)
    {
        if (btn == null) return;
        var tmp = btn.GetComponentInChildren<TextMeshProUGUI>();
        if (tmp != null) tmp.text = label;
    }
}
