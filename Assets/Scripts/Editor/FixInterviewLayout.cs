#if UNITY_EDITOR
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using TMPro;

public class FixInterviewLayout : EditorWindow
{
    const int VERSION = 8;

    // =====================================================================
    // WIREFRAME PALETTE
    // =====================================================================
    // #E7C9A9 — "Darker Manila"  — default & undiscovered card background
    // #FEF1DC — "Lighter Cream"  — selected card background
    // These are also exposed here so DialogueManager can reference them by
    // name when it switches states at runtime.
    // =====================================================================
    static readonly Color C_MANILA   = new Color(0.906f, 0.788f, 0.663f, 1f); // #E7C9A9
    static readonly Color C_CREAM    = new Color(0.996f, 0.945f, 0.863f, 1f); // #FEF1DC
    static readonly Color C_WHITE    = Color.white;
    static readonly Color C_BLACK    = Color.black;

    [MenuItem("Tools/Fix Interview Layout")]
    public static void Fix()
    {
        Undo.SetCurrentGroupName("Fix Interview Layout v" + VERSION);
        int undoGroup = Undo.GetCurrentGroup();

        // ── Top-level panels: stretch to fill ─────────────────────────────
        StretchAll("BriefingPanel");
        StretchAll("InterviewPanel");
        StretchAll("SummaryPanel");
        StretchAll("EndOfDayPanel");

        SetBg("InterviewPanel", new Color(0.13f, 0.13f, 0.13f, 0.92f));
        SetBg("BriefingPanel",  new Color(0.18f, 0.18f, 0.18f, 0.95f));

        // ── BRIEFING ──────────────────────────────────────────────────────
        FixRT("AssignmentTitle", rt => {
            Anchor(rt, 0, 0.85f, 1, 1);
            rt.offsetMin = new Vector2(30, 0); rt.offsetMax = new Vector2(-30, -10);
        });
        AutoTMP("AssignmentTitle", 18, 30, TextAlignmentOptions.Center, Color.white, "INTERVIEW BRIEFING");

        FixRT("CharacterPortrait", rt => {
            Anchor(rt, 0.08f, 0.25f, 0.35f, 0.8f);
            rt.offsetMin = Vector2.zero; rt.offsetMax = Vector2.zero;
        });

        FixRT("AssignmentBody", rt => {
            Anchor(rt, 0.38f, 0.35f, 0.92f, 0.75f);
            rt.offsetMin = Vector2.zero; rt.offsetMax = Vector2.zero;
        });
        AutoTMP("AssignmentBody", 14, 22, TextAlignmentOptions.Center, Color.white, null);

        FixRT("BeginInterviewBtn", rt => {
            Anchor(rt, 0.5f, 0.5f, 0.5f, 0.5f);
            rt.sizeDelta = new Vector2(260, 50);
            rt.anchoredPosition = new Vector2(0, -140);
        });
        var beginGO = Find("BeginInterviewBtn");
        if (beginGO != null)
        {
            if (beginGO.GetComponent<Button>() == null)
                Undo.AddComponent<Button>(beginGO);
            var img = beginGO.GetComponent<Image>();
            if (img != null) { Record(img); img.color = new Color(0.3f, 0.65f, 0.3f, 1f); }
            var btn = beginGO.GetComponent<Button>();
            if (btn != null && img != null) btn.targetGraphic = img;
        }

        // ── TOP BAR ───────────────────────────────────────────────────────
        FixRT("Top Bar", rt => {
            Anchor(rt, 0, 1, 1, 1);
            rt.offsetMin = new Vector2(0, -60); rt.offsetMax = Vector2.zero;
        });
        SafeDestroy<HorizontalLayoutGroup>("Top Bar");
        SafeDestroy<VerticalLayoutGroup>("Top Bar");
        SetBg("Top Bar", new Color(0.1f, 0.1f, 0.1f, 0.85f));

        FixRT("NameText", rt => {
            Anchor(rt, 0, 0, 0.55f, 1);
            rt.offsetMin = new Vector2(25, 5); rt.offsetMax = new Vector2(-10, -5);
        });
        SafeDestroy<LayoutElement>("NameText");
        AutoTMP("NameText", 16, 26, TextAlignmentOptions.MidlineLeft, Color.white, null);

        FixRT("StatusText", rt => {
            Anchor(rt, 0.55f, 0, 1, 1);
            rt.offsetMin = new Vector2(10, 5); rt.offsetMax = new Vector2(-25, -5);
        });
        SafeDestroy<LayoutElement>("StatusText");
        AutoTMP("StatusText", 12, 20, TextAlignmentOptions.MidlineRight, new Color(0.95f, 0.85f, 0.65f), null);

        // ── DIALOGUE AREA ─────────────────────────────────────────────────
        FixRT("DialogueArea", rt => {
            Anchor(rt, 0, 0, 1, 1);
            rt.offsetMin = new Vector2(20, 230); rt.offsetMax = new Vector2(-20, -65);
        });
        SafeDestroy<VerticalLayoutGroup>("DialogueArea");

        StretchAll("DialogueScroll");
        var scrollGO = Find("DialogueScroll");
        if (scrollGO != null)
        {
            var sr = scrollGO.GetComponent<ScrollRect>();
            if (sr != null) { sr.horizontal = false; sr.movementType = ScrollRect.MovementType.Clamped; }
        }
        StretchAll("Viewport");

        FixRT("Content", rt => {
            Anchor(rt, 0, 1, 1, 1);
            rt.offsetMin = Vector2.zero; rt.offsetMax = Vector2.zero;
            rt.pivot = new Vector2(0.5f, 1f);
        });
        var contentGO = Find("Content");
        if (contentGO != null)
        {
            var csf = contentGO.GetComponent<ContentSizeFitter>();
            if (csf == null) csf = Undo.AddComponent<ContentSizeFitter>(contentGO);
            csf.verticalFit   = ContentSizeFitter.FitMode.PreferredSize;
            csf.horizontalFit = ContentSizeFitter.FitMode.Unconstrained;
        }

        FixRT("MainText", rt => {
            Anchor(rt, 0, 0, 1, 1);
            rt.offsetMin = new Vector2(30, 10); rt.offsetMax = new Vector2(-30, -10);
        });
        AutoTMP("MainText", 16, 32, TextAlignmentOptions.Center, Color.white, null);

        // ── OPTIONS (2 × 2 grid) ──────────────────────────────────────────
        FixRT("OptionsGroup", rt => {
            Anchor(rt, 0, 0, 1, 0);
            rt.offsetMin = new Vector2(20, 10); rt.offsetMax = new Vector2(-20, 225);
        });
        SafeDestroy<VerticalLayoutGroup>("OptionsGroup");
        SetBg("OptionsGroup", new Color(0.1f, 0.1f, 0.1f, 0.6f));
        var optGO = Find("OptionsGroup");
        if (optGO != null)
        {
            var glg = optGO.GetComponent<GridLayoutGroup>();
            if (glg == null) glg = Undo.AddComponent<GridLayoutGroup>(optGO);
            glg.constraint      = GridLayoutGroup.Constraint.FixedColumnCount;
            glg.constraintCount = 2;
            glg.cellSize        = new Vector2(420, 55);
            glg.spacing         = new Vector2(14, 10);
            glg.padding         = new RectOffset(20, 20, 10, 10);
            glg.childAlignment  = TextAnchor.MiddleCenter;
        }

        FixOptBtn("OptionBtn1"); FixOptBtn("OptionBtn2");
        FixOptBtn("OptionBtn3"); FixOptBtn("OptionBtn4");

        // ── DICTIONARY (full rebuild) ─────────────────────────────────────
        RebuildDictionary();

        Undo.CollapseUndoOperations(undoGroup);
        EditorUtility.DisplayDialog("v" + VERSION,
            "Layout applied.\n\nDictionary rebuilt as 'Klingon Lexicon' panel.\n" +
            "See RebuildDictionary() comments for DialogueManager wiring.\n\n" +
            "Ctrl+Z to undo.", "OK");
    }

