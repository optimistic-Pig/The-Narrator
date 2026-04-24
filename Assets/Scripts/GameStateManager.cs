using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Singleton that tracks which NPCs are available each day, which have been
/// interviewed, secret path flags, and computes the final ending.
///
/// SETUP: Add this component to a persistent GameObject in your scene.
/// Drag each NPC's InterviewBase component into the four NPC slots in the Inspector.
/// </summary>
public class GameStateManager : MonoBehaviour
{
    // =====================================================================
    // SINGLETON
    // =====================================================================

    public static GameStateManager Instance { get; private set; }

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    // =====================================================================
    // CHARACTER IDs
    // =====================================================================

    public enum CharacterID { None, Izul, Kortnara, Gorp, Andrew }

    // =====================================================================
    // NPC REFERENCES  (assign in Inspector)
    // =====================================================================

    [Header("NPC InterviewBase References")]
    public InterviewBase izulNPC;
    public InterviewBase kortnaraNPC;
    public InterviewBase gorpNPC;
    public InterviewBase andrewNPC;

    // =====================================================================
    // STATE
    // =====================================================================

    public int  CurrentDay         { get; private set; } = 1;
    public bool InterviewInProgress { get; private set; } = false;

    /// <summary>
    /// True after an interview is finished but before the player has
    /// visited the desk and published an article.
    /// While true, both NPC clicks and further desk visits are blocked.
    /// </summary>
    public bool WaitingForDesk { get; private set; } = false;

    /// <summary>
    /// True when the player backed out of a dialogue mid-interview.
    /// The NPC is suspended — clicking the SAME NPC will resume; all
    /// other NPCs and the desk remain locked until the interview finishes.
    /// </summary>
    public bool InterviewPaused { get; private set; } = false;

    // ─── Secret-path flags ───────────────────────────────────────────────
    /// <summary>Set when the player learns Kortnara's promo code "MASK".</summary>
    public bool HasPromoCode       { get; private set; } = false;
    /// <summary>Set when the player reaches Gorp's secret-bunker dialogue node.</summary>
    public bool HasBunkerDialogue  { get; private set; } = false;

    // ─── Internal tracking ───────────────────────────────────────────────
    private readonly HashSet<CharacterID> interviewed  = new HashSet<CharacterID>();
    private InterviewBase                 activeNPC    = null;

    /// <summary>Fired whenever NPC lock states or day changes — PlayerController can subscribe.</summary>
    public System.Action OnStateChanged;

    // =====================================================================
    // PUBLIC API — called by DialogueManager
    // =====================================================================

    /// <summary>
    /// Returns true when this NPC can be clicked to start an interview.
    /// False if: an interview is already in progress, player still needs to
    /// visit the desk, the NPC was already interviewed, or it is not
    /// scheduled for the current day.
    /// </summary>
    public bool IsNPCAvailable(InterviewBase npc)
    {
        if (npc == null)                    return false;
        if (WaitingForDesk)                 return false;

        // Always allow re-clicking the NPC whose interview was paused mid-dialogue
        if (InterviewPaused && npc == activeNPC) return true;

        if (InterviewInProgress)            return false;
        if (InterviewPaused)                return false;   // paused, but wrong NPC

        CharacterID id = GetID(npc);
        if (id == CharacterID.None)         return false;
        if (interviewed.Contains(id))       return false;

        return GetAvailableForDay(CurrentDay).Contains(id);
    }

    /// <summary>Call from DialogueManager.StartInterview().</summary>
    public void OnInterviewStarted(InterviewBase npc)
    {
        activeNPC           = npc;
        InterviewInProgress = true;
        InterviewPaused     = false;
        WaitingForDesk      = false;
        OnStateChanged?.Invoke();
    }

    /// <summary>
    /// Call from DialogueManager.ReturnToOffice() when the player backs out
    /// of a dialogue that is still in progress (Phase.Dialogue).
    /// Suspends the interview so the same NPC can be re-clicked to resume.
    /// </summary>
    public void PauseInterview()
    {
        InterviewInProgress = false;
        InterviewPaused     = true;
        // activeNPC is intentionally kept — it identifies which NPC to resume
        OnStateChanged?.Invoke();
    }

    /// <summary>Returns true if <paramref name="npc"/> is the one currently paused mid-interview.</summary>
    public bool IsInterviewPaused(InterviewBase npc) => InterviewPaused && npc == activeNPC;

