using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Character-agnostic dialogue UI controller.
/// Handles Canvas panels, phase flow, translation, dictionary, headlines,
/// paragraph-by-paragraph article writing, scoring, and day management.
///
/// CHANGES FROM ORIGINAL:
///   1. StartInterview()      → notifies GameStateManager.OnInterviewStarted()
///   2. OptionClicked()       → notifies GameStateManager.OnInterviewComplete()
///                              when EndTransition fires
///   3. ShowArticleComplete() → "Publish" click notifies GameStateManager.OnArticlePublished()
///                              and fires EndingManager on Day 3
///   4. ShowDesk()            → new public method; called by PlayerController
///                              when the player clicks the writing desk
///   5. EndOfDayContinue()    → now driven by GameStateManager.CurrentDay
///                              instead of a local counter
/// </summary>
public class DialogueManager : MonoBehaviour
{
    // =====================================================================
    // UI REFERENCES
    // =====================================================================

    [Header("Dialogue Panel")]
    public TextMeshProUGUI mainText;
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI triesText;
    public ScrollRect dialogueScroll;

    [Header("Option Buttons (TMP text ON the Button object)")]
    public TextMeshProUGUI optionOne;
    public TextMeshProUGUI optionTwo;
    public TextMeshProUGUI optionThree;
    public TextMeshProUGUI optionFour;

    [Header("Dictionary Panel")]
    public GameObject dictionaryPanel;
    public TextMeshProUGUI dictionaryLookUpsText;
    public TextMeshProUGUI[] dictionarySlots;

    [Header("Dictionary Right Panel")]
    public GameObject dictDefaultMsg;        // DictDefaultMsg  — "No Entry Selected"
    public GameObject dictSelectedView;      // DictSelectedView — shown when entry is selected
    public GameObject dictUndiscoveredMsg;   // DictUndiscoveredMsg — shown for locked entries
    public TextMeshProUGUI dictKlingonText;  // DictKlingonText
    public TextMeshProUGUI dictEnglishText;  // DictEnglishText
    public TextMeshProUGUI dictContextText;  // DictContextText
    public TextMeshProUGUI dictStatusText;   // DictStatusText
    public TextMeshProUGUI dictNotesText;    // DictNotesText

    [Header("Screen Panels")]
    public GameObject briefingPanel;
    public GameObject interviewPanel;
    public GameObject summaryPanel;
    public GameObject endOfDayPanel;
    public TextMeshProUGUI endOfDaySummaryText;
    public UnityEngine.UI.Image briefingPortrait;   // portrait Image on the briefing panel

    [Header("Interviews")]
    public InterviewBase[] availableInterviews;

    // ─── NEW: Ending Manager reference ───────────────────────────────────
    [Header("Ending")]
    public EndingManager endingManager;

    // ─── NEW: Player reference (for auto-return after interview ends) ─────
    [Header("Player")]
    public PlayerMovement playerMovement;

    [Header("Buttons to manage visibility")]
    [Tooltip("ReturnToOfficeBtnBtn — hidden during headline/article phases. Auto-found if unassigned.")]
    public GameObject returnToOfficeBtn;
    [Tooltip("DictionaryToggleBtn — repositioned to not overlap status text. Auto-found if unassigned.")]
    public GameObject dictionaryToggleBtn;

    // =====================================================================
    // SCORING
    // =====================================================================

    [HideInInspector] public int marsOpinionScore = 0;
    [HideInInspector] public int truthfulCount    = 0;
    [HideInInspector] public int dishonestCount   = 0;
    [HideInInspector] public int ambitiousCount   = 0;

    // =====================================================================
    // DAY MANAGEMENT
    // =====================================================================

    /// <summary>
    /// Read-only accessor — source of truth is now GameStateManager.CurrentDay.
    /// Kept for backward-compat with any existing UI references.
    /// </summary>
    public int currentDay => GameStateManager.Instance != null
                           ? GameStateManager.Instance.CurrentDay : _localDay;
    private int _localDay = 1;

    public int totalDays = 3;

    // =====================================================================
    // PRIVATE STATE
    // =====================================================================

    private enum Phase { Dialogue, EndTransition, Headline, ArticleWriting, ArticleComplete, DayEnd }
    private Phase currentPhase = Phase.Dialogue;

    private InterviewBase current;
    private int dictionaryLookUps = 0;
    private float articleChosen = 0f;
    private List<InterviewBase> completedToday = new List<InterviewBase>();

    // Article writing state
    private InterviewBase.ArticleTemplate currentArticle;
    private int currentParagraphIndex = 0;
    private readonly List<string> articleLines = new List<string>();

    // Headline slot mapping: button position (0-3) → actual Headlines[] index
    private readonly int[] headlineSlotToIndex = new int[4] { -1, -1, -1, -1 };

    private InterviewBase pendingInterview;

    private GameObject optionsGroup;   // parent transform shared by all 4 option buttons
    private float _baseOptionFontSize = -1f;  // captured once from optionOne; 100% baseline for shrink-to-fit

    private string rawMainText = "";
    private string rawOpt1 = "";
    private string rawOpt2 = "";
    private string rawOpt3 = "";
    private string rawOpt4 = "";

    // Snapshot of option-button active states when an interview is paused mid-dialogue
    private bool _snapOpt1, _snapOpt2, _snapOpt3, _snapOpt4;

    // =====================================================================
    // AWAKE  —  runs before Start() on every script
    // =====================================================================

    void Awake()
    {
        // Panels may be left active in the saved scene from a previous
        // play-mode session or editor edit.  Hiding them in Awake() ensures
        // they are gone from frame 0, before any other script's Start() runs
        // and before the first render — preventing the UI from blocking NPC clicks.
        if (interviewPanel  != null) interviewPanel.SetActive(false);
        if (briefingPanel   != null) briefingPanel.SetActive(false);
        if (summaryPanel    != null) summaryPanel.SetActive(false);
        if (endOfDayPanel   != null) endOfDayPanel.SetActive(false);
        if (dictionaryPanel != null) dictionaryPanel.SetActive(false);
    }

    // =====================================================================
    // START
    // =====================================================================

    public void Start()
    {
        HideAllPanels();

        // ── Auto-find EndingManager if not assigned ───────────────────────
        if (endingManager == null)
            endingManager = FindObjectOfType<EndingManager>();

        // ── Auto-find ReturnToOfficeBtn if not assigned in Inspector ──────
        if (returnToOfficeBtn == null && interviewPanel != null)
        {
            var t = interviewPanel.transform.Find("ReturnToOfficeBtnBtn")
                 ?? interviewPanel.transform.Find("ReturnToOfficeBtn");
            if (t != null) returnToOfficeBtn = t.gameObject;
        }

        // ── Auto-find DictionaryToggleBtn if not assigned ─────────────────
        if (dictionaryToggleBtn == null && interviewPanel != null)
        {
            var t = interviewPanel.transform.Find("DictionaryToggleBtn");
            if (t != null) dictionaryToggleBtn = t.gameObject;
        }
        // DictionaryToggleBtn and triesText positions are set in the Inspector.

        // ── Responsive font sizing ────────────────────────────────────────
        ApplyResponsiveFontSizes();

        // ── Runtime panel layout fix (deferred one frame) ─────────────────
        // Running layout fixes on the same frame as Start() can be overridden
        // by Unity's own Canvas layout pass. Deferring to the next frame lets
        // us win that race.
        StartCoroutine(ApplyLayoutNextFrame());
    }

    private System.Collections.IEnumerator ApplyLayoutNextFrame()
    {
        yield return null;   // wait one frame for Canvas to initialise
        FixPanelLayout();
        Canvas.ForceUpdateCanvases();
    }

