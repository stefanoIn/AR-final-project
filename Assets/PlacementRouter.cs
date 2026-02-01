using UnityEngine;
using Vuforia;

public class PlacementRouter : MonoBehaviour
{
    [Header("Vuforia References")]
    public PlaneFinderBehaviour planeFinder;
    public ContentPositioningBehaviour contentPositioning;

    private PaintingTarget currentTarget;

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

    // CALLED BY: Plane Finder → AnchorInputListenerBehaviour → On Input Received (Vector2)
    public void LogTap(Vector2 screenPos)
    {
        Debug.Log($"[AR-MUSEUM][INPUT] TAP detected at {screenPos}");

        if (planeFinder == null)
        {
            Debug.LogError("[AR-MUSEUM][INPUT] PlaneFinder is NULL");
            return;
        }

        Debug.Log("[AR-MUSEUM][INPUT] PlaneFinder ACTIVE = "
                  + planeFinder.gameObject.activeInHierarchy);
    }

    // =============================
    // HIT TEST RESULT
    // =============================

    // CALLED BY: Plane Finder → PlaneFinderBehaviour → Interactive Hit Test (HitTestResult)
   public void OnInteractiveHitTest(HitTestResult result)
    {
        Debug.Log("[AR-MUSEUM][HIT] Interactive HitTest RESULT received");

        if (result == null)
        {
            Debug.LogError("[AR-MUSEUM][HIT] HitTestResult is NULL");
            return;
        }

        if (currentTarget == null)
        {
            Debug.LogError("[AR-MUSEUM][HIT] No currentTarget set");
            return;
        }

        // prevent double placement
        if (currentTarget.IsPlaced)
        {
            Debug.Log("[AR-MUSEUM][HIT] Target already placed — ignoring tap");
            return;
        }

        Debug.Log("[AR-MUSEUM][HIT] Placing content manually");

        // Create anchor ONCE per painting
        GameObject anchor = new GameObject("ManualAnchor_" + currentTarget.targetKey);

        anchor.transform.position = result.Position;
        anchor.transform.rotation = result.Rotation;

        // Optional but recommended: keep hierarchy clean
        // anchor.transform.SetParent(groundPlaneStage.transform, true);

        currentTarget.PlaceAt(anchor.transform);

        // Disable Plane Finder until another painting is detected
        planeFinder.gameObject.SetActive(false);
    }


    // =============================
    // TARGET ROUTING
    // =============================

    // CALLED BY: ImageTarget → PaintingTarget.OnTargetFound()
    public void SetCurrentTarget(PaintingTarget target)
    {
        Debug.Log("[AR-MUSEUM][Router] SetCurrentTarget CALLED");

        if (target == null)
        {
            Debug.LogError("[AR-MUSEUM][Router] SetCurrentTarget called with NULL");
            return;
        }

        currentTarget = target;
        Debug.Log($"[AR-MUSEUM][Router] Current target = {target.targetKey}");

        if (!currentTarget.IsPlaced)
        {
            if (planeFinder == null)
            {
                Debug.LogError("[AR-MUSEUM][Router] PlaneFinder reference is NULL");
            }
            else
            {
                planeFinder.gameObject.SetActive(true);
                Debug.Log("[AR-MUSEUM][Router] Plane Finder ENABLED");
            }
        }
        else
        {
            Debug.Log("[AR-MUSEUM][Router] Target already placed, no Plane Finder needed");
        }
    }

    // =============================
    // CONTENT PLACEMENT
    // =============================

    // CALLED BY: Plane Finder → ContentPositioningBehaviour → On Content Placed (GameObject)
    public void OnContentPlaced(GameObject anchorStage)
    {
        Debug.Log("[AR-MUSEUM][CONTENT] OnContentPlaced CALLED");

        if (anchorStage == null)
        {
            Debug.LogError("[AR-MUSEUM][CONTENT] anchorStage is NULL");
            return;
        }

        Debug.Log($"[AR-MUSEUM][CONTENT] AnchorStage name = {anchorStage.name}");
        Debug.Log($"[AR-MUSEUM][CONTENT] AnchorStage position = {anchorStage.transform.position}");

        if (currentTarget == null)
        {
            Debug.LogError("[AR-MUSEUM][CONTENT] currentTarget is NULL");
            return;
        }

        Debug.Log($"[AR-MUSEUM][CONTENT] Routing to {currentTarget.targetKey}");

        anchorStage.SetActive(true);
        currentTarget.PlaceAt(anchorStage.transform);
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
        }
    }
}
