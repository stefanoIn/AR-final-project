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
        spawnedInstance.name = characterPrefab.name + "_Instance";

        // 1️⃣ Parent FIRST (important for correct transforms)
        spawnedInstance.transform.SetParent(anchor, false);

        // 2️⃣ Reset local transform
        spawnedInstance.transform.localPosition = Vector3.zero;
        spawnedInstance.transform.localRotation = Quaternion.identity;
        spawnedInstance.transform.localScale = Vector3.one;

        // 3️⃣ Drop character so feet touch the ground
        Renderer[] renderers = spawnedInstance.GetComponentsInChildren<Renderer>();
        if (renderers.Length > 0)
        {
            Bounds bounds = renderers[0].bounds;
            foreach (var r in renderers)
                bounds.Encapsulate(r.bounds);

            float bottomY = bounds.min.y;
            float offset = anchor.position.y - bottomY;

            spawnedInstance.transform.position += Vector3.up * offset;

            Debug.Log($"[AR-MUSEUM][Painting:{targetKey}] Ground offset applied: {offset}");
        }
        else
        {
            Debug.LogWarning($"[AR-MUSEUM][Painting:{targetKey}] No Renderer found for grounding");
        }

        // 4️⃣ Face camera (Y only)
        Camera cam = Camera.main;
        if (cam != null)
        {
            Vector3 lookDir = cam.transform.position - spawnedInstance.transform.position;
            lookDir.y = 0f;

            if (lookDir.sqrMagnitude > 0.001f)
                spawnedInstance.transform.rotation = Quaternion.LookRotation(lookDir);
        }

        isPlaced = true;

        Debug.Log($"[AR-MUSEUM][Painting:{targetKey}] Placement COMPLETE");
    }

}