    // =====================================================================
    // RUNTIME LAYOUT FIX
    // =====================================================================

    private void FixPanelLayout()
    {
        // ── 1. All overlay panels fill the canvas ─────────────────────────
        ForceStretch(interviewPanel);
        ForceStretch(briefingPanel);
        ForceStretch(summaryPanel);
        ForceStretch(endOfDayPanel);
        // ── EndingPanel: solid background + lay out title and body text ──
        if (endingManager != null && endingManager.endingPanel != null)
        {
            var ep = endingManager.endingPanel;
            ForceStretch(ep);

            // Without an opaque Image, the 3D world shows through the panel.
            var epImg = ep.GetComponent<UnityEngine.UI.Image>();
            if (epImg == null) epImg = ep.AddComponent<UnityEngine.UI.Image>();
            if (epImg.color.a < 0.5f)
                epImg.color = new Color(0.08f, 0.08f, 0.13f, 0.97f); // dark navy

            // Title: upper band (78 %–95 % of panel height)
            if (endingManager.endingTitleText != null)
            {
                var rt = endingManager.endingTitleText.GetComponent<RectTransform>();
                if (rt != null)
                {
                    rt.anchorMin = new Vector2(0.08f, 0.78f);
                    rt.anchorMax = new Vector2(0.92f, 0.95f);
                    rt.offsetMin = Vector2.zero;
                    rt.offsetMax = Vector2.zero;
                }
                endingManager.endingTitleText.enableWordWrapping = true;
                endingManager.endingTitleText.alignment =
                    TMPro.TextAlignmentOptions.Center;
            }

            // Body: main area (10 %–74 % of panel height)
            if (endingManager.endingBodyText != null)
            {
                var rt = endingManager.endingBodyText.GetComponent<RectTransform>();
                if (rt != null)
                {
                    rt.anchorMin = new Vector2(0.08f, 0.10f);
                    rt.anchorMax = new Vector2(0.92f, 0.74f);
                    rt.offsetMin = Vector2.zero;
                    rt.offsetMax = Vector2.zero;
                }
                endingManager.endingBodyText.enableWordWrapping = true;
                endingManager.endingBodyText.alignment =
                    TMPro.TextAlignmentOptions.TopLeft;
            }
        }

        if (interviewPanel == null) return;

        // ── 2. Auto-wire DialogueScroll if it wasn't set in Inspector ─────
        // FixInterviewLayout creates a ScrollRect but doesn't auto-assign it
        // to this field. We find it here so the rest of the code can use it.
        if (dialogueScroll == null)
            dialogueScroll = interviewPanel.GetComponentInChildren<UnityEngine.UI.ScrollRect>();

        // ── 2b. Rewire mainText to the TMP inside the ScrollRect Content ──
        // When the user manually added a Scroll View, Unity created a default
        // "Text (TMP)" inside Content.  The Inspector's mainText field may still
        // point to an old MainText outside the scroll view — it gets updated by
        // code but is invisible.  Re-wire to the visible one inside Content.
        if (dialogueScroll != null && dialogueScroll.content != null)
        {
            bool insideContent = mainText != null &&
                                 mainText.transform.IsChildOf(dialogueScroll.content);
            if (!insideContent)
            {
                var mtt = dialogueScroll.content.transform.Find("MainText");
                if (mtt == null)
                {
                    var any = dialogueScroll.content
                                           .GetComponentInChildren<TMPro.TextMeshProUGUI>();
                    mtt = any?.transform;
                }
                if (mtt != null)
                {
                    var tmp = mtt.GetComponent<TMPro.TextMeshProUGUI>();
                    if (tmp != null)
                    {
                        Debug.Log($"[DialogueManager] Rewired mainText " +
                                  $"'{(mainText != null ? mainText.name : "null")}'" +
                                  $" -> '{tmp.name}' (inside Scroll Content)");
                        mainText = tmp;
                    }
                }
            }
        }

        // ── 2c. Wire Button.onClick for every option button ───────────────
        // Physics.Raycast (PlayerController) cannot hit Canvas UI elements
        // without 3D colliders.  Wiring onClick lets Unity's EventSystem handle
        // the click instead — works in any Canvas mode.
        WireBtn(optionOne,   1f);
        WireBtn(optionTwo,   2f);
        WireBtn(optionThree, 3f);
        WireBtn(optionFour,  4f);

        // ── Make option buttons grow to fit their text instead of clipping ──
        ConfigureOptionButtonsForDynamicSizing();

        WireBtnByName(briefingPanel,  "BeginInterviewBtn", BeginInterview);
        if (returnToOfficeBtn != null)
        {
            var rb = returnToOfficeBtn.GetComponent<UnityEngine.UI.Button>();
            if (rb != null) { rb.onClick.RemoveAllListeners(); rb.onClick.AddListener(ReturnToOffice); }
        }

        // ── 3. TopBar: anchor to top, 60 px tall ─────────────────────────
        {

            GameObject topBar = null;
            if (nameText != null)
                topBar = GetDirectChild(interviewPanel, nameText.transform);
            if (topBar == null)
                topBar = interviewPanel.transform.Find("Top Bar")?.gameObject;

            if (topBar != null)
            {
                var rt = topBar.GetComponent<RectTransform>();
                if (rt != null)
                {
                    rt.anchorMin = new Vector2(0f, 1f);
                    rt.anchorMax = Vector2.one;
                    rt.offsetMin = new Vector2(0f, -60f);
                    rt.offsetMax = Vector2.zero;
                }
            }
            // Remove HorizontalLayoutGroup that fights manual positioning
            var hlg = topBar.GetComponent<UnityEngine.UI.HorizontalLayoutGroup>();
            if (hlg != null) DestroyImmediate(hlg);
            // Set dark background
            var topBarImg = topBar.GetComponent<UnityEngine.UI.Image>();
            if (topBarImg != null) topBarImg.color = new Color(0.1f, 0.1f, 0.1f, 0.86f);
        }

        // ── DialogueArea: fills below TopBar ─────────────────────────
        var dialogueAreaGO = interviewPanel.transform.Find("DialogueArea")?.gameObject;
        if (dialogueAreaGO != null)
        {
            var rt = dialogueAreaGO.GetComponent<RectTransform>();
            if (rt != null)
            {
                rt.anchorMin = Vector2.zero;
                rt.anchorMax = Vector2.one;
                rt.offsetMin = new Vector2(0f, 230f);   // was (0f, 0f) — clears OptionsGroup
                rt.offsetMax = new Vector2(0f, -60f);   // clears Top Bar
            }
        }

        // ── 4. ReturnToOfficeBtn: top-left corner, within TopBar height ───
        if (returnToOfficeBtn != null)
        {
            var rt = returnToOfficeBtn.GetComponent<RectTransform>();
            if (rt != null)
            {
                rt.anchorMin        = new Vector2(0f, 1f);
                rt.anchorMax        = new Vector2(0f, 1f);
                rt.pivot            = new Vector2(0f, 1f);
                rt.anchoredPosition = new Vector2(10f, -12f);
                rt.sizeDelta        = new Vector2(150f, 36f);
            }
        }

        // ── 5. Branch: ScrollRect present vs absent ───────────────────────
        //
        // SCROLLRECT PATH (FixInterviewLayout was run):
        //   DialogueArea, OptionsGroup, and their children are already
        //   correctly positioned by the editor tool.  We must NOT move them —
        //   only fix the text component settings and ensure the viewport clips.
        //
        // NO-SCROLLRECT PATH (editor tool was not run):
        //   DialogueArea contains both text and option buttons with no
        //   intermediate hierarchy.  Apply manual RectMask2D and margins.

        if (dialogueScroll != null)
        {
            // ── ScrollRect path ───────────────────────────────────────────

            // Make the Scroll View itself fill its parent (DialogueArea).
            // Unity names this object "Scroll View" rather than "DialogueScroll"
            // so FixInterviewLayout never found and resized it.
            ForceStretch(dialogueScroll.gameObject);

            // Remove the default grey background on the Scroll View root.
            
            var srImg = dialogueScroll.GetComponent<UnityEngine.UI.Image>();
            if (srImg != null)
            {
                srImg.color = new Color(0f, 0f, 0f, 0f);
                srImg.raycastTarget = false;  // ← add this
            }

            // ── Viewport fills Scroll View, mask clips children ───────────
            var vp = dialogueScroll.viewport;
            if (vp != null)
            {
                vp.anchorMin = Vector2.zero;
                vp.anchorMax = Vector2.one;
                vp.offsetMin = Vector2.zero;
                vp.offsetMax = Vector2.zero;

                var vpImg = vp.GetComponent<UnityEngine.UI.Image>();
                if (vpImg == null) vpImg = vp.gameObject.AddComponent<UnityEngine.UI.Image>();
                vpImg.color = new Color(1f, 1f, 1f, 1f);  // alpha = 1
                vpImg.raycastTarget = false;               // ← add this line

                var mask = vp.GetComponent<UnityEngine.UI.Mask>();
                if (mask == null) mask = vp.gameObject.AddComponent<UnityEngine.UI.Mask>();
            }

            // ── Content fills Viewport (fixed size — no ContentSizeFitter) ─
            // Using ContentSizeFitter at runtime requires a canvas rebuild
            // that can race with when text is first set.  A fixed-fill Content
            // is simpler and guaranteed to show text immediately.
           var content = dialogueScroll.content;
            if (content != null)
            {
                var csf = content.GetComponent<UnityEngine.UI.ContentSizeFitter>();
                if (csf == null) csf = content.gameObject.AddComponent<UnityEngine.UI.ContentSizeFitter>();
                csf.verticalFit   = UnityEngine.UI.ContentSizeFitter.FitMode.PreferredSize;
                csf.horizontalFit = UnityEngine.UI.ContentSizeFitter.FitMode.Unconstrained;
                csf.enabled = true;

                content.anchorMin = new Vector2(0f, 1f);
                content.anchorMax = new Vector2(1f, 1f);
                content.pivot     = new Vector2(0.5f, 1f);
                content.offsetMin = Vector2.zero;
                content.offsetMax = Vector2.zero;
            }

            // ── mainText fills Content with margins, clips via Masking ────
            if (mainText != null)
            {
                if (mainText != null)
                {
                    mainText.enableWordWrapping = true;
                    mainText.overflowMode = TMPro.TextOverflowModes.Overflow;  // was Masking
                    mainText.raycastTarget = false;   // ← add this
                    
                }

                var mtRt = mainText.GetComponent<RectTransform>();
                if (mtRt != null)
                {
                    mtRt.anchorMin = Vector2.zero;
                    mtRt.anchorMax = Vector2.one;
                    mtRt.offsetMin = new Vector2(60f, 10f);   // change from (30f, 240f)
                    mtRt.offsetMax = new Vector2(-60f, -10f); // change from (-30f, -10f)tor2(-30f, -10f);
                }
            }
        }
        else
        {
            // ── No-ScrollRect path ────────────────────────────────────────
            // DialogueArea contains both mainText and option buttons.
            // RectMask2D prevents text from bleeding below its bounds.

            var dialogueArea = mainText != null
                ? GetDirectChild(interviewPanel, mainText.transform)
                : null;

            if (dialogueArea != null)
            {
                var daRt = dialogueArea.GetComponent<RectTransform>();
                if (daRt != null)
                {
                    daRt.anchorMin = Vector2.zero;
                    daRt.anchorMax = Vector2.one;
                    daRt.offsetMin = Vector2.zero;
                    daRt.offsetMax = new Vector2(0f, -60f); // leave TopBar gap
                }

                if (dialogueArea.GetComponent<UnityEngine.UI.RectMask2D>() == null)
                    dialogueArea.AddComponent<UnityEngine.UI.RectMask2D>();
            }

            if (mainText != null)
            {
                mainText.enableWordWrapping = true;
                mainText.overflowMode = TMPro.TextOverflowModes.Overflow;  // was Masking

                var mtRt = mainText.GetComponent<RectTransform>();
                if (mtRt != null)
                {
                    mtRt.anchorMin = Vector2.zero;
                    mtRt.anchorMax = Vector2.one;
                    mtRt.offsetMin = new Vector2(60f, 10f);   // was (30f, 240f)
                    mtRt.offsetMax = new Vector2(-60f, -10f); // was (-30f, -10f)
                }
            }
        }
    }
    // Returns the direct child of <panel> that is an ancestor of <descendant>,
    // or null if <descendant> is not inside <panel>.
    private static GameObject GetDirectChild(GameObject panel, Transform descendant)
    {
        if (panel == null || descendant == null) return null;
        Transform t = descendant;
        while (t.parent != null)
        {
            if (t.parent.gameObject == panel) return t.gameObject;
            t = t.parent;
        }
        return null;
    }