    // =====================================================================
    // DICTIONARY REBUILD
    // =====================================================================
    //
    // New hierarchy under DictionaryPanel:
    //
    //  DictionaryPanel
    //  ├─ DictTitleRow          ← "Klingon Lexicon" heading
    //  ├─ DictTitleBorder       ← 3-px black rule below title
    //  ├─ DictColHeaders        ← "Discovered Terms" | "Translation Details"
    //  ├─ DictColHeaderBorder   ← 3-px black rule below headers
    //  ├─ DictColDivider        ← 3-px vertical black rule at 30 % x
    //  ├─ DictLeftCol           ← word-list buttons (DictBtn1–5 moved here)
    //  ├─ DictRightCol          ← detail area
    //  │   └─ DictDetailCard    ← tan rounded card
    //  │       ├─ DictDefaultMsg        (active on open)
    //  │       ├─ DictSelectedView      (activate when a KNOWN word is clicked)
    //  │       │   ├─ DictSelectedTitle
    //  │       │   ├─ DictKlingonText   ← set by DialogueManager: "Klingon: qih"
    //  │       │   ├─ DictEnglishText   ← set by DialogueManager: "English: strike / hit"
    //  │       │   ├─ DictContextHeader
    //  │       │   ├─ DictContextText   ← set by DialogueManager
    //  │       │   ├─ DictStatusHeader
    //  │       │   ├─ DictStatusText    ← set by DialogueManager
    //  │       │   ├─ DictNotesHeader
    //  │       │   └─ DictNotesText     ← set by DialogueManager
    //  │       └─ DictUndiscoveredMsg   (activate when a LOCKED word is clicked)
    //  └─ DictFooter
    //      ├─ LookUpsText       (moved here from root)
    //      └─ DictHelpBtn
    //
    // ── DialogueManager wiring ───────────────────────────────────────────
    // On DictionaryLookup(int index):
    //   1. Find "DictDetailCard" and set its Image.color:
    //        known word   → C_CREAM  (new Color(0.996f, 0.945f, 0.863f))
    //        undiscovered → C_MANILA (new Color(0.906f, 0.788f, 0.663f))
    //        none selected → C_MANILA
    //   2. SetActive the correct state child:
    //        DictDefaultMsg        — nothing selected
    //        DictSelectedView      — known word
    //        DictUndiscoveredMsg   — not yet discovered
    //   3. When showing DictSelectedView, populate:
    //        DictKlingonText.text  = "<b>Klingon:</b> " + entry.klingonWord;
    //        DictEnglishText.text  = "<b>English:</b> " + entry.translation;
    //        DictContextText.text  = entry.context;    (add field to DictEntry if needed)
    //        DictStatusText.text   = entry.translated ? "Confirmed" : "Unconfirmed";
    //        DictNotesText.text    = entry.notes;      (add field to DictEntry if needed)
    //   4. Highlight the active button: set its Image.color to C_CREAM,
    //      reset all others to C_MANILA.
    //
    // Fonts: assign "Fondamento" (or your imported serif) to DictTitleText
    //        in the Inspector for full wireframe fidelity.
    //        All other labels use the project default TMP font.
    // =====================================================================

