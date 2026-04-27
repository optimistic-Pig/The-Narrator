using UnityEngine;

/// <summary>
/// Moves each NPC GameObject to its assigned spawn slot at the start of each
/// day, and hides NPCs that are not scheduled for the current day.
///
/// SETUP:
///   1. Create 4 empty GameObjects in the scene at your desired NPC positions
///      (e.g. SlotA front-left, SlotB front-right, SlotC doorway, SlotD side).
///   2. Add this component to the Managers GameObject.
///   3. Assign the 4 slot Transforms and the 4 NPC Transforms in the Inspector.
///   4. GameStateManager will call RefreshNPCPositions() after OnArticlePublished.
///
/// SLOT LAYOUT (suggested):
///   Slot 0 — front-left  cubicle   (always visible)
///   Slot 1 — front-right cubicle   (always visible)
///   Slot 2 — doorway                (visible from front)
///   Slot 3 — side alcove            (visible by walking around)
/// </summary>
public class NPCSpawnManager : MonoBehaviour
{
    // =====================================================================
    // SINGLETON
    // =====================================================================

    public static NPCSpawnManager Instance { get; private set; }

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(this); return; }
        Instance = this;
    }

    // =====================================================================
    // INSPECTOR REFERENCES
    // =====================================================================

    [Header("Spawn Slot Transforms (empty GameObjects)")]
    public Transform slot0;   // front-left
    public Transform slot1;   // front-right
    public Transform slot2;   // doorway
    public Transform slot3;   // side alcove

    [Header("NPC GameObjects to move")]
    public GameObject izulNPC;
    public GameObject kortnaraNPC;
    public GameObject gorpNPC;
    public GameObject andrewNPC;

    // =====================================================================
    // UNITY LIFECYCLE
    // =====================================================================

    void Start()
    {
        RefreshNPCPositions();

        // Subscribe to state changes so positions update automatically
        // when the day advances after an article is published.
        if (GameStateManager.Instance != null)
            GameStateManager.Instance.OnStateChanged += RefreshNPCPositions;
    }

    void OnDestroy()
    {
        if (GameStateManager.Instance != null)
            GameStateManager.Instance.OnStateChanged -= RefreshNPCPositions;
    }

    // =====================================================================
    // PUBLIC API
    // =====================================================================

    /// <summary>
    /// Reads the current day from GameStateManager and repositions / hides
    /// NPCs accordingly. Safe to call at any time.
    /// </summary>
    public void RefreshNPCPositions()
    {
        if (GameStateManager.Instance == null) return;

        int day = GameStateManager.Instance.CurrentDay;

        // Hide every NPC first, then show and place the ones scheduled today.
        SetActive(izulNPC,    false);
        SetActive(kortnaraNPC, false);
        SetActive(gorpNPC,    false);
        SetActive(andrewNPC,  false);

        switch (day)
        {
            // ── Day 1: Izul (slot 0) + Kortnara (slot 1) ─────────────────
            case 1:
                PlaceNPC(izulNPC,     slot0);
                PlaceNPC(kortnaraNPC, slot1);
                break;

            // ── Day 2: uninterviewed {Izul|Kortnara} + Gorp ───────────────
            // Gorp always takes slot 2 (doorway).
            // Whichever of Izul/Kortnara was skipped fills slot 0.
            case 2:
                bool izulDone     = GameStateManager.Instance
                                        .IsAlreadyInterviewed(GameStateManager.Instance.izulNPC);
                bool kortnaraDone = GameStateManager.Instance
                                        .IsAlreadyInterviewed(GameStateManager.Instance.kortnaraNPC);

                if (!izulDone)     PlaceNPC(izulNPC,     slot0);
                if (!kortnaraDone) PlaceNPC(kortnaraNPC, slot0);

                PlaceNPC(gorpNPC, slot2);
                break;

            // ── Day 3: Andrew (slot 3) + any remaining NPCs ───────────────
            case 3:
                PlaceNPC(andrewNPC, slot3);

                int nextSlot = 0;
                if (!GameStateManager.Instance.IsAlreadyInterviewed(GameStateManager.Instance.izulNPC))
                    PlaceNPC(izulNPC,     SlotByIndex(nextSlot++));
                if (!GameStateManager.Instance.IsAlreadyInterviewed(GameStateManager.Instance.kortnaraNPC))
                    PlaceNPC(kortnaraNPC, SlotByIndex(nextSlot++));
                if (!GameStateManager.Instance.IsAlreadyInterviewed(GameStateManager.Instance.gorpNPC))
                    PlaceNPC(gorpNPC,     SlotByIndex(nextSlot++));
                break;
        }
    }

    // =====================================================================
    // HELPERS
    // =====================================================================

    private void PlaceNPC(GameObject npc, Transform slot)
    {
        if (npc == null || slot == null) return;
        npc.transform.position = slot.position;
        npc.transform.rotation = slot.rotation;
        npc.SetActive(true);
    }

    private void SetActive(GameObject npc, bool active)
    {
        if (npc != null) npc.SetActive(active);
    }

    /// <summary>Returns slot 0–2 by index (slot 3 is reserved for Andrew).</summary>
    private Transform SlotByIndex(int i)
    {
        switch (i)
        {
            case 0: return slot0;
            case 1: return slot1;
            case 2: return slot2;
            default: return slot0;
        }
    }
}