    private static void ForceStretch(GameObject go)
    {
        if (go == null) return;
        var rt = go.GetComponent<RectTransform>();
        if (rt == null) return;
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.offsetMin = Vector2.zero;
        rt.offsetMax = Vector2.zero;
    }

    /// <summary>
    /// Wires a Button.onClick listener for one of the dialogue option TextMeshProUGUI
    /// components.  The TMP may live directly on the Button GameObject or as a child.
    /// </summary>
    private void WireBtn(TMPro.TextMeshProUGUI tmp, float optionValue)
    {
        if (tmp == null) return;
        var btn = tmp.GetComponent<UnityEngine.UI.Button>()
               ?? tmp.GetComponentInParent<UnityEngine.UI.Button>();
        if (btn == null) return;
        btn.onClick.RemoveAllListeners();
        float v = optionValue;
        btn.onClick.AddListener(() => OptionClicked(v));
    }

    /// <summary>
    /// Finds a Button inside <paramref name="panel"/> by name and wires its onClick.
    /// </summary>
    private static void WireBtnByName(GameObject panel, string childName,
                                      UnityEngine.Events.UnityAction action)
    {
        if (panel == null) return;
        var t = panel.transform.Find(childName);
        if (t == null) return;
        var btn = t.GetComponent<UnityEngine.UI.Button>();
        if (btn == null) return;
        btn.onClick.RemoveAllListeners();
        btn.onClick.AddListener(action);
    }


    private static void SetChildRect(GameObject parent, string childName,
        Vector2 anchorMin, Vector2 anchorMax, Vector2 offsetMin, Vector2 offsetMax)
    {
        if (parent == null) return;
        var t = parent.transform.Find(childName);
        if (t == null) return;
        var rt = t.GetComponent<RectTransform>();
        if (rt == null) return;
        rt.anchorMin = anchorMin;  rt.anchorMax = anchorMax;
        rt.offsetMin = offsetMin;  rt.offsetMax = offsetMax;
    }

    private void HideAllPanels()
    {
        if (briefingPanel   != null) briefingPanel.SetActive(false);
        if (interviewPanel  != null) interviewPanel.SetActive(false);
        if (summaryPanel    != null) summaryPanel.SetActive(false);
        if (endOfDayPanel   != null) endOfDayPanel.SetActive(false);
        if (dictionaryPanel != null) dictionaryPanel.SetActive(false);
        // Always ensure the ending panel is hidden between phases —
        // it may have been left active in the scene from editing/testing.
        if (endingManager != null && endingManager.endingPanel != null)
            endingManager.endingPanel.SetActive(false);
    }