    static void RebuildDictionary()
    {
        // ── Panel: near-fullscreen overlay ──────────────────────────────────
        FixRT("DictionaryPanel", rt => {
            Anchor(rt, 0.02f, 0.04f, 0.98f, 0.96f);
            rt.offsetMin = Vector2.zero;
            rt.offsetMax = Vector2.zero;
        });
        SafeDestroy<GridLayoutGroup>("DictionaryPanel");
        SafeDestroy<VerticalLayoutGroup>("DictionaryPanel");
        SafeDestroy<HorizontalLayoutGroup>("DictionaryPanel");
        SetBg("DictionaryPanel", C_WHITE);

        var dictGO = Find("DictionaryPanel");
        if (dictGO == null) return;

        // ── Title row (86 %–100 %) ──────────────────────────────────────────
        var titleRowGO = GetOrCreateChild(dictGO, "DictTitleRow");
        FixRT(titleRowGO, rt => {
            Anchor(rt, 0, 0.86f, 1, 1);
            rt.offsetMin = Vector2.zero; rt.offsetMax = Vector2.zero;
        });
        EnsureBg(titleRowGO, C_WHITE);

        var titleTextGO = GetOrCreateChild(titleRowGO, "DictTitleText");
        FixRT(titleTextGO, rt => {
            Anchor(rt, 0, 0, 1, 1);
            rt.offsetMin = new Vector2(22, 4); rt.offsetMax = new Vector2(-22, -4);
        });
        var titleTMP = EnsureComp<TextMeshProUGUI>(titleTextGO);
        Record(titleTMP);
        titleTMP.text              = "Klingon Lexicon";
        titleTMP.enableAutoSizing  = true;
        titleTMP.fontSizeMin       = 24;
        titleTMP.fontSizeMax       = 56;
        titleTMP.color             = C_BLACK;
        titleTMP.alignment         = TextAlignmentOptions.MidlineLeft;
        titleTMP.enableWordWrapping = false;
        // ↑ Assign "Fondamento" TMP font asset in the Inspector for full wireframe match.

        // ── Title bottom border ─────────────────────────────────────────────
        var titleBorderGO = GetOrCreateChild(dictGO, "DictTitleBorder");
        FixRT(titleBorderGO, rt => {
            Anchor(rt, 0, 0.856f, 1, 0.856f);
            rt.sizeDelta = new Vector2(0, 3);
        });
        EnsureBg(titleBorderGO, C_BLACK);

        // ── Column headers row (78 %–85.6 %) ───────────────────────────────
        var colHeadersGO = GetOrCreateChild(dictGO, "DictColHeaders");
        FixRT(colHeadersGO, rt => {
            Anchor(rt, 0, 0.78f, 1, 0.856f);
            rt.offsetMin = Vector2.zero; rt.offsetMax = Vector2.zero;
        });
        EnsureBg(colHeadersGO, C_WHITE);

        var leftHeaderGO = GetOrCreateChild(colHeadersGO, "DictHeaderLeft");
        FixRT(leftHeaderGO, rt => {
            Anchor(rt, 0, 0, 0.30f, 1);
            rt.offsetMin = new Vector2(15, 3); rt.offsetMax = new Vector2(-5, -3);
        });
        var lhTMP = EnsureComp<TextMeshProUGUI>(leftHeaderGO);
        Record(lhTMP);
        lhTMP.text      = "Discovered Terms";
        lhTMP.fontSize  = 16;
        lhTMP.fontStyle = FontStyles.Bold;
        lhTMP.color     = C_BLACK;
        lhTMP.alignment = TextAlignmentOptions.MidlineLeft;

        var rightHeaderGO = GetOrCreateChild(colHeadersGO, "DictHeaderRight");
        FixRT(rightHeaderGO, rt => {
            Anchor(rt, 0.30f, 0, 1, 1);
            rt.offsetMin = new Vector2(15, 3); rt.offsetMax = new Vector2(-15, -3);
        });
        var rhTMP = EnsureComp<TextMeshProUGUI>(rightHeaderGO);
        Record(rhTMP);
        rhTMP.text      = "Translation Details";
        rhTMP.fontSize  = 16;
        rhTMP.fontStyle = FontStyles.Bold;
        rhTMP.color     = C_BLACK;
        rhTMP.alignment = TextAlignmentOptions.MidlineLeft;

        // ── Column header bottom border ─────────────────────────────────────
        var colBorderGO = GetOrCreateChild(dictGO, "DictColHeaderBorder");
        FixRT(colBorderGO, rt => {
            Anchor(rt, 0, 0.776f, 1, 0.776f);
            rt.sizeDelta = new Vector2(0, 3);
        });
        EnsureBg(colBorderGO, C_BLACK);

        // ── Vertical column divider ─────────────────────────────────────────
        var vDivGO = GetOrCreateChild(dictGO, "DictColDivider");
        FixRT(vDivGO, rt => {
            Anchor(rt, 0.30f, 0.08f, 0.30f, 0.776f);
            rt.sizeDelta = new Vector2(3, 0);
        });
        EnsureBg(vDivGO, C_BLACK);

        // ── Left column — word buttons (8 %–77.6 %) ────────────────────────
        var leftColGO = GetOrCreateChild(dictGO, "DictLeftCol");
        FixRT(leftColGO, rt => {
            Anchor(rt, 0, 0.08f, 0.30f, 0.776f);
            rt.offsetMin = Vector2.zero; rt.offsetMax = Vector2.zero;
        });
        EnsureBg(leftColGO, C_WHITE);
        SafeDestroyOn<GridLayoutGroup>(leftColGO);
        SafeDestroyOn<HorizontalLayoutGroup>(leftColGO);

        var leftVLG = EnsureComp<VerticalLayoutGroup>(leftColGO);
        Record(leftVLG);
        leftVLG.spacing             = 10;
        leftVLG.padding             = new RectOffset(12, 12, 16, 10);
        leftVLG.childControlWidth   = true;
        leftVLG.childControlHeight  = true;   // VLG reads LayoutElement minHeight
        leftVLG.childForceExpandWidth  = true;
        leftVLG.childForceExpandHeight = false;
        leftVLG.childAlignment      = TextAnchor.UpperCenter;

        // Move DictBtn1-5 into left column and restyle as pill buttons
        for (int i = 1; i <= 5; i++)
        {
            var btn = Find("DictBtn" + i);
            if (btn == null) continue;
            if (btn.transform.parent != leftColGO.transform)
                Undo.SetTransformParent(btn.transform, leftColGO.transform, "Move DictBtn" + i + " → DictLeftCol");
            StyleWordButton(btn, i);
        }

        // ── Right column — detail area (8 %–77.6 %) ────────────────────────
        var rightColGO = GetOrCreateChild(dictGO, "DictRightCol");
        FixRT(rightColGO, rt => {
            Anchor(rt, 0.30f, 0.08f, 1f, 0.776f);
            rt.offsetMin = Vector2.zero; rt.offsetMax = Vector2.zero;
        });
        EnsureBg(rightColGO, C_WHITE);

        // Detail card (assign a rounded-rect Sliced sprite in Inspector for pill corners)
        var cardGO = GetOrCreateChild(rightColGO, "DictDetailCard");
        FixRT(cardGO, rt => {
            Anchor(rt, 0.05f, 0.07f, 0.93f, 0.93f);
            rt.offsetMin = Vector2.zero; rt.offsetMax = Vector2.zero;
        });
        var cardImg = EnsureComp<Image>(cardGO);
        Record(cardImg);
        cardImg.color     = C_MANILA;
        cardImg.type      = Image.Type.Sliced;
        // ↑ Assign a 9-slice rounded-rect sprite here for full wireframe look.

        // ── Card state: DEFAULT ─────────────────────────────────────────────
        var defaultGO = GetOrCreateChild(cardGO, "DictDefaultMsg");
        FixRT(defaultGO, rt => {
            Anchor(rt, 0, 0, 1, 1);
            rt.offsetMin = new Vector2(28, 22); rt.offsetMax = new Vector2(-28, -22);
        });
        var defTMP = EnsureComp<TextMeshProUGUI>(defaultGO);
        Record(defTMP);
        defTMP.text = "Select a discovered Klingon term to reveal its English meaning, " +
                      "context and contextual notes.\n\n<b>No Entry Selected</b>";
        defTMP.enableAutoSizing  = true;
        defTMP.fontSizeMin       = 14;
        defTMP.fontSizeMax       = 26;
        defTMP.color             = C_BLACK;
        defTMP.alignment         = TextAlignmentOptions.Center;
        defTMP.enableWordWrapping = true;
        defaultGO.SetActive(true);   // shown on open

        // ── Card state: SELECTED (known word) ──────────────────────────────
        var selectedGO = GetOrCreateChild(cardGO, "DictSelectedView");
        FixRT(selectedGO, rt => {
            Anchor(rt, 0, 0, 1, 1);
            rt.offsetMin = new Vector2(28, 18); rt.offsetMax = new Vector2(-28, -18);
        });
        SafeDestroyOn<HorizontalLayoutGroup>(selectedGO);
        var selVLG = EnsureComp<VerticalLayoutGroup>(selectedGO);
        Record(selVLG);
        selVLG.spacing              = 5;
        selVLG.padding              = new RectOffset(0, 0, 4, 4);
        selVLG.childControlWidth    = true;
        selVLG.childControlHeight   = true;
        selVLG.childForceExpandWidth  = true;
        selVLG.childForceExpandHeight = false;

        MakeDictField(selectedGO, "DictSelectedTitle",  "<b>Selected Entry:</b>", 15);
        MakeDictField(selectedGO, "DictKlingonText",    "<b>Klingon:</b> —",      14);
        MakeDictField(selectedGO, "DictEnglishText",    "<b>English:</b> —",      14);
        MakeDictField(selectedGO, "DictContextHeader",  "<b>Context:</b>",         14);
        MakeDictField(selectedGO, "DictContextText",    "—",                       13);
        MakeDictField(selectedGO, "DictStatusHeader",   "<b>Status:</b>",          14);
        MakeDictField(selectedGO, "DictStatusText",     "—",                       13);
        MakeDictField(selectedGO, "DictNotesHeader",    "<b>Notes:</b>",           14);
        MakeDictField(selectedGO, "DictNotesText",      "—",                       13);
        selectedGO.SetActive(false);  // hidden until DialogueManager activates it

        // ── Card state: UNDISCOVERED ────────────────────────────────────────
        var undiscGO = GetOrCreateChild(cardGO, "DictUndiscoveredMsg");
        FixRT(undiscGO, rt => {
            Anchor(rt, 0, 0, 1, 1);
            rt.offsetMin = new Vector2(28, 22); rt.offsetMax = new Vector2(-28, -22);
        });
        var undiscTMP = EnsureComp<TextMeshProUGUI>(undiscGO);
        Record(undiscTMP);
        undiscTMP.text = "<b>Entry Unavailable:</b>\nThis term has not been fully " +
                         "discovered yet through gameplay.";
        undiscTMP.enableAutoSizing  = true;
        undiscTMP.fontSizeMin       = 16;
        undiscTMP.fontSizeMax       = 28;
        undiscTMP.color             = C_BLACK;
        undiscTMP.alignment         = TextAlignmentOptions.Center;
        undiscTMP.enableWordWrapping = true;
        undiscGO.SetActive(false);    // hidden until DialogueManager activates it

        // ── Footer — lookup counter + Help button (0 %–8 %) ────────────────
        var footerGO = GetOrCreateChild(dictGO, "DictFooter");
        FixRT(footerGO, rt => {
            Anchor(rt, 0, 0, 1, 0.08f);
            rt.offsetMin = Vector2.zero; rt.offsetMax = Vector2.zero;
        });
        EnsureBg(footerGO, C_WHITE);

        // Move LookUpsText into footer (was a root child of DictionaryPanel)
        var lutGO = Find("LookUpsText");
        if (lutGO != null)
        {
            if (lutGO.transform.parent != footerGO.transform)
                Undo.SetTransformParent(lutGO.transform, footerGO.transform, "Move LookUpsText → DictFooter");
            FixRT(lutGO, rt => {
                Anchor(rt, 0, 0, 0.6f, 1);
                rt.offsetMin = new Vector2(20, 0); rt.offsetMax = Vector2.zero;
            });
            var lutTMP = lutGO.GetComponent<TextMeshProUGUI>();
            if (lutTMP != null)
            {
                Record(lutTMP);
                lutTMP.fontSize  = 14;
                lutTMP.color     = new Color(0.25f, 0.25f, 0.25f, 1f);
                lutTMP.alignment = TextAlignmentOptions.MidlineLeft;
            }
        }

        // Help button (bottom-right corner per wireframe)
        var helpGO = GetOrCreateChild(footerGO, "DictHelpBtn");
        FixRT(helpGO, rt => {
            Anchor(rt, 1, 0.5f, 1, 0.5f);
            rt.sizeDelta        = new Vector2(80, 36);
            rt.anchoredPosition = new Vector2(-20, 0);
        });
        var helpImg = EnsureComp<Image>(helpGO);
        Record(helpImg);
        helpImg.color = new Color(0.72f, 0.76f, 0.90f, 1f); // soft blue-grey per wireframe
        if (helpGO.GetComponent<Button>() == null)
            Undo.AddComponent<Button>(helpGO);

        // TMP must live on a child — Image and TextMeshProUGUI both extend Graphic
        // and Unity won't allow two Graphic components on the same GameObject.
        var helpLabelGO = GetOrCreateChild(helpGO, "DictHelpBtnLabel");
        FixRT(helpLabelGO, rt => {
            Anchor(rt, 0, 0, 1, 1);
            rt.offsetMin = Vector2.zero; rt.offsetMax = Vector2.zero;
        });
        var helpTMP = EnsureComp<TextMeshProUGUI>(helpLabelGO);
        if (helpTMP != null)
        {
            Record(helpTMP);
            helpTMP.text      = "Help";
            helpTMP.fontSize  = 14;
            helpTMP.color     = C_BLACK;
            helpTMP.alignment = TextAlignmentOptions.Center;
        }
    }

