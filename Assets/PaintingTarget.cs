using UnityEngine;

public class PaintingTarget : MonoBehaviour
{
    [Header("Painting Setup")]
    public string targetKey;
    public GameObject characterPrefab;

    [Header("References")]
    public PlacementRouter router;

    [Header("Automatic Placement")]
    public float autoPlacementTimeout = 1.5f;

    private GameObject spawnedInstance;
    private bool isPlaced = false;

    private Vector3 placedWorldPosition;
    private Quaternion placedWorldRotation;

    private bool autoPlacementRequested = false;

    public bool IsPlaced => isPlaced;

    private void Awake()
    {
        Debug.Log($"[AR-MUSEUM][Painting:{targetKey}] Awake");
    }

    // =============================
    // IMAGE TARGET EVENTS
    // =============================

    public void OnTargetFound()
    {
        Debug.Log($"[AR-MUSEUM][Painting:{targetKey}] OnTargetFound");

        // Already placed → just restore
        if (isPlaced && spawnedInstance != null)
        {
            spawnedInstance.SetActive(true);
            spawnedInstance.transform.position = placedWorldPosition;
            spawnedInstance.transform.rotation = placedWorldRotation;
            return;
        }

        router.SetCurrentTarget(this);

        autoPlacementRequested = true;
        router.RequestAutomaticPlacement();

        Invoke(nameof(AutoPlacementFallback), autoPlacementTimeout);
    }

    public void OnTargetLost()
    {
        if (isPlaced && spawnedInstance != null)
            spawnedInstance.SetActive(false);

        router.ClearCurrentTarget(this);
    }

    private void AutoPlacementFallback()
    {
        if (!isPlaced && autoPlacementRequested)
        {
            autoPlacementRequested = false;
            router.ShowStatus("Automatic placement failed. Tap to place guide");
        }
    }

    // =============================
    // PLACEMENT
    // =============================

    public void PlaceAt(Transform anchor)
    {
        if (anchor == null || isPlaced)
            return;

        autoPlacementRequested = false;
        CancelInvoke(nameof(AutoPlacementFallback));
        router.HideStatus();

        spawnedInstance = Instantiate(characterPrefab);
        spawnedInstance.name = characterPrefab.name + "_Instance";

        // Parent to floor anchor
        spawnedInstance.transform.SetParent(anchor, false);
        spawnedInstance.transform.localPosition = Vector3.zero;
        spawnedInstance.transform.localRotation = Quaternion.identity;

        // Feet aligned to floor
        spawnedInstance.transform.up = anchor.up;

        // Detach to freeze world transform
        spawnedInstance.transform.SetParent(null, true);

        // Orientation: wall + camera
        Vector3 wallNormal = transform.forward;
        Vector3 floorUp = anchor.up;
        Vector3 baseForward = -wallNormal;

        spawnedInstance.transform.rotation =
            Quaternion.LookRotation(baseForward, floorUp);

        Camera cam = Camera.main;
        if (cam != null)
        {
            Vector3 toCamera = cam.transform.position - spawnedInstance.transform.position;
            Vector3 flatDir = Vector3.ProjectOnPlane(toCamera, floorUp);

            if (flatDir.sqrMagnitude > 0.001f)
                spawnedInstance.transform.rotation =
                    Quaternion.LookRotation(flatDir, floorUp);
        }

        placedWorldPosition = spawnedInstance.transform.position;
        placedWorldRotation = spawnedInstance.transform.rotation;

        isPlaced = true;

        Debug.Log($"[AR-MUSEUM][Painting:{targetKey}] Placement COMPLETE");
    }
}
