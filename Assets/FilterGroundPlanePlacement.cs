using UnityEngine;
using Vuforia;

public class FilterGroundPlanePlacement : MonoBehaviour
{
    public ContentPositioningBehaviour contentPositioning;
    public GameObject planeFinder;

    [Range(0.5f, 1f)]
    public float minUpwardDot = 0.8f;

    private bool placed = false;

    // Keeps the plane indicator alive (AUTOMATIC mode requirement)
    public void OnAutomaticHitTest(HitTestResult result)
    {
        // Do nothing on purpose
    }

    // Called when the user taps
    public void OnInteractiveHitTest(HitTestResult result)
    {
        if (placed)
            return;

        // Compute surface "up" direction
        Vector3 surfaceUp = result.Rotation * Vector3.up;

        // Check if surface is mostly horizontal
        if (Vector3.Dot(surfaceUp, Vector3.up) > minUpwardDot)
        {
            contentPositioning.PositionContentAtPlaneAnchor(result);
            placed = true;

            // Freeze placement
            planeFinder.SetActive(false);
        }
        else
        {
            Debug.Log("Hit a wall or steep surface — ignored");
        }
    }
}
