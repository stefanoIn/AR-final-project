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

    // 🔒 Store final world transform
    private Vector3 placedWorldPosition;
    private Quaternion placedWorldRotation;

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

        router.SetCurrentTarget(this);

        if (isPlaced && spawnedInstance != null)
        {
            spawnedInstance.SetActive(true);

            // 🔁 Restore saved transform
            spawnedInstance.transform.position = placedWorldPosition;
            spawnedInstance.transform.rotation = placedWorldRotation;

            Debug.Log($"[AR-MUSEUM][Painting:{targetKey}] Restored position {placedWorldPosition}");
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

    public void PlaceAt(Transform anchor)
    {
        Debug.Log($"[AR-MUSEUM][Painting:{targetKey}] PlaceAt CALLED");

        if (anchor == null || isPlaced)
            return;

        // Instantiate
        spawnedInstance = Instantiate(characterPrefab);
        spawnedInstance.name = characterPrefab.name + "_Instance";

        // TEMPORARILY parent to anchor
        spawnedInstance.transform.SetParent(anchor, false);
        spawnedInstance.transform.localPosition = Vector3.zero;
        spawnedInstance.transform.localRotation = Quaternion.identity;

        // 🔓 DETACH — critical fix
        spawnedInstance.transform.SetParent(null, true);

        // Save final world transform
        placedWorldPosition = spawnedInstance.transform.position;
        placedWorldRotation = spawnedInstance.transform.rotation;

        Debug.Log($"[AR-MUSEUM][Placement] Final world position: {placedWorldPosition}");

        // Face camera (Y only)
        Camera cam = Camera.main;
        if (cam != null)
        {
            Vector3 dir = cam.transform.position - placedWorldPosition;
            dir.y = 0f;

            if (dir.sqrMagnitude > 0.001f)
                spawnedInstance.transform.rotation = Quaternion.LookRotation(dir);

            placedWorldRotation = spawnedInstance.transform.rotation;
        }

        isPlaced = true;

        Debug.Log($"[AR-MUSEUM][Painting:{targetKey}] Placement COMPLETE");
    }
}
