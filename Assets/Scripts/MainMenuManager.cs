using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Manages the Main Menu scene for The Narrator.
///
/// SCENE SETUP (in the MainMenu scene):
///   1. Create a Canvas (Screen Space - Overlay)
///   2. Add an EventSystem
///   3. Create a GameObject called "MenuManager" and attach this script
///   4. Build the UI panels as described below and wire them in the Inspector
///
/// PANEL HIERARCHY:
///   Canvas
///   ├─ MainPanel          ← root panel, always starts active
///   │   ├─ TitleText      (TMP) "The Narrator"
///   │   ├─ SubtitleText   (TMP) "Truth is shaped by how it's told"
///   │   ├─ BeginDayBtn    → calls OnBeginDay()
///   │   ├─ ContinueBtn    → calls OnContinue()   (greyed out — no save yet)
///   │   ├─ OptionsBtn     → calls OnOptions()
///   │   └─ CreditsBtn     → calls OnCredits()
///   ├─ OptionsPanel       ← starts inactive
///   │   ├─ TitleText      (TMP) "Options"
///   │   ├─ FullscreenToggle (Toggle)
///   │   ├─ SoundToggle    (Toggle)
///   │   └─ BackBtn        → calls OnBack()
///   └─ CreditsPanel       ← starts inactive
///       ├─ TitleText      (TMP) "Credits"
///       ├─ CreditsText    (TMP) — auto-filled from creditLines below
///       └─ BackBtn        → calls OnBack()
///
/// SCENE SETUP IN BUILD SETTINGS:
///   File → Build Settings → add both scenes:
///     Index 0: MainMenu
///     Index 1: SampleScene
/// </summary>
public class MainMenuManager : MonoBehaviour
{
    // =====================================================================
    // INSPECTOR REFERENCES
    // =====================================================================

    [Header("Panels")]
    public GameObject mainPanel;
    public GameObject optionsPanel;
    public GameObject creditsPanel;

    [Header("Main Panel Buttons")]
    public Button beginDayBtn;
    public Button continueBtn;      // greyed out until save system exists
    public Button optionsBtn;
    public Button creditsBtn;

    [Header("Options Panel")]
    public Toggle fullscreenToggle;
    public Toggle soundToggle;

    [Header("Credits Panel")]
    public TextMeshProUGUI creditsBodyText;

    // =====================================================================
    // CREDITS CONTENT  (hardcoded per your team)
    // =====================================================================

    private readonly string[] creditLines = new string[]
    {
        "Olivia Gray",
        "UI / UX Designer",
        "",
        "Miles Mattson",
        "Programming",
        "",
        "Joseph Ortiz",
        "Narrative  ·  Sound",
    };

    // =====================================================================
    // SCENE NAMES
    // =====================================================================

    private const string GAME_SCENE = "SampleScene";

    // =====================================================================
    // UNITY LIFECYCLE
    // =====================================================================

    void Start()
    {
        // Show only main panel
        ShowPanel(mainPanel);

        // Wire buttons (safe even if already wired in Inspector)
        if (beginDayBtn  != null) beginDayBtn .onClick.AddListener(OnBeginDay);
        if (continueBtn  != null) continueBtn .onClick.AddListener(OnContinue);
        if (optionsBtn   != null) optionsBtn  .onClick.AddListener(OnOptions);
        if (creditsBtn   != null) creditsBtn  .onClick.AddListener(OnCredits);

        // Grey out Continue until save system is implemented
        if (continueBtn != null)
        {
            continueBtn.interactable = false;
            var tmp = continueBtn.GetComponentInChildren<TextMeshProUGUI>();
            if (tmp != null) tmp.text = "Continue  (coming soon)";
        }

        // Options: sync toggles to current state
        if (fullscreenToggle != null)
        {
            fullscreenToggle.isOn = Screen.fullScreen;
            fullscreenToggle.onValueChanged.AddListener(OnFullscreenToggle);
        }
        if (soundToggle != null)
        {
            soundToggle.isOn = AudioListener.volume > 0f;
            soundToggle.onValueChanged.AddListener(OnSoundToggle);
        }

        // Credits: build body text from array
        if (creditsBodyText != null)
            creditsBodyText.text = string.Join("\n", creditLines);
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
        // When save is implemented: load save data then LoadScene(GAME_SCENE)
        Debug.Log("[MainMenu] Continue: no save system yet.");
    }

    public void OnOptions()
    {
        ShowPanel(optionsPanel);
    }

    public void OnCredits()
    {
        ShowPanel(creditsPanel);
    }

    public void OnBack()
    {
        ShowPanel(mainPanel);
    }

    // =====================================================================
    // OPTIONS HANDLERS
    // =====================================================================

    private void OnFullscreenToggle(bool value)
    {
        Screen.fullScreen = value;
    }

    private void OnSoundToggle(bool value)
    {
        AudioListener.volume = value ? 1f : 0f;
    }

    // =====================================================================
    // HELPERS
    // =====================================================================

    private void ShowPanel(GameObject target)
    {
        if (mainPanel    != null) mainPanel   .SetActive(mainPanel    == target);
        if (optionsPanel != null) optionsPanel.SetActive(optionsPanel == target);
        if (creditsPanel != null) creditsPanel.SetActive(creditsPanel == target);
    }
}