    // =====================================================================
    // DICTIONARY HELPERS
    // =====================================================================

    /// <summary>
    /// Styles a DictBtn as a tan pill-shaped word entry button.
    /// Raycast tags are untouched — PlayerController and DialogueManager
    /// continue to call DictionaryLookup(i) as before.
    /// For a true pill shape, assign a rounded-rect Sliced sprite in Inspector.
    /// To highlight the active button at runtime:
    ///   active   → img.color = C_CREAM  (#FEF1DC)
    ///   inactive → img.color = C_MANILA (#E7C9A9)
    /// </summary>
    static void StyleWordButton(GameObject go, int index)
    {
        // Reset anchors to a clean centre-point so the VLG takes full control.
        var rt = go.GetComponent<RectTransform>();
        if (rt != null)
        {
            Record(rt);
            rt.anchorMin = new Vector2(0.5f, 0.5f);
            rt.anchorMax = new Vector2(0.5f, 0.5f);
            rt.sizeDelta = new Vector2(0, 44);
        }

        var img = go.GetComponent<Image>();
        if (img != null) { Record(img); img.color = C_MANILA; img.type = Image.Type.Sliced; }

        // TMP may live directly on the button or on a child
        var tmp = go.GetComponent<TextMeshProUGUI>();
        if (tmp == null) tmp = go.GetComponentInChildren<TextMeshProUGUI>();
        if (tmp != null)
        {
            Record(tmp);
            if (string.IsNullOrEmpty(tmp.text))
                tmp.text = "• — (DictBtn" + index + ")";
            tmp.enableAutoSizing  = true;
            tmp.fontSizeMin       = 12;
            tmp.fontSizeMax       = 18;
            tmp.color             = C_BLACK;
            tmp.alignment         = TextAlignmentOptions.MidlineLeft;
            tmp.enableWordWrapping = false;
            tmp.margin            = new Vector4(14, 0, 8, 0); // left bullet indent
        }

        var le = go.GetComponent<LayoutElement>();
        if (le == null) le = Undo.AddComponent<LayoutElement>(go);
        Record(le);
        le.minHeight       = 44;
        le.preferredHeight = 44;
        le.flexibleWidth   = 1;
    }

