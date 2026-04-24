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

    [Header("Screen Panels")]
    public GameObject briefingPanel;
    public GameObject interviewPanel;
    public GameObject summaryPanel;
    public GameObject endOfDayPanel;
    public TextMeshProUGUI endOfDaySummaryText;

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

    private string rawMainText = "";
    private string rawOpt1 = "";
    private string rawOpt2 = "";
    private string rawOpt3 = "";
    private string rawOpt4 = "";

    // Snapshot of option-button active states when an interview is paused mid-dialogue
    private bool _snapOpt1, _snapOpt2, _snapOpt3, _snapOpt4;

    // =====================================================================
    // START
    // =====================================================================

    public void Start()
    {
        HideAllPanels();

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
        // ── Move Dictionary button away from status text ──────────────────
        // Status text (triesText) sits in the top-right of the TopBar.
        // Move the Dictionary toggle to the far right EDGE so they don't overlap.
        if (dictionaryToggleBtn != null)
        {
            var rt = dictionaryToggleBtn.GetComponent<RectTransform>();
            if (rt != null)
            {
                rt.anchorMin        = new Vector2(1f, 1f);
                rt.anchorMax        = new Vector2(1f, 1f);
                rt.pivot            = new Vector2(1f, 1f);
                rt.anchoredPosition = new Vector2(-4f, -4f);   // flush to top-right corner
                rt.sizeDelta        = new Vector2(90f, 26f);   // compact size
            }
        }
        // Give triesText a right margin so it never slides behind the button
        if (triesText != null)
        {
            var rt = triesText.GetComponent<RectTransform>();
            if (rt != null)
            {
                // Shrink the right edge inward by ~100 px to leave room for the button
                rt.offsetMax = new Vector2(-100f, rt.offsetMax.y);
            }
        }

        // ── Responsive font sizing ────────────────────────────────────────
        // Scale relative to a 1080p reference height, clamped to a readable range.
        // This keeps text legible on everything from a 720p laptop to a 4K monitor.
        ApplyResponsiveFontSizes();
    }

    private void HideAllPanels()
    {
        if (briefingPanel   != null) briefingPanel.SetActive(false);
        if (interviewPanel  != null) interviewPanel.SetActive(false);
        if (summaryPanel    != null) summaryPanel.SetActive(false);
        if (endOfDayPanel   != null) endOfDayPanel.SetActive(false);
        if (dictionaryPanel != null) dictionaryPanel.SetActive(false);
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

        nameText.text  = current.CharacterName;
        triesText.text = "Pick your questions wisely";
        triesText.gameObject.SetActive(true);
        dictionaryLookUpsText.text = "Lookups: " + dictionaryLookUps;

        SetReturnButtonVisible(true);
        InitDictionarySlots();
        ShowAllOptions();

        current.DialogueSetter(0f, this);
        RefreshDictionaryVisibility();

        // ── CHANGE 1: notify GameStateManager ────────────────────────────
        if (GameStateManager.Instance != null)
            GameStateManager.Instance.OnInterviewStarted(current);
    }

    public void StartInterviewByIndex(int index)
    {
        if (availableInterviews != null && index >= 0 && index < availableInterviews.Length)
            StartInterview(availableInterviews[index]);
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
        foreach (var entry in current.DictionaryEntries)
        {
            if (entry.translated)
            {
                result = result.Replace(entry.klingonWord, entry.translation);
                if (entry.altSpellings != null)
                    foreach (var alt in entry.altSpellings)
                        result = result.Replace(alt, entry.translation);
            }
            else if (entry.seen)
            {
                result = result.Replace(entry.klingonWord, entry.klingonWord + " (?)");
                if (entry.altSpellings != null)
                    foreach (var alt in entry.altSpellings)
                        result = result.Replace(alt, alt + " (?)");
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
                if (allText.Contains(entry.klingonWord)) entry.seen = true;
                if (entry.altSpellings != null)
                    foreach (var alt in entry.altSpellings)
                        if (allText.Contains(alt)) entry.seen = true;
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
    }

    private void ScrollToTop()
    {
        if (dialogueScroll != null)
        {
            Canvas.ForceUpdateCanvases();
            dialogueScroll.verticalNormalizedPosition = 1f;
        }
    }

    // =====================================================================
    // DICTIONARY
    // =====================================================================

    public void ToggleDictionary()
    {
        if (dictionaryPanel != null)
            dictionaryPanel.SetActive(!dictionaryPanel.activeSelf);
    }

    private void InitDictionarySlots()
    {
        if (current == null || dictionarySlots == null) return;
        for (int i = 0; i < dictionarySlots.Length; i++)
        {
            if (i < current.DictionaryEntries.Length)
                dictionarySlots[i].gameObject.SetActive(true);
            else
                dictionarySlots[i].gameObject.SetActive(false);
        }
    }

    private void RefreshDictionaryVisibility()
    {
        if (current == null || dictionarySlots == null) return;
        for (int i = 0; i < dictionarySlots.Length; i++)
        {
            if (i < current.DictionaryEntries.Length)
            {
                var e = current.DictionaryEntries[i];
                dictionarySlots[i].gameObject.SetActive(e.seen || e.translated);
            }
            else
                dictionarySlots[i].gameObject.SetActive(false);
        }
    }

    public void DictionaryLookup(int oneBasedIndex)
    {
        if (current == null) return;
        int i = oneBasedIndex - 1;
        if (i < 0 || i >= current.DictionaryEntries.Length) return;

        var entry = current.DictionaryEntries[i];
        if (!entry.seen || entry.translated || dictionaryLookUps <= 0) return;

        dictionaryLookUps--;
        dictionaryLookUpsText.text = "Lookups: " + dictionaryLookUps;
        entry.translated = true;
        dictionarySlots[i].text = entry.klingonWord + " = " + entry.translation;

        ApplyTranslations();
        RefreshDictionaryVisibility();
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
                        // Game over — trigger ending
                        if (endingManager != null)
                            endingManager.TriggerEnding(marsOpinionScore);
                        HideAllPanels();
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

        string built = "";
        for (int i = 0; i < articleLines.Count; i++)
            built += articleLines[i] + "\n\n";
        if (articleLines.Count > 0)
            built += "\u2014\u2014\u2014\u2014\n\n";

        built += $"[{currentParagraphIndex + 1}/{currentArticle.paragraphs.Length}] {para.promptText}";

        SetDialogueTexts(built,
            para.truthful.text,
            para.dishonest.text,
            para.ambitious.text,
            "");

        optionOne.gameObject.SetActive(true);
        optionTwo.gameObject.SetActive(true);
        optionThree.gameObject.SetActive(true);
        optionFour.gameObject.SetActive(false);

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

        SetDialogueTexts(fullArticle, "", "", "", "");

        optionOne.gameObject.SetActive(true);
        optionOne.text = "Publish";
        optionTwo.gameObject.SetActive(false);
        optionThree.gameObject.SetActive(false);
        optionFour.gameObject.SetActive(false);

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
        // Minimum clamp is 0.8 (never smaller than 80% of base) so the Unity
        // editor Game window at small sizes doesn't shrink text to unreadable.
        float scale = Mathf.Clamp(Screen.height / 1080f, 0.8f, 2.0f);

        float sizeMain    = Mathf.Round(34f * scale);   // dialogue body
        float sizeOptions = Mathf.Round(26f * scale);   // answer buttons
        float sizeName    = Mathf.Round(26f * scale);   // NPC name / status bar

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
