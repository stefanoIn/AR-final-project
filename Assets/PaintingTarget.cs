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

    private void OnEnable()
    {
        Debug.Log($"[AR-MUSEUM][Painting:{targetKey}] ENABLED");
    }

    private void OnDisable()
    {
        Debug.Log($"[AR-MUSEUM][Painting:{targetKey}] DISABLED");
    }

    // =============================
    // IMAGE TARGET EVENTS
    // =============================

    /// Called from DefaultObserverEventHandler → On Target Found
    public void OnTargetFound()
    {
        Debug.Log($"[AR-MUSEUM][Painting:{targetKey}] OnTargetFound");

        if (router == null)
        {
            Debug.LogError($"[AR-MUSEUM][Painting:{targetKey}] Router is NULL");
            return;
        }

        router.SetCurrentTarget(this);

        if (isPlaced && spawnedInstance != null)
        {
            Debug.Log($"[AR-MUSEUM][Painting:{targetKey}] Re-activating existing character");
            spawnedInstance.SetActive(true);
        }
    }

    /// Called from DefaultObserverEventHandler → On Target Lost
    public void OnTargetLost()
    {
        Debug.Log($"[AR-MUSEUM][Painting:{targetKey}] OnTargetLost");

        if (isPlaced && spawnedInstance != null)
        {
            spawnedInstance.SetActive(false);
        }

        if (router != null)
        {
            router.ClearCurrentTarget(this);
        }
    }

    // =============================
    // PLACEMENT
    // =============================

    /// Called ONLY by PlacementRouter after plane placement.
    public void PlaceAt(Transform anchor)
    {
        Debug.Log($"[AR-MUSEUM][Painting:{targetKey}] PlaceAt CALLED");

        if (anchor == null)
        {
            Debug.LogError($"[AR-MUSEUM][Painting:{targetKey}] Anchor is NULL");
            return;
        }

        Debug.Log($"[AR-MUSEUM][Painting:{targetKey}] Anchor name = {anchor.name}");
        Debug.Log($"[AR-MUSEUM][Painting:{targetKey}] Anchor position = {anchor.position}");

        if (isPlaced)
        {
            Debug.Log($"[AR-MUSEUM][Painting:{targetKey}] Already placed — skipping");
            return;
        }

        if (characterPrefab == null)
        {
            Debug.LogError($"[AR-MUSEUM][Painting:{targetKey}] Character Prefab is NULL");
            return;
        }

        Debug.Log($"[AR-MUSEUM][Painting:{targetKey}] Instantiating prefab: {characterPrefab.name}");

        spawnedInstance = Instantiate(characterPrefab);

        if (spawnedInstance == null)
        {
            Debug.LogError($"[AR-MUSEUM][Painting:{targetKey}] Instantiate FAILED");
            return;
        }

        Debug.Log($"[AR-MUSEUM][Painting:{targetKey}] Instance created: {spawnedInstance.name}");
        Debug.Log($"[AR-MUSEUM][Painting:{targetKey}] Instance active = {spawnedInstance.activeSelf}");

        // Position at anchor (small lift for visibility)
        spawnedInstance.transform.position = anchor.position + Vector3.up * 0.05f;
        spawnedInstance.transform.localScale = Vector3.one;

        Debug.Log($"[AR-MUSEUM][Painting:{targetKey}] Instance position set to {spawnedInstance.transform.position}");

        // Face camera (Y only)
        Camera cam = Camera.main;
        if (cam == null)
        {
            Debug.LogWarning($"[AR-MUSEUM][Painting:{targetKey}] Camera.main is NULL");
        }
        else
        {
            Vector3 lookDir = cam.transform.position - spawnedInstance.transform.position;
            lookDir.y = 0f;

            if (lookDir.sqrMagnitude > 0.001f)
            {
                spawnedInstance.transform.rotation = Quaternion.LookRotation(lookDir);
            }
        }

        // Parent to anchor stage
        spawnedInstance.transform.SetParent(anchor, true);

        Debug.Log($"[AR-MUSEUM][Painting:{targetKey}] Parent set to anchor");

        isPlaced = true;

        Debug.Log($"[AR-MUSEUM][Painting:{targetKey}] Placement COMPLETE");
    }
}
