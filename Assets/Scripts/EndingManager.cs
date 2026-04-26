using UnityEngine;
using TMPro;
using UnityEngine.UI;

/// <summary>
/// Displays the correct ending cutscene / text panel when the game ends.
///
/// SETUP:
///   1. Add this component anywhere in the scene (e.g. on the same
///      GameObject as DialogueManager).
///   2. Create an "Ending Panel" in your Canvas and assign it below.
///   3. Paste the ending text from the design doc into the four text fields.
///   4. Call TriggerEnding(marsOpinionScore) from DialogueManager after the
///      Day 3 article is published.
/// </summary>
public class EndingManager : MonoBehaviour
{
    // =====================================================================
    // UI REFERENCES
    // =====================================================================

    [Header("Ending UI")]
    public GameObject endingPanel;
    public TextMeshProUGUI endingBodyText;
    public TextMeshProUGUI endingTitleText;   // optional — can leave blank
    public Button returnToMenuBtn;

    // =====================================================================
    // ENDING TEXTS  (fill these in the Inspector from the design doc)
    // =====================================================================

    [Header("Pro-Martian Ending  (score > 2)")]
    public string proMartianTitle = "The Truth Teller";
    [TextArea(4, 10)]
    public string proMartianBody =
        "Your work arrives on Earth, with very little concern. As time goes on, " +
        "and as you learn more, so does the public. The public starts to question " +
        "what the narrative they've been given all this time, and slowly, pressure builds up.\n\n" +
        "Just like all news, the public quickly forgets, and they start caring less " +
        "and less about your writing.\n\n" +
        "You remain on Mars, telling the stories as they are.\n\n" +
        "Nothing ever comes of your writing.";

    [Header("Pro-Human Ending  (score < -2)")]
    public string proHumanTitle = "Voice of Earth";
    [TextArea(4, 10)]
    public string proHumanBody =
        "Your work arrives on Earth, the public devouring your content. Outrage ensues, " +
        "with the public urging officials to get involved more on Mars and with people " +
        "praising what it means to be human.\n\n" +
        "As time goes on, Mars becomes a resource.\n\n" +
        "You remain on Mars, telling narratives the way you want.\n\n" +
        "You become a micro-celebrity, endorsed for telling the truth and speaking your mind.";

    [Header("Pro-Self Ending  (-2 to +2)")]
    public string proSelfTitle = "The Opportunist";
    [TextArea(4, 10)]
    public string proSelfBody =
        "Your work arrives on Earth, and major investors aren't impressed. They have known " +
        "for quite some time that Mars isn't what you make it out to be. Fortunately for you, " +
        "the normal citizen doesn't, so some investments trickle in.\n\n" +
        "As time goes on, you make some decent money, making a small empire on Mars.\n\n" +
        "You remain on Mars, attempting to get more investments.\n\n" +
        "As time goes on, and as your promises start to fail, you slowly lose everything.";

    [Header("Secret Ending  (Kortnara + Gorp path)")]
    public string secretTitle = "Everything Is A Lie";
    [TextArea(4, 10)]
    public string secretBody =
        "Your work arrives on Earth, with lots of concern at first. As time goes on, " +
        "and as you learn more, so does the public. The public starts to question what " +
        "the narrative they've been given all this time, and slowly, pressure builds up. " +
        "They are outraged, and demand answers.\n\n" +
        "Just like all news, the topic gets covered up, and the public slowly loses interest. " +
        "But something is different this time. The public doesn't completely forget, and they " +
        "start a movement, demanding answers.\n\n" +
        "You remain on Mars, trying to uncover the truth, where you unfortunately meet your end.\n\n" +
        "Your journalism, after many years, finally exposes some of what happens on Mars.";

    // =====================================================================
    // PUBLIC API
    // =====================================================================

