using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Singleton UI manager for displaying and reading documents.
/// SETUP:
///   1. Create a Canvas panel named "DocumentPanel" with:
///       - DocumentListScroll (ScrollRect) containing DocumentListContent (VerticalLayoutGroup)
///       - DocumentViewPanel with DocumentTitle and DocumentBody (TextMeshProUGUI)
///       - CloseBtn (Button)
///   2. Assign this component to the DocumentPanel.
///   3. Drag references into the Inspector.
/// </summary>
public class DocumentManager : MonoBehaviour
{
    public static DocumentManager Instance { get; private set; }

    [Header("Panel References")]
    public GameObject documentPanel;
    public GameObject documentListScroll;
    public Transform documentListContent;
    public GameObject documentViewPanel;
    public TextMeshProUGUI documentTitle;
    public TextMeshProUGUI documentBody;
    public TextMeshProUGUI emptyStateText;
    public Button closeBtn;

    [Header("Document List")]
    [Tooltip("All available documents in the game")]
    public DocumentDefinition[] allDocuments;

    [Header("Prefab")]
    [Tooltip("Button prefab for each document in the list")]
    public Button documentListItemPrefab;

    private List<Button> documentButtons = new List<Button>();

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    void Start()
    {
        if (documentPanel != null) documentPanel.SetActive(false);
        if (documentViewPanel != null) documentViewPanel.SetActive(false);
        if (closeBtn != null) closeBtn.onClick.AddListener(CloseDocuments);

        PopulateDocumentList();
    }

    private void PopulateDocumentList()
    {
        if (documentListContent == null || documentListItemPrefab == null) return;

        // Clear existing buttons
        foreach (var btn in documentButtons)
            Destroy(btn.gameObject);
        documentButtons.Clear();

        // Create a button for each unlocked document
        foreach (var doc in allDocuments)
        {
            if (doc == null) continue;

            var btnGO = Instantiate(documentListItemPrefab.gameObject, documentListContent);
            var btn = btnGO.GetComponent<Button>();
            var txt = btnGO.GetComponentInChildren<TextMeshProUGUI>();

            if (txt != null) txt.text = doc.title;
            if (btn != null)
            {
                btn.onClick.AddListener(() => ShowDocument(doc));
                documentButtons.Add(btn);
            }
        }
    }

    public void OpenDocuments()
    {
        // Lock player movement and UI
        var playerController = FindFirstObjectByType<PlayerMovement>();
        if (playerController != null)
            playerController.LockPlayer();

        if (documentPanel != null) documentPanel.SetActive(true);
        if (documentViewPanel != null) documentViewPanel.SetActive(false);
        RefreshDocumentList();
    }

    private void RefreshDocumentList()
    {
        int unlockedCount = 0;

        for (int i = 0; i < documentButtons.Count && i < allDocuments.Length; i++)
        {
            bool isUnlocked = allDocuments[i] != null && allDocuments[i].IsUnlocked();
            documentButtons[i].gameObject.SetActive(isUnlocked);
            if (isUnlocked) unlockedCount++;
        }

        // Show/hide empty state
        if (emptyStateText != null)
            emptyStateText.gameObject.SetActive(unlockedCount == 0);
    }

    private void ShowDocument(DocumentDefinition doc)
    {
        if (doc == null || !doc.IsUnlocked()) return;

        if (documentViewPanel != null) documentViewPanel.SetActive(true);
        if (documentTitle != null) documentTitle.text = doc.title;
        if (documentBody != null) documentBody.text = doc.body;
    }

    public void CloseDocuments()
    {
        if (documentPanel != null) documentPanel.SetActive(false);
        if (documentViewPanel != null) documentViewPanel.SetActive(false);

        // Unlock player movement
        var playerController = FindFirstObjectByType<PlayerMovement>();
        if (playerController != null)
            playerController.UnlockPlayer();
    }
}