    /// <summary>
    /// Creates or refreshes a single text row inside DictSelectedView.
    /// A LayoutElement is added so the VerticalLayoutGroup sizes it correctly.
    /// </summary>
    static void MakeDictField(GameObject parent, string name, string defaultText, int fontSize)
    {
        var go = GetOrCreateChild(parent, name);
        if (go == null) { Debug.LogWarning("[FixInterviewLayout] Could not find/create: " + name); return; }

        var tmp = EnsureComp<TextMeshProUGUI>(go);
        if (tmp == null) { Debug.LogWarning("[FixInterviewLayout] No TMP on: " + name); return; }
        Record(tmp);
        tmp.text               = defaultText;
        tmp.fontSize           = fontSize;
        tmp.color              = C_BLACK;
        tmp.enableWordWrapping = true;
        tmp.alignment          = TextAlignmentOptions.TopLeft;

        var le = EnsureComp<LayoutElement>(go);
        if (le == null) return;
        Record(le);
        le.flexibleWidth = 1;
    }

    // =====================================================================
    // SHARED HELPERS (original + new additions)
    // =====================================================================

    static GameObject Find(string name)
    {
        foreach (var rt in Resources.FindObjectsOfTypeAll<RectTransform>())
            if (rt != null && rt.gameObject != null &&
                rt.gameObject.name == name && rt.gameObject.scene.isLoaded)
                return rt.gameObject;
        return null;
    }

