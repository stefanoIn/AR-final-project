using UnityEngine;
using Vuforia;

public class PlacementRouter : MonoBehaviour
{
    [Header("Vuforia References")]
    public PlaneFinderBehaviour planeFinder;
    public ContentPositioningBehaviour contentPositioning;

    private PaintingTarget currentTarget;
    private bool placementRequested = false;

    // =============================
    // LIFECYCLE
    // =============================

    private void Awake()
    {
        Debug.Log("[AR-MUSEUM][Router] Awake");

        if (planeFinder == null)
            Debug.LogError("[AR-MUSEUM][Router] PlaneFinder reference NOT assigned");

        if (contentPositioning == null)
            Debug.LogError("[AR-MUSEUM][Router] ContentPositioning reference NOT assigned");
    }

    // =============================
    // INPUT
    // =============================

    // CALLED BY: AnchorInputListenerBehaviour → On Input Received (Vector2)
    public void LogTap(Vector2 screenPos)
    {
        Debug.Log($"[AR-MUSEUM][INPUT] Tap detected at {screenPos}");

        if (currentTarget == null)
        {
            Debug.Log("[AR-MUSEUM][INPUT] No active painting — ignoring tap");
            return;
        }

        if (currentTarget.IsPlaced)
        {
            Debug.Log("[AR-MUSEUM][INPUT] Target already placed — ignoring tap");
            return;
        }

        placementRequested = true;
    }

    // =============================
    // HIT TEST RESULT
    // =============================

    // CALLED BY: PlaneFinderBehaviour → Interactive Hit Test (HitTestResult)
    public void OnInteractiveHitTest(HitTestResult result)
    {
        Debug.Log("[AR-MUSEUM][HIT] Interactive HitTest received");

        if (!placementRequested)
        {
            Debug.Log("[AR-MUSEUM][HIT] No placement requested — ignoring hit");
            return;
        }

        placementRequested = false;

        if (result == null)
        {
            Debug.LogError("[AR-MUSEUM][HIT] HitTestResult is NULL");
            return;
        }

        if (currentTarget == null)
        {
            Debug.LogError("[AR-MUSEUM][HIT] currentTarget is NULL");
            return;
        }

        if (currentTarget.IsPlaced)
        {
            Debug.Log("[AR-MUSEUM][HIT] Target already placed — aborting");
            return;
        }

        Debug.Log("[AR-MUSEUM][HIT] Creating Ground Plane anchor via ContentPositioningBehaviour");

        // THIS creates the REAL Vuforia Ground Plane anchor
       contentPositioning.PositionContentAtPlaneAnchor(result);

    }

    // =============================
    // CONTENT PLACEMENT
    // =============================

    // CALLED BY: ContentPositioningBehaviour → On Content Placed (GameObject)
    public void OnContentPlaced(GameObject anchorStage)
    {
        Debug.Log("[AR-MUSEUM][CONTENT] OnContentPlaced");

        if (anchorStage == null)
        {
            Debug.LogError("[AR-MUSEUM][CONTENT] anchorStage is NULL");
            return;
        }

        if (currentTarget == null)
        {
            Debug.LogError("[AR-MUSEUM][CONTENT] currentTarget is NULL");
            return;
        }

        Debug.Log($"[AR-MUSEUM][CONTENT] Using anchor: {anchorStage.name}");
        Debug.Log($"[AR-MUSEUM][CONTENT] Anchor position: {anchorStage.transform.position}");

        currentTarget.PlaceAt(anchorStage.transform);

        // Stop plane detection after successful placement
        planeFinder.gameObject.SetActive(false);
    }

    // =============================
    // TARGET ROUTING
    // =============================

    // CALLED BY: PaintingTarget.OnTargetFound()
    public void SetCurrentTarget(PaintingTarget target)
    {
        Debug.Log("[AR-MUSEUM][Router] SetCurrentTarget");

        if (target == null)
        {
            Debug.LogError("[AR-MUSEUM][Router] SetCurrentTarget called with NULL");
            return;
        }

        currentTarget = target;
        Debug.Log($"[AR-MUSEUM][Router] Current target = {target.targetKey}");

        if (!currentTarget.IsPlaced)
        {
            planeFinder.gameObject.SetActive(true);
            Debug.Log("[AR-MUSEUM][Router] PlaneFinder ENABLED");
        }
        else
        {
            Debug.Log("[AR-MUSEUM][Router] Target already placed — PlaneFinder not needed");
        }
    }

    // =============================
    // CLEANUP
    // =============================

    // CALLED BY: PaintingTarget.OnTargetLost()
    public void ClearCurrentTarget(PaintingTarget target)
    {
        if (currentTarget == target)
        {
            Debug.Log($"[AR-MUSEUM][Router] Clearing current target: {target.targetKey}");
            currentTarget = null;
            placementRequested = false;
        }
    }
}
