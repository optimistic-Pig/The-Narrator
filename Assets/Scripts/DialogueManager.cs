using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Character-agnostic dialogue UI controller.
/// Handles Canvas panels, phase flow, translation, dictionary, headlines,
/// paragraph-by-paragraph article writing, scoring, and day management.
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

    // =====================================================================
    // SCORING
    // =====================================================================

    [HideInInspector] public int marsOpinionScore = 0;
    [HideInInspector] public int truthfulCount = 0;
    [HideInInspector] public int dishonestCount = 0;
    [HideInInspector] public int ambitiousCount = 0;

    // =====================================================================
    // DAY MANAGEMENT
    // =====================================================================

    [HideInInspector] public int currentDay = 1;
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
    // -1 means that slot is empty / unused
    private readonly int[] headlineSlotToIndex = new int[4] { -1, -1, -1, -1 };

    // The NPC the player clicked on — set by ShowBriefing(), used by BeginInterview()
    private InterviewBase pendingInterview;

    // Raw text for re-translation on dictionary lookup
    private string rawMainText = "";
    private string rawOpt1 = "";
    private string rawOpt2 = "";
    private string rawOpt3 = "";
    private string rawOpt4 = "";

    // =====================================================================
    // START
    // =====================================================================

    public void Start()
    {
        HideAllPanels();
    }

    private void HideAllPanels()
    {
        if (briefingPanel != null)   briefingPanel.SetActive(false);
        if (interviewPanel != null)  interviewPanel.SetActive(false);
        if (summaryPanel != null)    summaryPanel.SetActive(false);
        if (endOfDayPanel != null)   endOfDayPanel.SetActive(false);
        if (dictionaryPanel != null) dictionaryPanel.SetActive(false);
    }

    /// <summary>
    /// Call from PlayerController when the player clicks an NPC.
    /// Pass the NPC's InterviewBase so BeginInterview() knows who to start.
    /// </summary>
    public void ShowBriefing(InterviewBase interview = null)
    {
        if (interview != null) pendingInterview = interview;
        HideAllPanels();
        if (briefingPanel != null) briefingPanel.SetActive(true);
    }

    public void ReturnToOffice()
    {
        HideAllPanels();
    }

    // =====================================================================
    // INTERVIEW LIFECYCLE
    // =====================================================================

    public void StartInterview(InterviewBase interview)
    {
        current = interview;
        current.ResetState();
        currentPhase = Phase.Dialogue;
        dictionaryLookUps = current.StartingLookups;
        articleChosen = 0f;

        rawMainText = ""; rawOpt1 = ""; rawOpt2 = ""; rawOpt3 = ""; rawOpt4 = "";

        if (briefingPanel != null) briefingPanel.SetActive(false);
        if (endOfDayPanel != null) endOfDayPanel.SetActive(false);
        if (interviewPanel != null) interviewPanel.SetActive(true);
        if (summaryPanel != null) summaryPanel.SetActive(false);
        if (dictionaryPanel != null) dictionaryPanel.SetActive(false);

        nameText.text = current.CharacterName;
        triesText.text = "Pick your questions wisely";
        triesText.gameObject.SetActive(true);
        dictionaryLookUpsText.text = "Lookups: " + dictionaryLookUps;

        InitDictionarySlots();
        ShowAllOptions();

        current.DialogueSetter(0f, this);
        RefreshDictionaryVisibility();
    }

    public void StartInterviewByIndex(int index)
    {
        if (availableInterviews != null && index >= 0 && index < availableInterviews.Length)
            StartInterview(availableInterviews[index]);
    }

    /// <summary>
    /// Wire this to the "Begin Interview" button on the briefing panel.
    /// Starts whichever NPC the player clicked on.
    /// </summary>
    public void BeginInterview()
    {
        if (pendingInterview != null)
            StartInterview(pendingInterview);
        else
            StartInterviewByIndex(0);   // fallback
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
        if (main != null) rawMainText = main;
        if (opt1 != null) rawOpt1 = opt1;
        if (opt2 != null) rawOpt2 = opt2;
        if (opt3 != null) rawOpt3 = opt3;
        if (opt4 != null) rawOpt4 = opt4;

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
        mainText.text  = TranslateText(rawMainText);
        optionOne.text  = TranslateText(rawOpt1);
        optionTwo.text  = TranslateText(rawOpt2);
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

                bool allOff = !optionOne.gameObject.activeSelf &&
                              !optionTwo.gameObject.activeSelf &&
                              !optionThree.gameObject.activeSelf &&
                              !optionFour.gameObject.activeSelf;

                if (allOff)
                {
                    currentPhase = Phase.EndTransition;
                    optionOne.gameObject.SetActive(true);
                    optionOne.text = "Continue";
                    triesText.text = "Interview complete";
                }
                else if (current.LastQuestionNodes.Contains(current.dialogueIndexTracker))
                {
                    triesText.text = "Last Question";
                }
                break;

            // ── EndTransition → Headline ──────────────────────────────────
            case Phase.EndTransition:
                currentPhase = Phase.Headline;

                if (summaryPanel != null)
                {
                    if (interviewPanel != null) interviewPanel.SetActive(false);
                    summaryPanel.SetActive(true);
                }

                ShowAllOptions();
                triesText.gameObject.SetActive(false);
                BuildAndShowHeadlines();
                break;

            // ── Headline → ArticleWriting ─────────────────────────────────
            case Phase.Headline:
                int slot = Mathf.RoundToInt(option) - 1;   // button 1-4 → slot 0-3
                if (slot < 0 || slot >= 4) return;
                int headlineIdx = headlineSlotToIndex[slot];
                if (headlineIdx < 0) return;                // empty slot, ignore

                articleChosen = headlineIdx;                // store actual headline index
                StartArticleWriting(headlineIdx);
                break;

            // ── ArticleWriting ────────────────────────────────────────────
            case Phase.ArticleWriting:
                HandleParagraphChoice(option);
                break;

            // ── ArticleComplete → DayEnd ──────────────────────────────────
            case Phase.ArticleComplete:
                ShowEndOfDay();
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

        bool dictOk = h.requiredDictIndex < 0 ||
                      (h.requiredDictIndex < current.DictionaryEntries.Length &&
                       current.DictionaryEntries[h.requiredDictIndex].translated);

        bool topicOk = h.requiredTopicIndex < 0 ||
                       (h.requiredTopicIndex < current.Topics.Length &&
                        current.Topics[h.requiredTopicIndex].encountered);

        return dictOk && topicOk && current.IsAdditionalHeadlineConditionMet(index);
    }

    /// <summary>
    /// Collects the first 4 unlocked headlines from the full Headlines array,
    /// assigns them to button slots 1-4, and shows the headline choice screen.
    /// Works regardless of how many total headlines the character has.
    /// </summary>
    private void BuildAndShowHeadlines()
    {
        // Reset slot mapping
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

        // Build button labels from the slot map
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

        int hIdx = Mathf.RoundToInt(articleChosen);
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
        triesText.text = $"Day {currentDay}  |  Score: {marsOpinionScore}  "
                       + $"(T:{truthfulCount}  D:{dishonestCount}  A:{ambitiousCount})";
    }

    // =====================================================================
    // END OF DAY
    // =====================================================================

    private void ShowEndOfDay()
    {
        currentPhase = Phase.DayEnd;

        if (current != null && !completedToday.Contains(current))
            completedToday.Add(current);

        if (interviewPanel != null) interviewPanel.SetActive(false);
        if (summaryPanel != null) summaryPanel.SetActive(false);

        if (endOfDayPanel != null)
        {
            endOfDayPanel.SetActive(true);
            if (endOfDaySummaryText != null && current != null)
            {
                string summary = current.GetEndOfDaySummary();
                summary += $"\n\nCumulative score: {marsOpinionScore}";
                endOfDaySummaryText.text = summary;
            }
        }

        optionOne.gameObject.SetActive(false);
        optionTwo.gameObject.SetActive(false);
        optionThree.gameObject.SetActive(false);
        optionFour.gameObject.SetActive(false);
    }

    /// <summary>
    /// Call from the end-of-day "Continue" button.
    /// Advances the day counter and returns to briefing.
    /// TODO Day 3: force-start Gorp's interview here instead of showing briefing.
    /// </summary>
    public void EndOfDayContinue()
    {
        currentDay++;

        if (endOfDayPanel != null) endOfDayPanel.SetActive(false);
        if (briefingPanel != null) briefingPanel.SetActive(true);
        currentPhase = Phase.Dialogue;
    }
}
