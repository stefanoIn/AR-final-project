using UnityEngine;
using Vuforia;

public class PlacementRouter : MonoBehaviour
{
    [Header("Vuforia References")]
    public PlaneFinderBehaviour planeFinder;
    public ContentPositioningBehaviour contentPositioning;

    [Header("UI")]
    public TMPro.TextMeshProUGUI statusText;

    [Header("Floor Filtering")]
    [Range(0f, 1f)]
    public float floorUpThreshold = 0.85f;

    private PaintingTarget currentTarget;
    private bool placementRequested = false;

    private void Awake()
    {
        Debug.Log("[AR-MUSEUM][Router] Awake");
    }

    // =============================
    // UI
    // =============================

    public void ShowStatus(string message)
    {
        if (statusText == null) return;

        statusText.text = message;
        statusText.gameObject.SetActive(true);
    }

    public void HideStatus()
    {
        if (statusText == null) return;

        statusText.gameObject.SetActive(false);
    }

    // =============================
    // INPUT (tap fallback)
    // =============================

    public void LogTap(Vector2 screenPos)
    {
        if (currentTarget == null || currentTarget.IsPlaced)
            return;

        placementRequested = true;
    }

    // =============================
    // AUTOMATIC PLACEMENT
    // =============================

    public void RequestAutomaticPlacement()
    {
        placementRequested = true;
    }

    // =============================
    // HIT TEST
    // =============================

    public void OnInteractiveHitTest(HitTestResult result)
    {
        if (!placementRequested || result == null || currentTarget == null)
            return;

        Debug.Log("[AR-MUSEUM][HIT] Hit received");
        contentPositioning.PositionContentAtPlaneAnchor(result);
    }

    // =============================
    // CONTENT PLACED
    // =============================

    public void OnContentPlaced(GameObject anchorStage)
    {
        if (anchorStage == null || currentTarget == null)
            return;

        Vector3 planeUp = anchorStage.transform.up;
        float floorDot = Vector3.Dot(planeUp, Vector3.up);

        if (floorDot < floorUpThreshold)
        {
            Debug.Log("[AR-MUSEUM][FILTER] Rejected non-floor plane");
            return; // KEEP TRYING
        }

        placementRequested = false;

        Debug.Log("[AR-MUSEUM][CONTENT] Valid floor anchor");

        currentTarget.PlaceAt(anchorStage.transform);
        planeFinder.gameObject.SetActive(false);
    }

    // =============================
    // TARGET ROUTING
    // =============================

    public void SetCurrentTarget(PaintingTarget target)
    {
        currentTarget = target;

        if (!target.IsPlaced)
        {
            planeFinder.gameObject.SetActive(true);
        }
    }

    public void ClearCurrentTarget(PaintingTarget target)
    {
        if (currentTarget == target)
        {
            currentTarget = null;
            placementRequested = false;

            // Safety: never disable already placed guides
            planeFinder.gameObject.SetActive(false);
        }
    }

}