    /// <summary>
    /// Call from PlayerController when the player clicks an NPC.
    /// Pass the NPC's InterviewBase so BeginInterview() knows who to start.
    /// </summary>
    public void ShowBriefing(InterviewBase interview = null)
    {
        if (interview != null) pendingInterview = interview;

        // Re-clicking a paused NPC skips the briefing and resumes dialogue
        if (GameStateManager.Instance != null &&
            pendingInterview != null &&
            GameStateManager.Instance.IsInterviewPaused(pendingInterview))
        {
            ResumeInterview(pendingInterview);
            return;
        }

        HideAllPanels();
        if (briefingPanel != null) briefingPanel.SetActive(true);

        // Assign the NPC portrait sprite
        if (briefingPortrait != null && pendingInterview != null)
            briefingPortrait.sprite = pendingInterview.portrait;
    }

    // =====================================================================
    // ── NEW ── DESK INTERACTION
    // =====================================================================

    /// <summary>
    /// Called by PlayerController when the player clicks the writing desk.
    /// Skips straight to the Headline selection screen using whichever NPC
    /// was just interviewed (tracked by GameStateManager).
    ///
    /// Only works when GameStateManager.WaitingForDesk is true.
    /// </summary>
    public void ShowDesk()
    {
        if (GameStateManager.Instance == null || !GameStateManager.Instance.WaitingForDesk)
        {
            Debug.Log("[DialogueManager] ShowDesk() called but not waiting for desk — ignoring.");
            return;
        }

        InterviewBase npc = GameStateManager.Instance.GetActiveNPC();
        if (npc != null) current = npc;

        if (current == null)
        {
            Debug.LogWarning("[DialogueManager] ShowDesk(): no active NPC found.");
            return;
        }

        HideAllPanels();
        if (interviewPanel != null) interviewPanel.SetActive(true);

        // Hide Return to Office — player can't leave mid-article
        SetReturnButtonVisible(false);

        currentPhase = Phase.Headline;
        ShowAllOptions();
        if (triesText != null) triesText.gameObject.SetActive(false);
        BuildAndShowHeadlines();
    }

    // =====================================================================
    // INTERVIEW LIFECYCLE
    // =====================================================================

    public void StartInterview(InterviewBase interview)
    {
        current = interview;
        current.ResetState();
        currentPhase  = Phase.Dialogue;
        dictionaryLookUps = current.StartingLookups;
        articleChosen = 0f;

        rawMainText = ""; rawOpt1 = ""; rawOpt2 = ""; rawOpt3 = ""; rawOpt4 = "";

        if (briefingPanel   != null) briefingPanel.SetActive(false);
        if (endOfDayPanel   != null) endOfDayPanel.SetActive(false);
        if (interviewPanel  != null) interviewPanel.SetActive(true);
        if (summaryPanel    != null) summaryPanel.SetActive(false);
        if (dictionaryPanel != null) dictionaryPanel.SetActive(false);

        AudioManager.Instance?.SwitchInterviewMusic(
        GameStateManager.Instance != null 
        ? GameStateManager.Instance.GetID(current) 
        : GameStateManager.CharacterID.None);

        nameText.text  = current.CharacterName;
        triesText.text = "Pick your questions wisely";
        triesText.gameObject.SetActive(true);
        dictionaryLookUpsText.text = "Lookups: " + dictionaryLookUps;

        SetReturnButtonVisible(true);
        InitDictionarySlots();
        ShowAllOptions();

        // Dialogue phase: leave 230px at bottom for option buttons
        SetMainTextBottom(230f);

        current.DialogueSetter(0f, this);
        RefreshDictionaryVisibility();

        // ── CHANGE 1: notify GameStateManager ────────────────────────────
        if (GameStateManager.Instance != null)
            GameStateManager.Instance.OnInterviewStarted(current);
    }

    /// <summary>
    /// Waits until after all Unity layout passes and ScrollRect OnEnable
    /// have finished, then snaps Content to Pos Y = 0 (top of scroll area).
    /// </summary>
    private System.Collections.IEnumerator ForceScrollTopEndOfFrame()
    {
        yield return new WaitForEndOfFrame();
        ScrollToTop();
    }


    public void StartInterviewByIndex(int index)
    {
        if (availableInterviews != null && index >= 0 && index < availableInterviews.Length)
            StartInterview(availableInterviews[index]);
    }

    /// <summary>
    /// Starts an interview at an arbitrary dialogue node, bypassing the
    /// normal briefing panel and day-availability checks.
    /// Used by the secret base path to drop straight into Andrew's
    /// secret-reveal branch (node 279).
    /// </summary>
    public void StartInterviewAtNode(InterviewBase interview, float node)
    {
        current           = interview;
        currentPhase      = Phase.Dialogue;
        dictionaryLookUps = interview.StartingLookups;
        articleChosen     = 0f;

        rawMainText = ""; rawOpt1 = ""; rawOpt2 = ""; rawOpt3 = ""; rawOpt4 = "";

        if (briefingPanel  != null) briefingPanel.SetActive(false);
        if (interviewPanel != null) interviewPanel.SetActive(true);
        if (summaryPanel   != null) summaryPanel.SetActive(false);
        if (endOfDayPanel  != null) endOfDayPanel.SetActive(false);

        nameText.text  = interview.CharacterName + " [TRUE FORM]";
        triesText.gameObject.SetActive(false);

        SetReturnButtonVisible(false); // no escaping the secret base
        SetMainTextBottom(230f);
        InitDictionarySlots();
        ShowAllOptions();

        interview.dialogueIndexTracker = node;
        interview.DialogueSetter(node, this);

        if (GameStateManager.Instance != null)
            GameStateManager.Instance.OnInterviewStarted(interview);
    }

    /// <summary>
    /// Wire this to the "Begin Interview" button on the briefing panel.
    /// </summary>
    public void BeginInterview()
    {
        if (pendingInterview != null)
            StartInterview(pendingInterview);
        else
            StartInterviewByIndex(0);
    }

    // =====================================================================
    // TRANSLATION SYSTEM
    // =====================================================================

    private string TranslateText(string input)
    {
        if (string.IsNullOrEmpty(input) || current == null) return input;

        string result = input;
        foreach (var entry in GetDictionaryEntries())
        {
            if (entry.translated)
            {
                result = InterviewBase.ReplaceDictionarySpellings(
                    result, entry, entry.translation);
            }
            else if (entry.seen)
            {
                result = InterviewBase.ReplaceDictionarySpellings(
                    result, entry, entry.klingonWord + " (?)");
            }
        }
        return result;
    }

    public void SetDialogueTexts(string main, string opt1 = null, string opt2 = null,
                                  string opt3 = null, string opt4 = null)
    {
        if (main  != null) rawMainText = main;
        if (opt1  != null) rawOpt1     = opt1;
        if (opt2  != null) rawOpt2     = opt2;
        if (opt3  != null) rawOpt3     = opt3;
        if (opt4  != null) rawOpt4     = opt4;

        string allText = rawMainText + rawOpt1 + rawOpt2 + rawOpt3 + rawOpt4;

        if (current != null)
        {
            foreach (var entry in current.DictionaryEntries)
            {
                if (InterviewBase.ContainsDictionarySpelling(allText, entry))
                    entry.seen = true;
            }

            foreach (var topic in current.Topics)
                foreach (var keyword in topic.keywords)
                    if (allText.Contains(keyword)) { topic.encountered = true; break; }
        }

        ApplyTranslations();
        RefreshDictionaryVisibility();
    }

    private void ApplyTranslations()
    {
        mainText.text    = TranslateText(rawMainText);
        optionOne.text   = TranslateText(rawOpt1);
        optionTwo.text   = TranslateText(rawOpt2);
        optionThree.text = TranslateText(rawOpt3);
        optionFour.text  = TranslateText(rawOpt4);
        ScrollToTop();
        StartCoroutine(RefreshOptionsLayoutNextFrame());
    }

