using UnityEngine;

/// <summary>
/// ScriptableObject representing a single document that the player can read.
/// Create instances via Assets > Create > Document.
/// </summary>
public class DocumentDefinition : ScriptableObject
{
    [Header("Identity")]
    [Tooltip("Unique identifier for this document (e.g., 'doc_ves_report')")]
    public string documentID = "";

    [Tooltip("Display name shown in the document list")]
    public string title = "Untitled";

    [Header("Content")]
    [TextArea(5, 10)]
    [Tooltip("Full body text of the document")]
    public string body = "";

    [Header("Unlock Condition")]
    [Tooltip("Which dialogue node or event unlocks this document")]
    public string unlockedBy = "";
    // Examples: "IzulNode9", "GorpInterview", "GameStart", etc.

    /// <summary>
    /// Checks if this document is unlocked based on the current game state.
    /// Override or extend as needed for complex unlock logic.
    /// </summary>
    public bool IsUnlocked()
    {
        if (string.IsNullOrEmpty(unlockedBy) || unlockedBy == "GameStart")
            return true;
        // Add more unlock logic here as needed
        return GameStateManager.Instance != null && 
               GameStateManager.Instance.IsDocumentUnlocked(documentID);
    }
}
