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

    // Store final world transform
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

            //  Restore saved transform
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

        // TEMPORARILY parent to floor anchor
        spawnedInstance.transform.SetParent(anchor, false);
        spawnedInstance.transform.localPosition = Vector3.zero;
        spawnedInstance.transform.localRotation = Quaternion.identity;

        // Align character UP to the detected floor normal
        spawnedInstance.transform.up = anchor.up;

        // Detach to freeze world transform
        spawnedInstance.transform.SetParent(null, true);

        // =============================
        // ORIENTATION: wall + camera
        // =============================

        // Wall normal comes from the ImageTarget (this script is on it)
        Vector3 wallNormal = transform.forward;

        // True floor normal (use anchor, not Vector3.up)
        Vector3 floorUp = anchor.up;

        // Base forward: perpendicular to the wall
        Vector3 wallForward = -wallNormal;

        Debug.Log($"[AR-MUSEUM][Placement] Wall forward: {wallForward}");
        Debug.Log($"[AR-MUSEUM][Placement] Floor up: {floorUp}");
        // Initial rotation: torso aligned to wall & floor
        spawnedInstance.transform.rotation =
            Quaternion.LookRotation(wallForward, floorUp);

        // Rotate AROUND FLOOR to face the camera
        Camera cam = Camera.main;
        if (cam != null)
        {
            Vector3 toCamera = cam.transform.position - spawnedInstance.transform.position;

            // Project camera direction onto the floor plane
            Vector3 flatDir = Vector3.ProjectOnPlane(toCamera, floorUp);

            if (flatDir.sqrMagnitude > 0.001f)
            {
                spawnedInstance.transform.rotation =
                    Quaternion.LookRotation(flatDir, floorUp);
            }
        }

        // Save final world transform
        placedWorldPosition = spawnedInstance.transform.position;
        placedWorldRotation = spawnedInstance.transform.rotation;

        Debug.Log($"[AR-MUSEUM][Placement] Final world position: {placedWorldPosition}");
        Debug.Log($"[AR-MUSEUM][Placement] Final world rotation: {placedWorldRotation.eulerAngles}");

        isPlaced = true;

        Debug.Log($"[AR-MUSEUM][Painting:{targetKey}] Placement COMPLETE");
    }

}
