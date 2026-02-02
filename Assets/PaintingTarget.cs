using UnityEngine;

public class PaintingTarget : MonoBehaviour
{
    [Header("Painting Setup")]
    public string targetKey;
    public GameObject characterPrefab;

    [Header("References")]
    public PlacementRouter router;

    private GameObject spawnedInstance;
    private bool isPlaced = false;

    public bool IsPlaced => isPlaced;

    // =============================
    // LIFECYCLE
    // =============================

    private void Awake()
    {
        Debug.Log($"[AR-MUSEUM][Painting:{targetKey}] Awake");

        if (string.IsNullOrEmpty(targetKey))
            Debug.LogError("[AR-MUSEUM][Painting] targetKey is EMPTY");

        if (characterPrefab == null)
            Debug.LogError($"[AR-MUSEUM][Painting:{targetKey}] characterPrefab is NULL");

        if (router == null)
            Debug.LogError($"[AR-MUSEUM][Painting:{targetKey}] router reference is NULL");
    }

    // =============================
    // IMAGE TARGET EVENTS
    // =============================

    public void OnTargetFound()
    {
        Debug.Log($"[AR-MUSEUM][Painting:{targetKey}] OnTargetFound");

        router.SetCurrentTarget(this);

        if (isPlaced && spawnedInstance != null)
        {
            Debug.Log($"[AR-MUSEUM][Painting:{targetKey}] Re-activating existing character");
            spawnedInstance.SetActive(true);
        }
    }

    public void OnTargetLost()
    {
        Debug.Log($"[AR-MUSEUM][Painting:{targetKey}] OnTargetLost");

        if (isPlaced && spawnedInstance != null)
            spawnedInstance.SetActive(false);

        router.ClearCurrentTarget(this);
    }

    // =============================
    // PLACEMENT
    // =============================

    /// Called ONLY by PlacementRouter after plane placement
    public void PlaceAt(Transform anchor)
    {
        Debug.Log($"[AR-MUSEUM][Painting:{targetKey}] PlaceAt CALLED");

        if (anchor == null || isPlaced)
            return;

        // Instantiate
        spawnedInstance = Instantiate(characterPrefab);
        spawnedInstance.name = characterPrefab.name + "_Instance";

        // Parent to anchor
        spawnedInstance.transform.SetParent(anchor, false);

        // Reset local transform
        spawnedInstance.transform.localPosition = Vector3.zero;
        spawnedInstance.transform.localRotation = Quaternion.identity;
        spawnedInstance.transform.localScale = Vector3.one;

        // =============================
        // 🔴 CORE FIX: CAMERA-RELATIVE → FLOOR-RELATIVE
        // =============================

        float anchorY = anchor.position.y;

        // Compensate for camera-relative ground plane
        spawnedInstance.transform.position += Vector3.up * anchorY;

        // =============================
        // DEBUG LOGS (PLACEMENT)
        // =============================

        Debug.Log($"[AR-MUSEUM][Placement] Anchor world position: {anchor.position}");
        Debug.Log($"[AR-MUSEUM][Placement] Character world position: {spawnedInstance.transform.position}");

        // =============================
        // OPTIONAL: Speech / Bubble debug
        // =============================

        Transform bubble = spawnedInstance.transform.Find("SpeechBubble");
        if (bubble != null)
        {
            Debug.Log($"[AR-MUSEUM][Placement] Bubble world position: {bubble.position}");
            Debug.Log($"[AR-MUSEUM][Placement] Bubble local position: {bubble.localPosition}");
        }
        else
        {
            Debug.LogWarning($"[AR-MUSEUM][Placement] No SpeechBubble found on {spawnedInstance.name}");
        }

        // =============================
        // Face camera (Y only)
        // =============================

        Camera cam = Camera.main;
        if (cam != null)
        {
            Vector3 dir = cam.transform.position - spawnedInstance.transform.position;
            dir.y = 0f;

            if (dir.sqrMagnitude > 0.001f)
                spawnedInstance.transform.rotation = Quaternion.LookRotation(dir);
        }

        isPlaced = true;

        Debug.Log($"[AR-MUSEUM][Painting:{targetKey}] Placement COMPLETE");
    }
}
