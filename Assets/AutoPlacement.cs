using UnityEngine;
using Vuforia;

public class AutoFloorPlacement : MonoBehaviour
{
    public ContentPositioningBehaviour contentPositioning;
    public GameObject characterRoot;

    [Range(0f, 1f)]
    public float minUpwardNormal = 0.85f;

    private bool placed = false;

    // This will be called by AUTOMATIC hit test
    public void OnAutomaticHitTest(HitTestResult result)
    {
        if (placed || result == null)
            return;

        // Compute plane normal
        Vector3 normal = result.Rotation * Vector3.up;

        // Accept ONLY floor-like planes
        if (normal.y >= minUpwardNormal)
        {
            contentPositioning.PositionContentAtPlaneAnchor(result);

            if (characterRoot != null)
                characterRoot.SetActive(true);

            placed = true;

            // Freeze everything
            gameObject.SetActive(false);

            Debug.Log("Automatic floor detected — character placed.");
        }
    }
}