    /// <summary>
    /// Returns the named direct child of parent, re-parenting a globally
    /// found object if necessary, or creating a fresh UI GameObject.
    /// </summary>
    static GameObject GetOrCreateChild(GameObject parent, string name)
    {
        // 1. Already a direct child — nothing to do
        var existing = parent.transform.Find(name);
        if (existing != null) return existing.gameObject;

        // 2. Exists elsewhere in the scene — move it under parent
        var global = Find(name);
        if (global != null)
        {
            if (global.transform.parent != parent.transform)
                Undo.SetTransformParent(global.transform, parent.transform, "Parent " + name);
            return global;
        }

        // 3. Brand-new UI element
        var go = new GameObject(name, typeof(RectTransform));
        Undo.RegisterCreatedObjectUndo(go, "Create " + name);
        Undo.SetTransformParent(go.transform, parent.transform, "Parent " + name);
        go.transform.localScale    = Vector3.one;
        go.transform.localPosition = Vector3.zero;
        return go;
    }

    static T EnsureComp<T>(GameObject go) where T : Component
    {
        var c = go.GetComponent<T>();
        if (c != null) return c;
        // Undo.AddComponent can return null on freshly created objects due to
        // Editor undo-system timing; fall back to regular AddComponent if so.
        try { c = Undo.AddComponent<T>(go); } catch { }
        if (c == null) c = go.AddComponent<T>();
        return c;
    }