    /// <summary>
    /// Call this from DialogueManager immediately after the Day 3 article
    /// is published (i.e. in the Publish button handler when currentDay == 3).
    /// </summary>
    public void TriggerEnding(int finalScore)
    {
        if (GameStateManager.Instance == null)
        {
            Debug.LogError("[EndingManager] GameStateManager not found!");
            return;
        }

        GameStateManager.EndingType type = GameStateManager.Instance.GetEnding(finalScore);

        string title = "";
        string body  = "";

        switch (type)
        {
            case GameStateManager.EndingType.ProMartian:
                title = proMartianTitle;
                body  = proMartianBody;
                break;
            case GameStateManager.EndingType.ProHuman:
                title = proHumanTitle;
                body  = proHumanBody;
                break;
            case GameStateManager.EndingType.ProSelf:
                title = proSelfTitle;
                body  = proSelfBody;
                break;
            case GameStateManager.EndingType.Secret:
                title = secretTitle;
                body  = secretBody;
                break;
        }

        if (endingTitleText != null) endingTitleText.text = title;
        if (endingBodyText  != null) endingBodyText.text  = body;
        if (endingPanel     != null) endingPanel.SetActive(true);
        if (returnToMenuBtn != null)
        {
            returnToMenuBtn.gameObject.SetActive(true);
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
            returnToMenuBtn.onClick.RemoveAllListeners();
            returnToMenuBtn.onClick.AddListener(() => 
                UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu"));
        }
        var pm = FindObjectOfType<PlayerMovement>();
        if (pm != null) pm.enabled = false;

        // Hide crosshair so it doesn't float over the ending screen
        var crosshair = GameObject.Find("CrosshairUI");
        if (crosshair != null) crosshair.SetActive(false);

        // Fix ending panel layout: push text down so nothing is cut off at top
        if (endingPanel != null)
        {
            // Solid background — add Image if missing
            var bg = endingPanel.GetComponent<UnityEngine.UI.Image>();
            if (bg == null) bg = endingPanel.AddComponent<UnityEngine.UI.Image>();
            bg.color = new Color(0.06f, 0.06f, 0.10f, 1f); // dark navy, fully opaque

            // Stretch panel to fill canvas
            var panelRt = endingPanel.GetComponent<RectTransform>();
            if (panelRt != null)
            {
                panelRt.anchorMin = Vector2.zero;
                panelRt.anchorMax = Vector2.one;
                panelRt.offsetMin = Vector2.zero;
                panelRt.offsetMax = Vector2.zero;
            }

            // Title: top band
            if (endingTitleText != null)
            {
                var rt = endingTitleText.GetComponent<RectTransform>();
                if (rt != null)
                {
                    rt.anchorMin = new Vector2(0.05f, 0.80f);
                    rt.anchorMax = new Vector2(0.95f, 0.95f);
                    rt.offsetMin = Vector2.zero; rt.offsetMax = Vector2.zero;
                }
                endingTitleText.enableWordWrapping = true;
                endingTitleText.alignment = TMPro.TextAlignmentOptions.Center;
                endingTitleText.fontSize = 48f;
            }

            // Body: middle band
            if (endingBodyText != null)
            {
                var rt = endingBodyText.GetComponent<RectTransform>();
                if (rt != null)
                {
                    rt.anchorMin = new Vector2(0.05f, 0.25f);
                    rt.anchorMax = new Vector2(0.95f, 0.77f);
                    rt.offsetMin = Vector2.zero; rt.offsetMax = Vector2.zero;
                }
                endingBodyText.enableWordWrapping = true;
                endingBodyText.alignment = TMPro.TextAlignmentOptions.TopLeft;
                endingBodyText.fontSize = 28f;
            }

            // Return button: bottom band
            if (returnToMenuBtn != null)
            {
                var rt = returnToMenuBtn.GetComponent<RectTransform>();
                if (rt != null)
                {
                    rt.anchorMin = new Vector2(0.35f, 0.08f);
                    rt.anchorMax = new Vector2(0.65f, 0.18f);
                    rt.offsetMin = Vector2.zero; rt.offsetMax = Vector2.zero;
                }
            }
        }

        Debug.Log($"[EndingManager] Triggering ending: {type}  (score: {finalScore})");
    }
}