    private void ScrollToTop()
    {
        // No ScrollRect — MainText sits directly in DialogueArea.
        // Nothing to scroll; text always starts at top of its rect.
    }

    /// <summary>
    /// Adjusts how far MainText's bottom edge sits from the panel bottom.
    /// 230 = leaves room for 4 option buttons (dialogue phase).
    /// 10  = nearly full-height for article/headline text.
    /// </summary>
    private void SetMainTextBottom(float bottomOffset)
    {
        if (mainText == null) return;
        var rt = mainText.GetComponent<RectTransform>();
        if (rt == null) return;
        rt.offsetMin = new Vector2(rt.offsetMin.x, bottomOffset);
    }

    // =====================================================================
    // DICTIONARY
    // =====================================================================

    private List<InterviewBase.DictEntry> GetDictionaryEntries()
    {
        var entries = GameStateManager.Instance != null
            ? GameStateManager.Instance.GetGlobalDictionaryEntries()
            : new List<InterviewBase.DictEntry>();

        if (current != null && current.DictionaryEntries != null)
            foreach (var entry in current.DictionaryEntries)
                if (entry != null && !entries.Contains(entry)) entries.Add(entry);

        return entries;
    }

    private List<InterviewBase.DictEntry> GetDiscoveredDictionaryEntries()
    {
        var entries = new List<InterviewBase.DictEntry>();
        foreach (var entry in GetDictionaryEntries())
            if (entry.seen || entry.translated) entries.Add(entry);
        return entries;
    }

    public void ToggleDictionary()
    {
        if (dictionaryPanel != null)
        {
            bool opening = !dictionaryPanel.activeSelf;
            dictionaryPanel.SetActive(opening);
            if (opening) AudioManager.Instance?.PlaySFX(AudioManager.SFX.Dictionary);
            if (opening) ResetDictRightPanel();
        }
    }

    private void InitDictionarySlots()
    {
        if (dictionarySlots == null) return;
        var entries = GetDiscoveredDictionaryEntries();
        for (int i = 0; i < dictionarySlots.Length; i++)
        {
            // Hide the parent button (DictBtn), not just the text child
            var btn = dictionarySlots[i].transform.parent.gameObject;
            if (i < entries.Count)
                btn.SetActive(true);
            else
                btn.SetActive(false);
        }
    }

    private void RefreshDictionaryVisibility()
    {
        if (dictionarySlots == null) return;
        var entries = GetDiscoveredDictionaryEntries();
        for (int i = 0; i < dictionarySlots.Length; i++)
        {
            var btn = dictionarySlots[i].transform.parent.gameObject;
            if (i < entries.Count)
            {
                var e = entries[i];
                bool show = e.seen || e.translated;
                btn.SetActive(show);
                if (show)
                    dictionarySlots[i].text = e.translated
                        ? e.translation
                        : e.klingonWord + " (?)";
            }
            else
                btn.SetActive(false);
        }
    }

    public void DictionaryLookup(int oneBasedIndex)
    {
        var entries = GetDiscoveredDictionaryEntries();
        int i = oneBasedIndex - 1;
        if (i < 0 || i >= entries.Count) return;

        var entry = entries[i];

        // If already translated, just show the entry — don't spend a lookup
        if (entry.translated)
        {
            ShowDictEntry(entry);
            return;
        }

        // Word seen but not yet translated — spend a lookup
        if (!entry.seen || dictionaryLookUps <= 0)
        {
            ShowDictEntryLocked();
            return;
        }

        dictionaryLookUps--;
        dictionaryLookUpsText.text = "Lookups: " + dictionaryLookUps;
        entry.translated = true;
        dictionarySlots[i].text = entry.translation;

        ApplyTranslations();
        RefreshDictionaryVisibility();
        ShowDictEntry(entry);
    }

    private void ShowDictEntry(InterviewBase.DictEntry entry)
    {
Debug.Log("[DM] ShowDictEntry called: " + entry.klingonWord);
        if (dictDefaultMsg    != null) dictDefaultMsg.SetActive(false);
        if (dictUndiscoveredMsg != null) dictUndiscoveredMsg.SetActive(false);
        if (dictSelectedView  != null) 
        {
            dictSelectedView.SetActive(true);

            // Force layout rebuild so anchored children get correct rects
            var rt = dictSelectedView.GetComponent<RectTransform>();
            if (rt != null)
            {
                UnityEngine.UI.LayoutRebuilder.ForceRebuildLayoutImmediate(rt);
                Debug.Log($"[DM] DictSelectedView rect after rebuild: {rt.rect}");
            }

            // Warn about CanvasGroup alpha issues
            var cg = dictSelectedView.GetComponentInParent<CanvasGroup>();
            if (cg != null && cg.alpha < 0.01f)
                Debug.LogWarning($"[DM] CanvasGroup '{cg.name}' has alpha~0 — content will be invisible!");

            // Warn about RectMask2D clipping
            var mask = dictSelectedView.GetComponentInParent<UnityEngine.UI.RectMask2D>();
            if (mask != null)
            {
                var maskRt = mask.GetComponent<RectTransform>();
                Debug.Log($"[DM] RectMask2D found on '{mask.name}', rect: {maskRt?.rect}");
            }
        }        
        if (dictKlingonText != null) dictKlingonText.text  = entry.klingonWord;
        if (dictEnglishText != null) dictEnglishText.text  = entry.translation;
        if (dictStatusText  != null) dictStatusText.text   = "Confirmed";
        if (dictContextText != null) dictContextText.text  = "";  // fill from data if added later
        if (dictNotesText   != null) dictNotesText.text    = "Translation logged after player interaction";
    }

    private void ShowDictEntryLocked()
    {
        if (dictDefaultMsg      != null) dictDefaultMsg.SetActive(false);
        if (dictSelectedView    != null) dictSelectedView.SetActive(false);
        if (dictUndiscoveredMsg != null) dictUndiscoveredMsg.SetActive(true);
    }

    private void ResetDictRightPanel()
    {
        if (dictDefaultMsg      != null) dictDefaultMsg.SetActive(true);
        if (dictSelectedView    != null) dictSelectedView.SetActive(false);
        if (dictUndiscoveredMsg != null) dictUndiscoveredMsg.SetActive(false);
    }

    // =====================================================================
    // OPTION HELPERS
    // =====================================================================

    public void ShowAllOptions()
    {
        optionOne.gameObject.SetActive(true);
        optionTwo.gameObject.SetActive(true);
        optionThree.gameObject.SetActive(true);
        optionFour.gameObject.SetActive(true);
    }

    // =====================================================================
    // OPTION CLICKED — Main flow controller
    // =====================================================================