    // Ensures an Image component exists and sets its color
    static void EnsureBg(GameObject go, Color c)
    {
        if (go == null) return;
        var img = EnsureComp<Image>(go);
        Record(img);
        img.color = c;
    }

    static void Record(Object obj) { if (obj != null) Undo.RecordObject(obj, "Fix"); }

    // Overload: fix by name
    static void FixRT(string name, System.Action<RectTransform> action)
    {
        var go = Find(name);
        FixRT(go, action);
    }

    // Overload: fix by reference (used for newly created objects)
    static void FixRT(GameObject go, System.Action<RectTransform> action)
    {
        if (go == null) return;
        var rt = go.GetComponent<RectTransform>();
        if (rt == null) return;
        Record(rt);
        action(rt);
    }

    static void Anchor(RectTransform rt, float minX, float minY, float maxX, float maxY)
    {
        rt.anchorMin = new Vector2(minX, minY);
        rt.anchorMax = new Vector2(maxX, maxY);
    }

    static void StretchAll(string name)
    {
        FixRT(name, rt => {
            Anchor(rt, 0, 0, 1, 1);
            rt.offsetMin = Vector2.zero;
            rt.offsetMax = Vector2.zero;
        });
    }

    static void SetBg(string name, Color c)
    {
        var go = Find(name);
        if (go == null) return;
        var img = go.GetComponent<Image>();
        if (img == null) return;
        Record(img);
        img.color = c;
    }

