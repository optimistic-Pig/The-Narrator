using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Base class for every interviewee. Each character provides their own
/// dialogue tree, dictionary words, topic keywords, headline options,
/// and article paragraph templates.
/// The DialogueManager handles all shared UI and phase logic.
/// </summary>
public abstract class InterviewBase : MonoBehaviour
{
    // =====================================================================
    // DATA TYPES
    // =====================================================================

    [System.Serializable]
    public class DictEntry
    {
        public string klingonWord;
        public string translation;
        public string[] altSpellings;
        [HideInInspector] public bool translated;
        [HideInInspector] public bool seen;
    }

    [System.Serializable]
    public class Topic
    {
        public string name;
        public string[] keywords;
        [HideInInspector] public bool encountered;
    }

    [System.Serializable]
    public class Headline
    {
        public string text;
        public int requiredDictIndex;        // index into DictionaryEntries (-1 = none)
        public int requiredTopicIndex;       // index into Topics (-1 = none)
        public bool alwaysAvailable;
    }

    // =====================================================================
    // ARTICLE WRITING DATA TYPES
    // =====================================================================

    [System.Serializable]
    public class ParagraphChoice
    {
        [TextArea(2, 5)]
        public string text;
        public int scoreEffect;             // +1 pro-Martian, -1 pro-Human, 0 pro-Self
    }

    [System.Serializable]
    public class ArticleParagraph
    {
        [TextArea(1, 3)]
        public string promptText;
        public ParagraphChoice truthful;    // Option 1
        public ParagraphChoice dishonest;   // Option 2
        public ParagraphChoice ambitious;   // Option 3 (self-serving)
    }

    [System.Serializable]
    public class ArticleTemplate
    {
        public int headlineIndex;
        public ArticleParagraph[] paragraphs;
    }

    // =====================================================================
    // ABSTRACT PROPERTIES
    // =====================================================================

    public abstract string CharacterName { get; }
    public abstract DictEntry[] DictionaryEntries { get; }
    public abstract Topic[] Topics { get; }
    public abstract Headline[] Headlines { get; }
    public abstract ArticleTemplate[] ArticleTemplates { get; }
    public abstract HashSet<float> LastQuestionNodes { get; }
    public abstract int StartingLookups { get; }

    // =====================================================================
    // SHARED STATE
    // =====================================================================

    [HideInInspector] public float dialogueIndexTracker = 0f;

    // =====================================================================
    // ABSTRACT METHODS
    // =====================================================================

    public abstract void DialogueSetter(float option, DialogueManager dm);

    // =====================================================================
    // VIRTUAL METHODS
    // =====================================================================

    public virtual void ResetState()
    {
        dialogueIndexTracker = 0f;
        if (DictionaryEntries != null)
            foreach (var e in DictionaryEntries) { e.translated = false; e.seen = false; }
        if (Topics != null)
            foreach (var t in Topics) { t.encountered = false; }
    }

    public virtual string GetEndOfDaySummary()
    {
        return "Interview with " + CharacterName + " complete.";
    }

    /// <summary>
    /// Override to gate specific headlines behind character-specific flags
    /// (e.g. unlockTranscript, unlockOldPaper) that aren't in the dict/topic system.
    /// Called by DialogueManager.IsHeadlineUnlocked() after the dict/topic checks pass.
    /// Return false to lock a headline even if dict/topic requirements are met.
    /// </summary>
    public virtual bool IsAdditionalHeadlineConditionMet(int headlineIndex) => true;
}