    public void OptionClicked(float option)
    {
        switch (currentPhase)
        {
            // ── Dialogue ──────────────────────────────────────────────────
            case Phase.Dialogue:
                if (current == null) return;
                current.DialogueSetter(option, this);

                bool allOff = !optionOne.gameObject.activeSelf  &&
                              !optionTwo.gameObject.activeSelf  &&
                              !optionThree.gameObject.activeSelf &&
                              !optionFour.gameObject.activeSelf;

                if (allOff)
                {
                    currentPhase = Phase.EndTransition;
                    optionOne.gameObject.SetActive(true);
                    optionOne.text = "Continue";
                    triesText.text = "Interview complete — head to the desk to write your article";
                }
                else if (current.LastQuestionNodes.Contains(current.dialogueIndexTracker))
                {
                    triesText.text = "Last Question";
                }
                break;

            // ── EndTransition → sends player to desk ──────────────────────
            // The headline / article phase now begins only when the player
            // physically walks to the desk and clicks it (ShowDesk()).
            // Here we just tidy up the UI and set the WaitingForDesk flag.
            case Phase.EndTransition:
                if (current != null && !completedToday.Contains(current))
                    completedToday.Add(current);

                // ── Secret ending: skip desk entirely ─────────────────────
                if (GameStateManager.Instance != null &&
                    GameStateManager.Instance.IsSecretEndingFound())
                {
                    HideAllPanels();
                    if (endingManager != null)
                        endingManager.TriggerEnding(marsOpinionScore);
                    else
                        Debug.LogError("[DM] EndingManager not assigned — secret ending cannot fire.");
                    break;
                }

                if (GameStateManager.Instance != null)
                    GameStateManager.Instance.OnInterviewComplete();

                // Hide panels FIRST, then restore camera/controls
                HideAllPanels();
                if (playerMovement != null)
                    playerMovement.ReturnToOffice();
                break;

            // ── Headline → ArticleWriting ─────────────────────────────────
            case Phase.Headline:
                int slot = Mathf.RoundToInt(option) - 1;
                if (slot < 0 || slot >= 4) return;
                int headlineIdx = headlineSlotToIndex[slot];
                if (headlineIdx < 0) return;

                articleChosen = headlineIdx;
                StartArticleWriting(headlineIdx);
                break;

            // ── ArticleWriting ────────────────────────────────────────────
            case Phase.ArticleWriting:
                HandleParagraphChoice(option);
                break;

            // ── ArticleComplete → publish ─────────────────────────────────
            case Phase.ArticleComplete:
                // ── CHANGE 3: notify GameStateManager + trigger ending ────
                if (GameStateManager.Instance != null)
                {
                    int dayBeforeAdvance = GameStateManager.Instance.CurrentDay;
                    GameStateManager.Instance.OnArticlePublished();

                    if (dayBeforeAdvance >= totalDays)
                    {
                        // Auto-find endingManager if not wired in Inspector
                        if (endingManager == null)
                            endingManager = FindObjectOfType<EndingManager>();

                        // Restore the 3D world camera FIRST (hides interview UI)
                        HideAllPanels();
                        if (playerMovement != null) playerMovement.ReturnToOffice();

                        // Then overlay the ending screen
                        if (endingManager != null)
                            endingManager.TriggerEnding(marsOpinionScore);
                        else
                            Debug.LogError("[DialogueManager] EndingManager not found — " +
                                "please assign it in the Inspector or add it to the scene.");
                        return;
                    }
                }

                ShowEndOfDay();
                break;

            // ── DayEnd → return to office for next day ────────────────────
            case Phase.DayEnd:
                EndOfDayContinue();
                break;
        }
    }

    // =====================================================================
    // HEADLINES
    // =====================================================================

    private bool IsHeadlineUnlocked(int index)
    {
        if (current == null || index >= current.Headlines.Length) return false;
        var h = current.Headlines[index];
        if (h.alwaysAvailable) return true;

        bool dictOk  = h.requiredDictIndex < 0 ||
                       (h.requiredDictIndex < current.DictionaryEntries.Length &&
                        current.DictionaryEntries[h.requiredDictIndex].translated);

        bool topicOk = h.requiredTopicIndex < 0 ||
                       (h.requiredTopicIndex < current.Topics.Length &&
                        current.Topics[h.requiredTopicIndex].encountered);

        return dictOk && topicOk && current.IsAdditionalHeadlineConditionMet(index);
    }

    private void BuildAndShowHeadlines()
    {
        for (int i = 0; i < 4; i++) headlineSlotToIndex[i] = -1;

        int slot = 0;
        if (current != null)
        {
            for (int i = 0; i < current.Headlines.Length && slot < 4; i++)
            {
                if (IsHeadlineUnlocked(i))
                {
                    headlineSlotToIndex[slot] = i;
                    slot++;
                }
            }
        }

        string[] labels = new string[4];
        TextMeshProUGUI[] buttons = { optionOne, optionTwo, optionThree, optionFour };
        for (int s = 0; s < 4; s++)
        {
            if (headlineSlotToIndex[s] >= 0)
            {
                labels[s] = current.Headlines[headlineSlotToIndex[s]].text;
                buttons[s].gameObject.SetActive(true);
            }
            else
            {
                labels[s] = "";
                buttons[s].gameObject.SetActive(false);
            }
        }

        SetDialogueTexts("Choose your article headline:",
            labels[0], labels[1], labels[2], labels[3]);
    }

    // =====================================================================
    // ARTICLE WRITING
    // =====================================================================

    private void StartArticleWriting(int headlineIndex)
    {
        currentPhase = Phase.ArticleWriting;
        currentParagraphIndex = 0;
        articleLines.Clear();

        // Article phase: expand MainText downward — options are still visible
        // but only 1-3 are shown and they sit in the OptionsGroup below.
        // Give the article text more vertical room.
        SetMainTextBottom(10f);

        currentArticle = null;
        if (current != null && current.ArticleTemplates != null)
            foreach (var t in current.ArticleTemplates)
                if (t.headlineIndex == headlineIndex) { currentArticle = t; break; }

        if (currentArticle == null ||
            currentArticle.paragraphs == null ||
            currentArticle.paragraphs.Length == 0)
        {
            Debug.LogWarning($"[DialogueManager] No ArticleTemplate for headline index {headlineIndex}.");
            ShowArticleComplete();
            return;
        }

        ShowCurrentParagraph();
    }

    private void ShowCurrentParagraph()
    {
        var para = currentArticle.paragraphs[currentParagraphIndex];

        // Show only the MOST RECENTLY WRITTEN paragraph (not every previous one)
        // so the text area stays short and never overlaps the option buttons.
        string built = "";
        if (articleLines.Count > 0)
            built = articleLines[articleLines.Count - 1] + "\n\n\u2014\u2014\u2014\u2014\n\n";

        built += $"[{currentParagraphIndex + 1}/{currentArticle.paragraphs.Length}] {para.promptText}";

        // Set active state BEFORE the text/layout pass below. RefreshOptionsLayout
        // (triggered inside SetDialogueTexts) measures whichever option buttons are
        // currently active — if we activate/deactivate them afterward instead, it
        // measures the wrong set (leftover from the previous screen), computes too
        // small a height, and the real (larger) button block ends up overlapping
        // mainText once Unity's own layout pass catches up a frame later.
        optionOne.gameObject.SetActive(true);
        optionTwo.gameObject.SetActive(true);
        optionThree.gameObject.SetActive(true);
        optionFour.gameObject.SetActive(false);

        // Buttons now resize to fit their text (see ConfigureOptionButtonsForDynamicSizing),
        // so the full paragraph choice can be shown instead of being cut at 100 chars.
        SetDialogueTexts(built,
            para.truthful.text,
            para.dishonest.text,
            para.ambitious.text,
            "");

        triesText.gameObject.SetActive(true);
        triesText.text = $"Writing article... ({currentParagraphIndex + 1}/{currentArticle.paragraphs.Length})";
    }

    private void HandleParagraphChoice(float option)
    {
        if (currentArticle == null) return;
        var para = currentArticle.paragraphs[currentParagraphIndex];

        InterviewBase.ParagraphChoice chosen;
        if      (option == 1f) { chosen = para.truthful;  truthfulCount++;  }
        else if (option == 2f) { chosen = para.dishonest; dishonestCount++; }
        else if (option == 3f) { chosen = para.ambitious; ambitiousCount++; }
        else return;

        marsOpinionScore += chosen.scoreEffect;
        articleLines.Add(chosen.text);
        currentParagraphIndex++;

        if (currentParagraphIndex >= currentArticle.paragraphs.Length)
            ShowArticleComplete();
        else
            ShowCurrentParagraph();
    }