    static void AutoTMP(string name, int min, int max,
                        TextAlignmentOptions align, Color color, string overrideText)
    {
        var go = Find(name); if (go == null) return;
        var tmp = go.GetComponent<TextMeshProUGUI>(); if (tmp == null) return;
        Record(tmp);
        tmp.enableAutoSizing  = true;
        tmp.fontSizeMin       = min;
        tmp.fontSizeMax       = max;
        tmp.alignment         = align;
        tmp.color             = color;
        tmp.enableWordWrapping = true;
        if (overrideText != null) tmp.text = overrideText;
    }

    // Destroys a component by searching the scene by name
    static void SafeDestroy<T>(string name) where T : Component
    {
        var go = Find(name); if (go == null) return;
        SafeDestroyOn<T>(go);
    }

    // Destroys a component on a known GameObject reference
    static void SafeDestroyOn<T>(GameObject go) where T : Component
    {
        if (go == null) return;
        var c = go.GetComponent<T>(); if (c == null) return;
        Undo.DestroyObjectImmediate(c);
    }

    static void FixOptBtn(string name)
    {
        var go = Find(name); if (go == null) return;
        var tmp = go.GetComponent<TextMeshProUGUI>();
        if (tmp == null)
        {
            var child = go.GetComponentInChildren<TextMeshProUGUI>();
            string t  = child != null ? child.text : "";
            tmp       = Undo.AddComponent<TextMeshProUGUI>(go);
            tmp.text  = t;
            if (child != null && child.gameObject != go)
                Undo.DestroyObjectImmediate(child.gameObject);
        }
        Record(tmp);
        tmp.enableAutoSizing  = true; tmp.fontSizeMin = 11; tmp.fontSizeMax = 18;
        tmp.color             = Color.white;
        tmp.alignment         = TextAlignmentOptions.Center;
        tmp.enableWordWrapping = true;
        tmp.overflowMode      = TextOverflowModes.Ellipsis;
        var img = go.GetComponent<Image>();
        if (img != null) { Record(img); img.color = new Color(0.2f, 0.2f, 0.2f, 0.9f); }
        var le = go.GetComponent<LayoutElement>();
        if (le != null) Undo.DestroyObjectImmediate(le);
    }
}
#endif

