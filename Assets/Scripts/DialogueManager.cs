using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Character-agnostic dialogue UI controller.
/// Handles Canvas panels, phase flow, translation, dictionary, headlines,
/// and day management. The actual dialogue content comes from InterviewBase subclasses.
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
    public TextMeshProUGUI[] dictionarySlots;       // assign up to ~6 in inspector

    [Header("Screen Panels")]
    public GameObject briefingPanel;
    public GameObject interviewPanel;
    public GameObject summaryPanel;
    public GameObject endOfDayPanel;
    public TextMeshProUGUI endOfDaySummaryText;

    [Header("Interviews")]
    public InterviewBase[] availableInterviews;     // all interviewees for this day

    // =====================================================================
    // PRIVATE STATE
    // =====================================================================

    private enum Phase { Dialogue, EndTransition, Headline, Tone, Article, DayEnd }
    private Phase currentPhase = Phase.Dialogue;

    private InterviewBase current;                  // who we're talking to right now
    private int dictionaryLookUps = 0;
    private float articleChosen = 0f;
    private List<InterviewBase> completedToday = new List<InterviewBase>();

    // Raw (untranslated) text for live re-translation
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
        // Hide ALL dialogue UI on game start — player sees only the 3D world + crosshair
        HideAllPanels();
    }

    /// <summary>
    /// Hides every dialogue panel.
    /// </summary>
    private void HideAllPanels()
    {
        if (briefingPanel != null)   briefingPanel.SetActive(false);
        if (interviewPanel != null)  interviewPanel.SetActive(false);
        if (summaryPanel != null)    summaryPanel.SetActive(false);
        if (endOfDayPanel != null)   endOfDayPanel.SetActive(false);
        if (dictionaryPanel != null) dictionaryPanel.SetActive(false);
    }

    /// <summary>
    /// Call from PlayerController when the player clicks on an NPC.
    /// Shows the briefing screen with "Begin Interview" and "Return to Office" buttons.
    /// </summary>
    public void ShowBriefing()
    {
        HideAllPanels();
        if (briefingPanel != null) briefingPanel.SetActive(true);
    }

    /// <summary>
    /// Call from the "Return to Office" button on the briefing panel.
    /// Hides all UI so the player is back in the 3D world.
    /// PlayerController should also re-enable movement and lock the cursor.
    /// </summary>
    public void ReturnToOffice()
    {
        HideAllPanels();
    }

    // =====================================================================
    // INTERVIEW LIFECYCLE
    // =====================================================================

    /// <summary>
    /// Call to begin an interview with a specific character.
    /// Wire this to a character-select button or call from the briefing panel.
    /// </summary>
    public void StartInterview(InterviewBase interview)
    {
        current = interview;
        current.ResetState();
        currentPhase = Phase.Dialogue;
        dictionaryLookUps = current.StartingLookups;
        articleChosen = 0f;

        // Reset raw text
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

        // Run the character's opening dialogue
        current.DialogueSetter(0f, this);

        RefreshDictionaryVisibility();
    }

    /// <summary>
    /// Convenience: start interview at index in availableInterviews.
    /// Wire character-select buttons to this with the index parameter.
    /// </summary>
    public void StartInterviewByIndex(int index)
    {
        if (availableInterviews != null && index >= 0 && index < availableInterviews.Length)
            StartInterview(availableInterviews[index]);
    }

    /// <summary>
    /// No-parameter version for easy button wiring.
    /// Starts the first available interview. Use for "Begin Interview" buttons.
    /// </summary>
    public void BeginInterview()
    {
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

    /// <summary>
    /// Stores raw text, detects seen words and topics, applies translations.
    /// Called by each interview's DialogueSetter.
    /// </summary>
    public void SetDialogueTexts(string main, string opt1 = null, string opt2 = null, string opt3 = null, string opt4 = null)
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
        mainText.text = TranslateText(rawMainText);
        optionOne.text = TranslateText(rawOpt1);
        optionTwo.text = TranslateText(rawOpt2);
        optionThree.text = TranslateText(rawOpt3);
        optionFour.text = TranslateText(rawOpt4);
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
                //dictionarySlots[i].text = current.DictionaryEntries[i].klingonWord + " = ???";
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

    /// <summary>
    /// Call from dictionary entry buttons. Button index is 1-based
    /// and maps to the character's DictionaryEntries array (0-based).
    /// </summary>
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

            case Phase.EndTransition:
                currentPhase = Phase.Headline;

                if (summaryPanel != null)
                {
                    if (interviewPanel != null) interviewPanel.SetActive(false);
                    summaryPanel.SetActive(true);
                }

                ShowAllOptions();
                triesText.gameObject.SetActive(false);

                string[] hLabels = BuildHeadlineLabels();
                SetDialogueTexts("Choose your article headline",
                    hLabels[0], hLabels[1], hLabels[2], hLabels[3]);
                break;

            case Phase.Headline:
                int idx = Mathf.RoundToInt(option) - 1;
                if (idx < 0 || current == null || idx >= current.Headlines.Length) return;
                if (!IsHeadlineUnlocked(idx)) return;

                currentPhase = Phase.Tone;
                articleChosen = option;
                optionOne.gameObject.SetActive(true);
                optionTwo.gameObject.SetActive(true);
                optionThree.gameObject.SetActive(true);
                optionFour.gameObject.SetActive(false);
                SetDialogueTexts("What tone do you want to write with?",
                    "Pro Martian", "Neutral", "Pro Human", "");
                break;

            case Phase.Tone:
                currentPhase = Phase.Article;
                ArticleSelector(option);
                break;

            case Phase.Article:
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

        return dictOk && topicOk;
    }

    private string[] BuildHeadlineLabels()
    {
        string[] labels = new string[4];
        for (int i = 0; i < 4; i++)
        {
            if (current != null && i < current.Headlines.Length)
                labels[i] = IsHeadlineUnlocked(i) ? current.Headlines[i].text : "Locked";
            else
                labels[i] = "";
        }
        return labels;
    }

    // =====================================================================
    // ARTICLE
    // =====================================================================

    private void ArticleSelector(float option)
    {
        optionOne.gameObject.SetActive(false);
        optionTwo.gameObject.SetActive(false);
        optionThree.gameObject.SetActive(false);
        optionFour.gameObject.SetActive(false);

        string tone = option == 1f ? "Pro Martian" : option == 2f ? "Neutral" : "Pro Human";
        int hIdx = Mathf.RoundToInt(articleChosen) - 1;
        string headline = (current != null && hIdx >= 0 && hIdx < current.Headlines.Length)
            ? current.Headlines[hIdx].text : "Unknown";

        SetDialogueTexts("[ARTICLE: '" + headline + "' \u2014 " + tone + "]\n\nTODO: Article text here.");

        optionOne.gameObject.SetActive(true);
        optionOne.text = "Finish";
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
                endOfDaySummaryText.text = current.GetEndOfDaySummary();
        }

        optionOne.gameObject.SetActive(false);
        optionTwo.gameObject.SetActive(false);
        optionThree.gameObject.SetActive(false);
        optionFour.gameObject.SetActive(false);
    }

    /// <summary>
    /// Call from the end-of-day panel's "Continue" button.
    /// Returns to briefing/character select for the next interview or day.
    /// </summary>
    public void EndOfDayContinue()
    {
        if (endOfDayPanel != null) endOfDayPanel.SetActive(false);
        if (briefingPanel != null) briefingPanel.SetActive(true);
        currentPhase = Phase.Dialogue;
    }
}
