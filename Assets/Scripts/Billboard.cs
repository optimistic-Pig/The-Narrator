using UnityEngine;

/// <summary>
/// Attach to any GameObject holding a 2D art asset (Quad + material) in the
/// 3D office.  The object will always face the player camera as they walk
/// around, so flat PNG art reads clearly from any angle.
///
/// SETUP:
///   1. GameObject → 3D Object → Quad
///   2. Create a Material: Shader = Universal Render Pipeline/Lit (or
///      Standard), Surface Type = Transparent, set your PNG as Base Map.
///      For cut-out edges (transparent background) use Alpha Clipping.
///   3. Drag the material onto the Quad.
///   4. Attach this script to the Quad.
///   5. Scale the Quad to match your PNG's aspect ratio.
///      Example: 1024×768 PNG → Scale X=1.33, Y=1, Z=1
///
/// The object rotates in LateUpdate (after the camera moves) so there is
/// never a one-frame lag between the camera and the facing direction.
/// </summary>
public class Billboard : MonoBehaviour
{
    [Tooltip("Leave empty to auto-use Camera.main.")]
    public Camera targetCamera;

    [Tooltip("Lock vertical axis so the art stays upright even if the " +
             "camera tilts (recommended for wall art / newspaper clippings).")]
    public bool lockVerticalAxis = true;

    void LateUpdate()
    {
        Camera cam = targetCamera != null ? targetCamera : Camera.main;
        if (cam == null) return;

        if (lockVerticalAxis)
        {
            // Only rotate around Y — keeps the art perfectly upright.
            Vector3 dir = transform.position - cam.transform.position;
            dir.y = 0f;
            if (dir.sqrMagnitude > 0.001f)
                transform.rotation = Quaternion.LookRotation(dir);
        }
        else
        {
            // Full billboard — faces camera in all axes.
            transform.LookAt(cam.transform.position);
            transform.Rotate(0f, 180f, 0f); // flip so front faces camera
        }
    }
}