    private void ShowArticleComplete()
    {
        currentPhase = Phase.ArticleComplete;

        int    hIdx     = Mathf.RoundToInt(articleChosen);
        string headline = (current != null && hIdx >= 0 && hIdx < current.Headlines.Length)
                        ? current.Headlines[hIdx].text : "Unknown";

        string fullArticle = $"\u2014\u2014 {headline} \u2014\u2014\n\n";
        foreach (var line in articleLines)
            fullArticle += line + "\n\n";

        // Active state set BEFORE the text/layout pass — see the comment in
        // ShowCurrentParagraph for why the order matters.
        optionOne.gameObject.SetActive(true);
        optionTwo.gameObject.SetActive(false);
        optionThree.gameObject.SetActive(false);
        optionFour.gameObject.SetActive(false);

        SetDialogueTexts(fullArticle, "", "", "", "");

        AudioManager.Instance?.PlaySFX(AudioManager.SFX.Publish);
        optionOne.text = "Publish";
        RefreshOptionsLayout();   // "Publish" was set directly, bypassing ApplyTranslations

        triesText.gameObject.SetActive(true);
        triesText.text = $"Day {currentDay}  —  ready to publish";
    }

    // =====================================================================
    // END OF DAY
    // =====================================================================

    private void ShowEndOfDay()
    {
        currentPhase = Phase.DayEnd;

        // Keep interviewPanel ACTIVE — the Continue button (optionOne) is its child.
        // Hiding interviewPanel would make the button unclickable.
        // Instead, write the day summary into mainText and show only Continue.
        if (interviewPanel != null) interviewPanel.SetActive(true);
        if (summaryPanel   != null) summaryPanel.SetActive(false);
        // Don't touch endOfDayPanel here — if it's mistakenly wired to EndingPanel
        // we do not want to activate it for a day transition.

        string summary = current != null ? current.GetEndOfDaySummary() : "Day complete.";
        if (mainText != null) mainText.text = summary;

        optionOne.gameObject.SetActive(true);
        optionOne.text = "Continue";
        optionTwo.gameObject.SetActive(false);
        optionThree.gameObject.SetActive(false);
        optionFour.gameObject.SetActive(false);

        SetReturnButtonVisible(false);

        if (triesText != null)
        {
            triesText.gameObject.SetActive(true);
            triesText.text = $"Day {currentDay} complete \u2014 click Continue to head back.";
        }
    }

    /// <summary>
    /// Wire to the end-of-day "Continue" button.
    /// Day counter is now owned by GameStateManager; this just resets the UI.
    /// </summary>
    public void EndOfDayContinue()
    {
        AudioManager.Instance?.SwitchMusic(AudioManager.MusicTrack.Office);
        _localDay = currentDay;
        completedToday.Clear();
        currentPhase = Phase.Dialogue;

        // Return player to the 3D office to start the next day
        HideAllPanels();
        if (playerMovement != null)
            playerMovement.ReturnToOffice();
    }

    // =====================================================================
    // RETURN TO OFFICE  (called by ReturnToOfficeBtnBtn)
    // =====================================================================

    public void ReturnToOffice()
    {
        AudioManager.Instance?.SwitchMusic(AudioManager.MusicTrack.Office);
        // If mid-dialogue, pause so the player can resume by re-clicking the NPC
        if (currentPhase == Phase.Dialogue && current != null)
        {
            SnapshotButtonStates();
            if (GameStateManager.Instance != null)
                GameStateManager.Instance.PauseInterview();
        }

        HideAllPanels();
        if (playerMovement != null)
            playerMovement.ReturnToOffice();
    }

    // =====================================================================
    // PAUSE / RESUME HELPERS
    // =====================================================================

    private void SnapshotButtonStates()
    {
        _snapOpt1 = optionOne.gameObject.activeSelf;
        _snapOpt2 = optionTwo.gameObject.activeSelf;
        _snapOpt3 = optionThree.gameObject.activeSelf;
        _snapOpt4 = optionFour.gameObject.activeSelf;
    }

    private void RestoreButtonStates()
    {
        optionOne.gameObject.SetActive(_snapOpt1);
        optionTwo.gameObject.SetActive(_snapOpt2);
        optionThree.gameObject.SetActive(_snapOpt3);
        optionFour.gameObject.SetActive(_snapOpt4);
    }

    private void ResumeInterview(InterviewBase interview)
    {
        current      = interview;
        currentPhase = Phase.Dialogue;

        if (briefingPanel   != null) briefingPanel.SetActive(false);
        if (endOfDayPanel   != null) endOfDayPanel.SetActive(false);
        if (interviewPanel  != null) interviewPanel.SetActive(true);
        if (summaryPanel    != null) summaryPanel.SetActive(false);
        if (dictionaryPanel != null) dictionaryPanel.SetActive(false);

        nameText.text = current.CharacterName;
        triesText.gameObject.SetActive(true);
        dictionaryLookUpsText.text = "Lookups: " + dictionaryLookUps;

        RestoreButtonStates();
        ApplyTranslations();
        RefreshDictionaryVisibility();
        SetReturnButtonVisible(true);

        if (GameStateManager.Instance != null)
            GameStateManager.Instance.OnInterviewStarted(current);
    }

    // =====================================================================
    // UTILITY
    // =====================================================================

    private void SetReturnButtonVisible(bool visible)
    {
        if (returnToOfficeBtn != null)
            returnToOfficeBtn.SetActive(visible);
    }

    // =====================================================================
    // DYNAMIC OPTION-BUTTON SIZING
    // =====================================================================
    // The old setup put all 4 option buttons in a fixed 2x2 grid inside a
    // 230px-tall "OptionsGroup", with each TMP text left on its prefab's
    // default overflow mode (Ellipsis/Truncate). Any answer long enough to
    // need more than one line inside that fixed cell got cut off with "...".
    //
    // Fix: turn OptionsGroup into a VerticalLayoutGroup + ContentSizeFitter
    // so it (and each button inside it) grows to fit however much text is
    // actually there, then push DialogueArea's bottom edge down to match —
    // so the two never overlap.

