using UnityEngine;

/// <summary>
/// Interaction component for a filing cabinet in the 3D world.
/// Add this to a cabinet GameObject with a Collider (tagged "FilingCabinet").
/// When clicked, opens the DocumentManager UI.
/// </summary>
public class FilingCabinet : MonoBehaviour
{
    [Tooltip("Optional category or description (for future multi-cabinet support)")]
    public string cabinetName = "Filing Cabinet";

    public void OnClicked()
    {
        if (DocumentManager.Instance != null)
            DocumentManager.Instance.OpenDocuments();
    }
}