    /// <summary>Returns true if this NPC has already been fully interviewed this game.</summary>
    public bool IsAlreadyInterviewed(InterviewBase npc)
    {
        CharacterID id = GetID(npc);
        return id != CharacterID.None && interviewed.Contains(id);
    }

    /// <summary>
    /// Call when the dialogue phase ends and the player should now go to
    /// the desk (i.e. when "Continue" is clicked after the last dialogue
    /// node, before the headline screen).
    /// </summary>
    public void OnInterviewComplete()
    {
        InterviewInProgress = false;
        WaitingForDesk      = true;
        OnStateChanged?.Invoke();
    }

    /// <summary>
    /// Call when the player clicks "Publish" on the finished article.
    /// Marks the NPC as interviewed, clears the desk-wait flag, and
    /// advances to the next day.
    /// </summary>
    public void OnArticlePublished()
    {
        if (activeNPC != null)
        {
            CharacterID id = GetID(activeNPC);
            if (id != CharacterID.None) interviewed.Add(id);
        }

        activeNPC           = null;
        InterviewInProgress = false;
        InterviewPaused     = false;
        WaitingForDesk      = false;

        if (CurrentDay < 3) CurrentDay++;

        OnStateChanged?.Invoke();
    }

    // ─── Secret-flag setters ─────────────────────────────────────────────

    /// <summary>Call from KortnaraInterview when the MASK promo-code node fires.</summary>
    public void SetPromoCodeFound()     { HasPromoCode      = true; }

    /// <summary>Call from GorpInterview when the secret-bunker node fires.</summary>
    public void SetBunkerDialogueFound() { HasBunkerDialogue = true; }

    // =====================================================================
    // ENDING LOGIC
    // =====================================================================

    public enum EndingType { ProMartian, ProHuman, ProSelf, Secret }

    /// <summary>
    /// Pass the final cumulative marsOpinionScore from DialogueManager.
    /// Secret ending overrides score-based endings.
    ///
    /// Score thresholds (6 paragraph choices total, range –6 to +6):
    ///   > 2  → Pro-Martian
    ///   < –2 → Pro-Human
    ///   else → Pro-Self
    /// </summary>
    public EndingType GetEnding(int score)
    {
        if (HasPromoCode && HasBunkerDialogue) return EndingType.Secret;
        if (score >  2) return EndingType.ProMartian;
        if (score < -2) return EndingType.ProHuman;
        return EndingType.ProSelf;
    }

    // =====================================================================
    // HELPERS
    // =====================================================================

    /// <summary>Returns the InterviewBase that is currently mid-interview (or post-interview).</summary>
    public InterviewBase GetActiveNPC() => activeNPC;

    public CharacterID GetID(InterviewBase npc)
    {
        if (npc == null)          return CharacterID.None;
        if (npc == izulNPC)       return CharacterID.Izul;
        if (npc == kortnaraNPC)   return CharacterID.Kortnara;
        if (npc == gorpNPC)       return CharacterID.Gorp;
        if (npc == andrewNPC)     return CharacterID.Andrew;
        return CharacterID.None;
    }

    // ─── Day availability schedule ───────────────────────────────────────

    /// <summary>
    /// Day 1: Izul OR Kortnara (player picks one)
    /// Day 2: whichever of {Izul, Kortnara} was NOT picked on Day 1, plus Gorp
    /// Day 3: Andrew, plus whichever of the remaining pool was not yet interviewed
    /// </summary>
    private List<CharacterID> GetAvailableForDay(int day)
    {
        var list = new List<CharacterID>();
        switch (day)
        {
            case 1:
                list.Add(CharacterID.Izul);
                list.Add(CharacterID.Kortnara);
                break;

            case 2:
                // One of {Izul, Kortnara} was skipped on Day 1
                if (!interviewed.Contains(CharacterID.Izul))     list.Add(CharacterID.Izul);
                if (!interviewed.Contains(CharacterID.Kortnara)) list.Add(CharacterID.Kortnara);
                list.Add(CharacterID.Gorp);
                break;

            case 3:
                list.Add(CharacterID.Andrew);
                // Whoever was skipped on Day 2
                if (!interviewed.Contains(CharacterID.Izul))     list.Add(CharacterID.Izul);
                if (!interviewed.Contains(CharacterID.Kortnara)) list.Add(CharacterID.Kortnara);
                if (!interviewed.Contains(CharacterID.Gorp))     list.Add(CharacterID.Gorp);
                break;
        }
        return list;
    }
}
