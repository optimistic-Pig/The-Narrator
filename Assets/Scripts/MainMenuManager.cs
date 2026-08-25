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

    [Header("Popups")]
    public GameObject creditsPanel;
    public TextMeshProUGUI creditsPanelText;
    public Button creditsCloseBtn;
    public GameObject optionsPanel;
    public Button optionsCloseBtn;

    // =====================================================================
    // CONTENT
    // =====================================================================

    private const string TAGLINE       = "Truth is shaped by how it's told";
    /*private const string CREDITS_TEXT  =
        "Miles Mattson — Lead Developer\n" +
        "Joseph Ortiz — Narrative Lead, Audio Engineer\n" +
        "Liv Gray — UI/UX Lead, Artist";
    */
    private const string GAME_SCENE    = "SampleScene";

    // =====================================================================
    // STATE
    // =====================================================================

    private bool muted           = false;

    // =====================================================================
    // UNITY LIFECYCLE
    // =====================================================================

    void Start()
    {
        InitializeMenu();
    }

    void OnEnable()
    {
        InitializeMenu();
    }

    private void InitializeMenu()
    {
        AudioManager.Instance?.SwitchMusic(AudioManager.MusicTrack.Menu);

        if (mainPanel != null) mainPanel.SetActive(true);

        // Wire buttons
        if (beginDayBtn    != null) { beginDayBtn.onClick.RemoveListener(OnBeginDay); beginDayBtn.onClick.AddListener(OnBeginDay); }
        if (continueBtn    != null) { continueBtn.onClick.RemoveListener(OnContinue); continueBtn.onClick.AddListener(OnContinue); }
        if (optionsBtn     != null) { optionsBtn.onClick.RemoveListener(OpenOptions); optionsBtn.onClick.AddListener(OpenOptions); }
        if (muteBtn        != null) { muteBtn.onClick.RemoveListener(OnToggleMute); muteBtn.onClick.AddListener(OnToggleMute); }
        if (fullscreenBtn  != null) { fullscreenBtn.onClick.RemoveListener(OnToggleFullscreen); fullscreenBtn.onClick.AddListener(OnToggleFullscreen); }
        if (creditsBtn     != null) { creditsBtn.onClick.RemoveListener(OnToggleCredits); creditsBtn.onClick.AddListener(OnToggleCredits); }

        // Popups start hidden
        if (creditsPanel != null) creditsPanel.SetActive(false);
        if (optionsPanel != null) optionsPanel.SetActive(false);
        //if (creditsPanelText != null) creditsPanelText.text = CREDITS_TEXT;
        if (creditsCloseBtn != null) { creditsCloseBtn.onClick.RemoveListener(OnToggleCredits); creditsCloseBtn.onClick.AddListener(OnToggleCredits); }
        if (optionsCloseBtn != null) { optionsCloseBtn.onClick.RemoveListener(CloseOptions); optionsCloseBtn.onClick.AddListener(CloseOptions); }

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

    public void OpenOptions()
    {
        if (optionsPanel != null) optionsPanel.SetActive(true);
    }
    public void CloseOptions()
    {
        if (optionsPanel != null) optionsPanel.SetActive(false);
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
        if (creditsPanel != null) creditsPanel.SetActive(!creditsPanel.activeSelf);
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