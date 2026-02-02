using UnityEngine;
using Vuforia;

public class PlacementRouter : MonoBehaviour
{
    [Header("Vuforia References")]
    public PlaneFinderBehaviour planeFinder;
    public ContentPositioningBehaviour contentPositioning;

    private PaintingTarget currentTarget;
    private bool placementRequested = false;

    private void Awake()
    {
        Debug.Log("[AR-MUSEUM][Router] Awake");
    }

    // =============================
    // INPUT
    // =============================

    public void LogTap(Vector2 screenPos)
    {
        Debug.Log($"[AR-MUSEUM][INPUT] Tap detected at {screenPos}");

        if (currentTarget == null || currentTarget.IsPlaced)
            return;

        placementRequested = true;
    }

    // =============================
    // HIT TEST
    // =============================

    public void OnInteractiveHitTest(HitTestResult result)
    {
        if (!placementRequested || result == null || currentTarget == null)
            return;

        placementRequested = false;

        Debug.Log("[AR-MUSEUM][HIT] Positioning content");
        contentPositioning.PositionContentAtPlaneAnchor(result);
    }

    // =============================
    // CONTENT PLACED
    // =============================

    public void OnContentPlaced(GameObject anchorStage)
    {
        if (anchorStage == null || currentTarget == null)
            return;

        Debug.Log($"[AR-MUSEUM][CONTENT] Anchor at {anchorStage.transform.position}");

        currentTarget.PlaceAt(anchorStage.transform);

        // Disable PlaneFinder after placement
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
            Debug.Log("[AR-MUSEUM][Router] PlaneFinder ENABLED");
        }
    }

    public void ClearCurrentTarget(PaintingTarget target)
    {
        if (currentTarget == target)
        {
            currentTarget = null;
            placementRequested = false;
        }
    }
}
