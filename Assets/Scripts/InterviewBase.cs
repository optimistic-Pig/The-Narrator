using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Base class for every interviewee. Each character provides their own
/// dialogue tree, dictionary words, topic keywords, and headline options.
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
        public string klingonWord;              // "QIH"
        public string translation;              // "destruction"
        public string[] altSpellings;           // e.g. unicode apostrophe variants
        [HideInInspector] public bool translated;
        [HideInInspector] public bool seen;
    }

    [System.Serializable]
    public class Topic
    {
        public string name;                     // "destruction", "war", etc.
        public string[] keywords;               // words that trigger this topic
        [HideInInspector] public bool encountered;
    }

    [System.Serializable]
    public class Headline
    {
        public string text;                     // "Martians face Destruction"
        public int requiredDictIndex;           // index into DictionaryEntries (-1 = none)
        public int requiredTopicIndex;          // index into Topics (-1 = none)
        public bool alwaysAvailable;            // true for fallback headlines like "Mars Updates"
    }

    // =====================================================================
    // ABSTRACT PROPERTIES — override in each character
    // =====================================================================

    public abstract string CharacterName { get; }       // "IZUL: Geographer, The Basin"
    public abstract DictEntry[] DictionaryEntries { get; }
    public abstract Topic[] Topics { get; }
    public abstract Headline[] Headlines { get; }
    public abstract HashSet<float> LastQuestionNodes { get; }
    public abstract int StartingLookups { get; }        // how many dictionary lookups

    // =====================================================================
    // SHARED STATE — lives here so it resets per character
    // =====================================================================

    [HideInInspector] public float dialogueIndexTracker = 0f;

    // =====================================================================
    // ABSTRACT METHODS
    // =====================================================================

    /// <summary>
    /// The character's full dialogue tree. Called by DialogueManager on each
    /// option click during the Dialogue phase.
    /// Use dm.SetDialogueTexts(...) to set text,
    /// and dm.optionX.gameObject.SetActive(...) to show/hide buttons.
    /// </summary>
    public abstract void DialogueSetter(float option, DialogueManager dm);

    // =====================================================================
    // VIRTUAL METHODS — override for character-specific behavior
    // =====================================================================

    /// <summary>
    /// Resets all interview state for a fresh playthrough.
    /// Override and call base.ResetState() if the character has extra state.
    /// </summary>
    public virtual void ResetState()
    {
        dialogueIndexTracker = 0f;
        if (DictionaryEntries != null)
            foreach (var e in DictionaryEntries) { e.translated = false; e.seen = false; }
        if (Topics != null)
            foreach (var t in Topics) { t.encountered = false; }
    }

    /// <summary>
    /// Returns a summary string shown on the end-of-day popup.
    /// Override to customize what the player sees after the interview.
    /// </summary>
    public virtual string GetEndOfDaySummary()
    {
        return "Interview with " + CharacterName + " complete.";
    }
}