    private void ConfigureOptionButtonsForDynamicSizing()
    {
        if (optionOne == null) return;

        if (_baseOptionFontSize < 0f) _baseOptionFontSize = optionOne.fontSize;

        Transform group = optionOne.transform.parent;
        if (group == null) return;
        optionsGroup = group.gameObject;

        // A GridLayoutGroup (fixed-size 2x2) fights dynamic sizing — remove it.
        var grid = optionsGroup.GetComponent<UnityEngine.UI.GridLayoutGroup>();
        if (grid != null) DestroyImmediate(grid);

        var vlg = optionsGroup.GetComponent<UnityEngine.UI.VerticalLayoutGroup>();
        if (vlg == null) vlg = optionsGroup.AddComponent<UnityEngine.UI.VerticalLayoutGroup>();
        vlg.childForceExpandWidth  = true;
        vlg.childForceExpandHeight = false;
        vlg.childControlWidth      = true;
        vlg.childControlHeight     = true;
        vlg.spacing                = 10f;
        vlg.padding                = new RectOffset(20, 20, 10, 10);

        var groupCsf = optionsGroup.GetComponent<UnityEngine.UI.ContentSizeFitter>();
        if (groupCsf == null) groupCsf = optionsGroup.AddComponent<UnityEngine.UI.ContentSizeFitter>();
        groupCsf.verticalFit   = UnityEngine.UI.ContentSizeFitter.FitMode.PreferredSize;
        groupCsf.horizontalFit = UnityEngine.UI.ContentSizeFitter.FitMode.Unconstrained;

        // Anchor to the bottom of the panel; height now comes from content,
        // not a hardcoded 230px block.
        var groupRt = optionsGroup.GetComponent<RectTransform>();
        if (groupRt != null)
        {
            groupRt.anchorMin        = new Vector2(0f, 0f);
            groupRt.anchorMax        = new Vector2(1f, 0f);
            groupRt.pivot            = new Vector2(0.5f, 0f);
            groupRt.anchoredPosition = Vector2.zero;
        }

        foreach (var opt in new[] { optionOne, optionTwo, optionThree, optionFour })
        {
            if (opt == null) continue;

            // Never truncate/ellipsis — wrap onto as many lines as needed.
            opt.enableWordWrapping = true;
            opt.overflowMode       = TMPro.TextOverflowModes.Overflow;

            GameObject btnGO = opt.gameObject;
            var btn = opt.GetComponentInParent<UnityEngine.UI.Button>();
            if (btn != null) btnGO = btn.gameObject;

            var le = btnGO.GetComponent<UnityEngine.UI.LayoutElement>();
            if (le == null) le = btnGO.AddComponent<UnityEngine.UI.LayoutElement>();
            le.minHeight     = 60f;
            le.flexibleWidth = 1f;

            var csf = btnGO.GetComponent<UnityEngine.UI.ContentSizeFitter>();
            if (csf == null) csf = btnGO.AddComponent<UnityEngine.UI.ContentSizeFitter>();
            csf.verticalFit   = UnityEngine.UI.ContentSizeFitter.FitMode.PreferredSize;
            csf.horizontalFit = UnityEngine.UI.ContentSizeFitter.FitMode.Unconstrained;
        }
    }

    /// <summary>
    /// Call whenever option text changes. Rebuilds the option-group layout
    /// so its height reflects the current text, then pushes both DialogueArea's
    /// and mainText's own bottom edge to sit just above it.
    ///
    /// This replaces the old hardcoded SetMainTextBottom(230f)/(10f) calls as
    /// the source of truth for that boundary. Those calls only set a one-time
    /// static number, so as soon as options grew taller than whatever number
    /// was hardcoded (which happens constantly now that paragraph choices are
    /// full-length instead of truncated to 100 chars), mainText's bottom edge
    /// stayed put and the options rendered right on top of it. Recomputing it
    /// here, every time text changes, keeps it correct in every phase.
    /// </summary>
    private System.Collections.IEnumerator RefreshOptionsLayoutNextFrame()
    {
        yield return null;
        RefreshOptionsLayout();
    }

    private void RefreshOptionsLayout()
    {
        if (optionsGroup == null || interviewPanel == null) return;

        var groupRt = optionsGroup.GetComponent<RectTransform>();
        if (groupRt == null) return;

        // Always start from the full baseline font size before measuring —
        // otherwise a shrink applied on a previous (longer) dialogue node
        // would silently carry over and compound on later, shorter ones.
        var allOptions = new[] { optionOne, optionTwo, optionThree, optionFour };
        if (_baseOptionFontSize > 0f)
            foreach (var opt in allOptions)
                if (opt != null) opt.fontSize = _baseOptionFontSize;

        Canvas.ForceUpdateCanvases();
        UnityEngine.UI.LayoutRebuilder.ForceRebuildLayoutImmediate(groupRt);

        float groupHeight = groupRt.rect.height;

        // Hard cap: the options block must never claim more than half the
        // panel's height. Without this, a node with several options (even
        // short ones — the exact bug reported on Andrew's yes/no/price node
        // in a smaller Editor Game window) can grow tall enough to push its
        // last button below the visible, clickable screen area entirely.
        // If it's still too tall after shrinking fonts to a sane floor, the
        // buttons just sit snugly rather than overflowing — nothing is ever
        // pushed off-screen or made unclickable again.
        var panelRt = interviewPanel.GetComponent<RectTransform>();
        float maxAllowed = panelRt != null ? panelRt.rect.height * 0.5f : groupHeight;

        if (groupHeight > maxAllowed && groupHeight > 0f && _baseOptionFontSize > 0f)
        {
            float shrink = Mathf.Clamp(maxAllowed / groupHeight, 0.55f, 1f);
            foreach (var opt in allOptions)
                if (opt != null) opt.fontSize = _baseOptionFontSize * shrink;

            Canvas.ForceUpdateCanvases();
            UnityEngine.UI.LayoutRebuilder.ForceRebuildLayoutImmediate(groupRt);
            groupHeight = Mathf.Min(groupRt.rect.height, maxAllowed);
        }

        GameObject dialogueAreaGO = null;
        if (mainText != null)
            dialogueAreaGO = GetDirectChild(interviewPanel, mainText.transform);
        if (dialogueAreaGO == null)
            dialogueAreaGO = interviewPanel.transform.Find("DialogueArea")?.gameObject;
        if (dialogueAreaGO == null && optionsGroup != null)
            dialogueAreaGO = GetDirectChild(interviewPanel, optionsGroup.transform);

        if (dialogueAreaGO != null)
        {
            var daRt = dialogueAreaGO.GetComponent<RectTransform>();
            if (daRt != null)
                daRt.offsetMin = new Vector2(daRt.offsetMin.x, groupHeight + 10f);
        }

        // mainText's bottom edge is a separate RectTransform from DialogueArea's
        // (see SetMainTextBottom) — update it too, or it keeps whatever fixed
        // number was last hardcoded and text sits underneath the options.
        if (mainText != null)
        {
            var mtRt = mainText.GetComponent<RectTransform>();
            if (mtRt != null)
                mtRt.offsetMin = new Vector2(mtRt.offsetMin.x, groupHeight + 10f);
        }
    }

    // ── Responsive font sizing ────────────────────────────────────────────

    private int _lastScreenHeight = 0;

    private void Update()
    {
        // Re-apply font sizes whenever the window is resized
        if (Screen.height != _lastScreenHeight)
        {
            _lastScreenHeight = Screen.height;
            ApplyResponsiveFontSizes();
        }
    }

    /// <summary>
    /// Scales all dialogue font sizes relative to a 1080p reference.
    /// Base sizes (at 1080p): mainText=20, options=15, name/status=16.
    /// Clamped so text never becomes unreadably tiny or screen-filling huge.
    /// </summary>
    private void ApplyResponsiveFontSizes()
    {
        // Scale relative to a 1080p reference, clamped so it's always legible.
        // Minimum raised to 0.85 so even small Game-view windows stay readable.
        float scale = Mathf.Clamp(Screen.height / 1080f, 0.85f, 2.0f);

        float sizeMain    = Mathf.Round(42f * scale);   // dialogue body  (was 34)
        float sizeOptions = Mathf.Round(32f * scale);   // answer buttons (was 26)
        float sizeName    = Mathf.Round(30f * scale);   // NPC name / status bar (was 26)

        if (mainText  != null) { mainText.enableAutoSizing  = false; mainText.fontSize  = sizeMain; }
        if (triesText != null) { triesText.enableAutoSizing = false; triesText.fontSize  = sizeName; }
        if (nameText  != null) { nameText.enableAutoSizing  = false; nameText.fontSize   = sizeName; }
        if (dictionaryLookUpsText != null)
        {
            dictionaryLookUpsText.enableAutoSizing = false;
            dictionaryLookUpsText.fontSize = sizeName;
        }
        foreach (var opt in new[] { optionOne, optionTwo, optionThree, optionFour })
            if (opt != null) { opt.enableAutoSizing = false; opt.fontSize = sizeOptions; }
        if (dictionarySlots != null)
            foreach (var s in dictionarySlots)
                if (s != null) { s.enableAutoSizing = false; s.fontSize = sizeOptions; }
    }
}
